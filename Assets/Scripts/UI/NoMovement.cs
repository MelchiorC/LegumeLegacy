using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NoMovement : MonoBehaviour
{
    public List<GameObject> clickableObjects;
    private List<BoxCollider2D> colliders;

    void Start()
    {
        colliders = new List<BoxCollider2D>(); // Initialize the list

        // Iterate through each GameObject in the clickableObjects list
        foreach (GameObject go in clickableObjects)
        {
            // Get the BoxCollider2D component
            BoxCollider2D collider = go.GetComponent<BoxCollider2D>();

            // Check if the component exists and add it to the list
            if (collider != null)
            {
                colliders.Add(collider);
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            Debug.Log(hit);

            if(hit.collider != null )

            foreach (BoxCollider2D  collider in colliders)
            {
                if (hit.collider == collider)
                {
                    Debug.Log(collider.gameObject.name);
                }
            }
        }
    } }
