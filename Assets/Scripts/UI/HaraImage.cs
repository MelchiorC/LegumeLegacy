using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HaraImage : MonoBehaviour
{
    public GameObject Water;
    public Sprite Unwatered;
    public Sprite Watered;
    public GameObject WaterText;

    public GameObject Compost;
    public Sprite NoCompost;
    public Sprite YesCompost;
    public GameObject CompostText;

    public GameObject Stick;
    public Sprite NoStick;
    public Sprite YesStick;
    public GameObject StickText;
    public GameObject StickBox;

    public void IsItWatered(bool water)
    {
        Image img = Water.GetComponent<Image>();
        TMP_Text text = WaterText.GetComponent<TMP_Text>();
        if (WaterText == null)
        {
            Debug.LogError("WaterText GameObject is not assigned.");
        }
        if (water == true)
        {
            text.text = "Tanaman sudah disiram";
            img.sprite = Watered;
        }
        else
        {
            img.sprite = Unwatered;
            text.text = "Tanaman belum disiram";
        }
    }

    public void CompostYes(bool compost) 
    {
        Image img = Compost.GetComponent<Image>();
        TMP_Text text = CompostText.GetComponent<TMP_Text>();
        if (compost == true)
        {
            img.sprite = YesCompost;
            text.text = "Tanaman sudah dipupuk";
        }
        else
        {
            img.sprite = NoCompost;
            text.text = "Tanaman belum dipupuk";
        }
    }

    public void PestStatus(bool hasPest)
    {
        Image img = Stick.GetComponent<Image>();
        TMP_Text text = StickText.GetComponent<TMP_Text>();

        img.gameObject.SetActive(true);
        text.gameObject.SetActive(true);
        StickBox.gameObject.SetActive(true);

        if (hasPest)
        {
            img.sprite = NoStick;
            text.text = "Peringatan: Hama menyerang tanaman";
        }
        else
        {
            img.sprite = YesStick;
            text.text = "Status hama aman";
        }
    }

    // Backward compatibility for existing callsites.
    public void StickYes(bool stick, bool treli)
    {
        PestStatus(stick);
    }
}
