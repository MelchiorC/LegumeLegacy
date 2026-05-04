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
    public Sprite PathogenicFungiIcon;
    public Sprite AphidsIcon;
    public Sprite ArmywormIcon;
    public Sprite CutwormIcon;
    public Sprite CabbageWormIcon;
    public Sprite SpiderMitesIcon;
    public GameObject StickText;
    public GameObject StickBox;

    public Button learnMoreButton;
    public TMP_Text learnMoreText;

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

    // hide button by default
    learnMoreButton.gameObject.SetActive(false);

    switch (pestType)
    {
        case Soil.PestType.None:
            img.sprite = YesStick;
            text.text = "Tanaman Sehat";
            break;
        case Soil.PestType.PathogenicFungi:
            img.sprite = PathogenicFungiIcon;
            text.text = "Jamur patogen menyerang tanaman.";
            ShowLearnMore(pestType);
            break;
        case Soil.PestType.Aphids:
            img.sprite = AphidsIcon;
            text.text = "Kutu daun menyerang tanaman";
            ShowLearnMore(pestType);
            break;
        case Soil.PestType.Armyworm:
            img.sprite = ArmywormIcon;
            text.text = "Ulat grayak menyerang tanaman";
            ShowLearnMore(pestType);
            break;
        case Soil.PestType.Cutworm:
            img.sprite = CutwormIcon;
            text.text = "Ulat tanah menyerang tanaman";
            ShowLearnMore(pestType);
            break;
        case Soil.PestType.CabbageWorm:
            img.sprite = CabbageWormIcon;
            text.text = "Ulat kubis menyerang tanaman";
            ShowLearnMore(pestType);
            break;
        case Soil.PestType.SpiderMites:
            img.sprite = SpiderMitesIcon;
            text.text = "Kutu laba-laba menyerang tanaman";
            ShowLearnMore(pestType);
            break;
    }
}
    // Backward compatibility for existing callsites.
    public void StickYes(bool stick, bool treli)
    {
        PestStatus(stick ? Soil.PestType.PathogenicFungi : Soil.PestType.None);
    }

    private void ShowLearnMore(Soil.PestType pestType)
{
    learnMoreButton.gameObject.SetActive(true);
    learnMoreButton.onClick.RemoveAllListeners();
    learnMoreButton.onClick.AddListener(() => 
        UIManager.Instance.OpenPestEntry(pestType));
}

}
