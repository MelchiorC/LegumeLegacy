using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using System;

public class PlayerController : MonoBehaviour
{
    const string IDLE = "Idle";
    const string WALK = "Walk";
    const string IDLE_STATE_PATH = "Base Layer.Idle";
    const string WALK_STATE_PATH = "Base Layer.Walk";
    const float INPUT_MOVE_THRESHOLD = 0.01f;
    const float POSITION_MOVE_THRESHOLD = 0.0001f;
    const float ARRIVAL_VELOCITY_THRESHOLD = 0.01f;
    const float NAVIGATION_SETTLE_TIME = 0.1f;
    public Boolean ONui = false;

    public UIManager UI;
    CustomActions input;
    public TimeSkip Skipper;
    public ShopShower Shop;
    public ShippingBin Bin;
    public GameObject ShippingBinUI;
    public GameObject ShopUI;
    public GameObject CompostUI;
    public CompostShower compost;
    public GameObject Backpack;
    public GameObject HaraUI;

    NavMeshAgent agent;
    Animator[] animators;
    string currentAnimation;
    Vector3 lastAnimationPosition;
    bool isClickNavigating;
    float clickNavigationStartTime;
    CharacterController characterController;

    PlayerInteraction playerInteraction;

    [Header("Movement")]
    [SerializeField] ParticleSystem clickEffect;
    [SerializeField] LayerMask clickableLayers;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 8f;

    Vector2 moveInput;

