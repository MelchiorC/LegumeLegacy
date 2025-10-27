using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopUp : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float time = 0.2f;
    public GameObject Hover;
    public Boolean timeUp = true;
    

    void Start()
    {
        GameObject parentObject = GameObject.Find("Manager");
        Hover = parentObject.transform.Find("3dPopUp").gameObject;
        Hover.SetActive(false);
    }
    void Update()
    {
        if(timeUp == false)
        {
            time -= Time.deltaTime;
            if(time <= 0)
            {
                Hover.SetActive(false);
                timeUp = true;
                time = 3f;
            }
        }

    }
    // New position for the object
    public Vector3 newPosition;
    public void OnPointerEnter(PointerEventData eventData)
    {

        
        Hover.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        timeUp = false;
        
    }
    public void ReceiveHighlightedPosition(Vector3 position)
    {
        float yOffset = 2.5f;
        float zOffset = 3f;
        newPosition = new Vector3(position.x, position.y + yOffset, position.z +zOffset);
        Hover.transform.position = newPosition;
    }
}

