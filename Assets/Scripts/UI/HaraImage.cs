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

    public void PestStatus(Soil.PestType pestType)
    {
        Image img = Stick.GetComponent<Image>();
        TMP_Text text = StickText.GetComponent<TMP_Text>();

        img.gameObject.SetActive(true);
        text.gameObject.SetActive(true);
        StickBox.gameObject.SetActive(true);

        switch (pestType)
        {
            case Soil.PestType.None:
                img.sprite = YesStick;
                text.text = "Status hama aman";
                break;
            case Soil.PestType.PathogenicFungi:
                img.sprite = NoStick;
                text.text = "Peringatan: Jamur patogen menyerang tanaman. Gunakan pesticide.";
                break;
            case Soil.PestType.Aphids:
                img.sprite = NoStick;
                text.text = "Peringatan: Kutu daun menyerang tanaman. Gunakan ladybug.";
                break;
            case Soil.PestType.Armyworm:
                img.sprite = NoStick;
                text.text = "Peringatan: Ulat grayak menyerang tanaman. Gunakan pesticide.";
                break;
        }
    }

    // Backward compatibility for existing callsites.
    public void StickYes(bool stick, bool treli)
    {
        PestStatus(stick ? Soil.PestType.PathogenicFungi : Soil.PestType.None);
    }
}
