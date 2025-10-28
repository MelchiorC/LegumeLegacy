using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using System;

public class PlayerController : MonoBehaviour
{
    const string IDLE = "Idle";
    const string WALK = "Walk";
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
    Animator animator;
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
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
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
        if (ONui) return;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, clickableLayers))
        {
            if (!agent.enabled) agent.enabled = true; // Enable agent only if it was disabled
            agent.ResetPath();
            agent.isStopped = false;
            agent.destination = hit.point;

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
            if (agent.enabled && !agent.isStopped) agent.isStopped = true; // Only stop if agent is enabled
            MoveWithWASD();
        }
        else
        {
            if (agent.enabled && agent.isStopped) agent.isStopped = false; // Only start if agent is enabled
            FaceTarget();
        }

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
        if (moveInput != Vector2.zero || agent.velocity != Vector3.zero)
        {
            animator.Play(WALK);
        }
        else
        {
            animator.Play(IDLE);
        }
    }

    void HandleUIInteraction()
    {
        if (Input.GetKeyDown(KeyCode.B)) UI.ToggleInventoryPanel();
        ONui = CompostUI.activeInHierarchy || Backpack.activeSelf;
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
