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

    //Interaction Components
    PlayerInteraction playerInteraction;

    [Header("Movement")]
    [SerializeField] ParticleSystem clickEffect;
    [SerializeField] LayerMask clickableLayers;

    float lookRotationSpeed = 8f;

    private void Start()
    {
        //Get interaction components
        playerInteraction = GetComponentInChildren<PlayerInteraction>();
        if(Skipper == null) Debug.Log("Error");
    }
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        input = new CustomActions();
        AssignInputs();
    }
    void AssignInputs()
    {
        input.Main.Move.performed += ctx => ClickToMove();
    }

    void ClickToMove()
    {
        RaycastHit hit;
        if (ONui == true)
        {
            return;
        }
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, clickableLayers))
        {
            
            agent.destination = hit.point;
            if (clickEffect != null)
            {
                
                ParticleSystem effect = Instantiate(clickEffect, hit.point += new Vector3(0, 0.1f, 0), clickEffect.transform.rotation);

                Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
            }
        }
    }

    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }
    void Update()
    {
        FaceTarget();
        SetAnimations();

        if (Input.GetKeyDown(KeyCode.B))
        {
            UI.ToggleInventoryPanel();
        }

        // Runs the function that handles all the interaction
        Interact();

        // Update ONui flag based on active UI elements, but allow movement if only HaraUI is active
        if (CompostUI.activeInHierarchy || Backpack.gameObject.activeSelf)
        {
            ONui = true;
        }
        else
        {
            ONui = false;  // HaraUI being active no longer sets ONui to true
        }
    }

    public void Interact()
    {
        // Tool interaction
        if (Input.GetKeyDown(KeyCode.F))
        {
            // Interact
            playerInteraction.Interact();

            // Close HaraUI if interacting
            if (ONui)
            {
                HaraUI.gameObject.SetActive(false);
            }
        }

        // Item Interaction
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!ONui)
            {
                ONui = compost.gameObject.GetComponent<CompostShower>().CompostUI();
            }
            else
            {
                ONui = false;
                compost.gameObject.GetComponent<CompostShower>().HideUI();
            }

            Skipper.gameObject.GetComponent<TimeSkip>().TimeSkiper();
            playerInteraction.ItemInteract();
        }

        // Item Keep
        if (Input.GetKeyDown(KeyCode.G))
        {
            playerInteraction.ItemKeep();
        }
    }
    void FaceTarget()
    {
        if (agent.velocity != Vector3.zero)
        {
            Vector3 direction = (agent.destination - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
        }
    }

    //Play Animations
    void SetAnimations()
    {
        if (agent.velocity == Vector3.zero)
        {
            animator.Play(IDLE);
        }
        else
        {
            animator.Play(WALK);
        }
    }
}