    private void Start()
    {
        playerInteraction = GetComponentInChildren<PlayerInteraction>();
        if (Skipper == null) Debug.Log("Error");
    }
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animators = GetComponentsInChildren<Animator>();
        characterController = GetComponent<CharacterController>();
        lastAnimationPosition = transform.position;
        input = new CustomActions();
        AssignInputs();
    }
    void AssignInputs()
    {
        input.Main.Move.performed += ctx => ClickToMove();
        input.Main.WASD.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Main.WASD.canceled += ctx => moveInput = Vector2.zero;
    }

    void ClickToMove()
    {
        // Prevent movement if UI is active
        if (ONui) return;

        // Use RaycastAll only, skip IsPointerOverGameObject entirely.
        if (EventSystem.current != null)
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject.GetComponent<UnityEngine.UI.Graphic>() != null)
                    return;
            }
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, clickableLayers))
        {
            if (!agent.enabled) agent.enabled = true; // Enable agent only if it was disabled
            agent.ResetPath();
            agent.isStopped = false;
            agent.destination = hit.point;
            isClickNavigating = true;
            clickNavigationStartTime = Time.time;
            ChangeAnimation(WALK);

            if (clickEffect != null)
            {
                ParticleSystem effect = Instantiate(clickEffect, hit.point + new Vector3(0, 0.1f, 0), clickEffect.transform.rotation);
                Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
            }
        }
    }

    void OnEnable() { input.Enable(); }
    void OnDisable() { input.Disable(); }

    void Update()
    {
        if (moveInput != Vector2.zero)
        {
            isClickNavigating = false;
            if (agent.enabled && !agent.isStopped) agent.isStopped = true; // Only stop if agent is enabled
            MoveWithWASD();
        }
        else
        {
            if (agent.enabled && agent.isStopped) agent.isStopped = false; // Only start if agent is enabled
            FaceTarget();
        }

        UpdateClickNavigationState();
        SetAnimations();
        HandleUIInteraction();
        Interact();
    }

    void MoveWithWASD()
    {
        if (agent.enabled)
        {
            agent.isStopped = true;
            agent.enabled = false;  // Disable NavMeshAgent to allow manual movement
        }

        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        // Apply movement
        moveDirection *= moveSpeed;

        // Apply gravity only if not grounded
        if (!characterController.isGrounded)
        {
            moveDirection.y -= 9.81f * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    void FaceTarget()
    {
        if (agent.velocity != Vector3.zero)
        {
            Vector3 direction = (agent.destination - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void SetAnimations()
    {
        if (animators == null || animators.Length == 0) return;

        Vector3 planarDelta = transform.position - lastAnimationPosition;
        planarDelta.y = 0f;
        lastAnimationPosition = transform.position;

        bool isMovingWithInput = moveInput.sqrMagnitude > INPUT_MOVE_THRESHOLD;
        bool movedSinceLastFrame = planarDelta.sqrMagnitude > POSITION_MOVE_THRESHOLD;
        bool hasActiveAgentPath = agent != null &&
                                  agent.enabled &&
                                  !agent.isStopped &&
                                  agent.hasPath &&
                                  agent.remainingDistance > agent.stoppingDistance + 0.05f;

        string targetAnimation = isMovingWithInput || isClickNavigating || movedSinceLastFrame || hasActiveAgentPath ? WALK : IDLE;

        ChangeAnimation(targetAnimation);
    }

    void UpdateClickNavigationState()
    {
        if (!isClickNavigating)
        {
            return;
        }

        if (Time.time - clickNavigationStartTime < NAVIGATION_SETTLE_TIME)
        {
            return;
        }

        if (agent == null || !agent.enabled || agent.isStopped)
        {
            isClickNavigating = false;
            return;
        }

        if (agent.pathPending)
        {
            return;
        }

        bool hasArrived = agent.remainingDistance <= agent.stoppingDistance + 0.05f &&
                          agent.velocity.sqrMagnitude <= ARRIVAL_VELOCITY_THRESHOLD;

        if (hasArrived || !agent.hasPath)
        {
            isClickNavigating = false;
        }
    }

    void ChangeAnimation(string targetAnimation)
    {
        if (animators == null || animators.Length == 0)
        {
            return;
        }

        if (currentAnimation == targetAnimation && AreAnimatorsInState(targetAnimation))
        {
            return;
        }

        foreach (Animator targetAnimator in animators)
        {
            if (targetAnimator == null || !targetAnimator.isActiveAndEnabled || targetAnimator.runtimeAnimatorController == null)
            {
                continue;
            }

            int stateHash = GetAnimationStateHash(targetAnimator, targetAnimation);
            targetAnimator.CrossFadeInFixedTime(stateHash, 0.08f, 0);
        }

        currentAnimation = targetAnimation;
    }

    int GetAnimationStateHash(Animator targetAnimator, string animationName)
    {
        int fullPathHash = Animator.StringToHash(animationName == WALK ? WALK_STATE_PATH : IDLE_STATE_PATH);
        if (targetAnimator.HasState(0, fullPathHash))
        {
            return fullPathHash;
        }

        return Animator.StringToHash(animationName);
    }

    bool AreAnimatorsInState(string animationName)
    {
        foreach (Animator targetAnimator in animators)
        {
            if (targetAnimator == null || !targetAnimator.isActiveAndEnabled || targetAnimator.runtimeAnimatorController == null)
            {
                continue;
            }

            if (!IsAnimatorStateActive(targetAnimator, animationName))
            {
                return false;
            }
        }

        return true;
    }

    bool IsAnimatorStateActive(Animator targetAnimator, string animationName)
    {
        int fullPathHash = Animator.StringToHash(animationName == WALK ? WALK_STATE_PATH : IDLE_STATE_PATH);
        int shortNameHash = Animator.StringToHash(animationName);

        AnimatorStateInfo currentState = targetAnimator.GetCurrentAnimatorStateInfo(0);
        if (currentState.fullPathHash == fullPathHash || currentState.shortNameHash == shortNameHash)
        {
            return true;
        }

        if (!targetAnimator.IsInTransition(0))
        {
            return false;
        }

        AnimatorStateInfo nextState = targetAnimator.GetNextAnimatorStateInfo(0);
        return nextState.fullPathHash == fullPathHash || nextState.shortNameHash == shortNameHash;
    }

    void HandleUIInteraction()
    {
        if (Input.GetKeyDown(KeyCode.B)) UI.ToggleInventoryPanel();
        ONui = CompostUI.activeInHierarchy || Backpack.activeSelf || ShopUI.activeInHierarchy || ShippingBinUI.activeInHierarchy;
    }

    public void Interact()
    {
        if (Input.GetKeyDown(KeyCode.F)) playerInteraction.Interact();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!ONui)
            {
                ONui = compost.gameObject.GetComponent<CompostShower>().CompostUI();
            }
            else
            {
                compost.gameObject.GetComponent<CompostShower>().HideUI();
            }

            Skipper.gameObject.GetComponent<TimeSkip>().TimeSkiper();
            playerInteraction.ItemInteract();
        }

        if (Input.GetKeyDown(KeyCode.G)) playerInteraction.ItemKeep();
    }
}
