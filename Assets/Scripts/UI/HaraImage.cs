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

    public void StickYes(bool stick, bool treli)
    {

            Image img = Stick.GetComponent<Image>();
            TMP_Text text = StickText.GetComponent<TMP_Text>();
            if (treli == false)
            {
                img.gameObject.SetActive(false);
                text.gameObject.SetActive(false);
                StickBox.gameObject.SetActive(false);
            }
            else
            {
                img.gameObject.SetActive(true);
                text.gameObject.SetActive(true);
                StickBox.gameObject.SetActive(true);
                if (stick == true)
                {
                    img.sprite = YesStick;
                    text.text = "Tanaman sudah diberi penyangga";
                }
                else
                {
                    img.sprite = NoStick;
                    text.text = "Tanaman belum diberi penyangga";
                }
            }
        
    }
}
