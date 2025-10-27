using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class StatsShower : MonoBehaviour
{

    public static StatsShower _instance;

    public TextMeshProUGUI txtComp;

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {

            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void SetAnsshow(string message)
    {
        gameObject.SetActive(true);
        txtComp.text = message;
    }

    public void HideTool()
    {
        gameObject.SetActive(false);
        txtComp.text = string.Empty;
    }
}
