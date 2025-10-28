using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CropBehaviour : MonoBehaviour
{
    // Information on what the crop will grow into
    public SeedData seedToGrow;
    SeedData waste;
    private Soil planted;

    [Header("Stages of life")]
    public GameObject seed;
    public GameObject seedling;
    public GameObject seedling2;
    public GameObject mature;
    public GameObject mature2;
    public GameObject mature3;
    public GameObject harvestable;

    // The growth points of the crop
    public int growth;
    // How many growth points it takes before it becomes harvestable
    int maxGrowth;

    public enum CropState
    {
        Seed, Seedling, Seedling2, Mature, Mature2, Mature3, Harvestable
    }

    // The current stage in the crop's growth
    public CropState cropState;

    // Initialization for the crop gameobject
    private void Start()
    {
        if (planted == null)
        {
            Debug.Log("planted");
        }
    }

    public void Plant(SeedData seedToGrow, Soil soil)
    {
        this.seedToGrow = seedToGrow;
        seedling = Instantiate(seedToGrow.seedling, transform);
        seedling2 = Instantiate(seedToGrow.seedling2, transform);
        planted = soil;
        mature = Instantiate(seedToGrow.mature, transform);
        mature2 = Instantiate(seedToGrow.mature2, transform);
        mature3 = Instantiate(seedToGrow.mature3, transform);
        harvestable = Instantiate(seedToGrow.harvestable, transform);

        maxGrowth = seedToGrow.daysToGrow;

        SwitchState(CropState.Seed);

        // Report quest progress
        QuestManager.Instance.ReportAction(QuestData.QuestType.Plant, seedToGrow.seedType);
    }

    // The crop will grow when watered
    public void Grow()
    {
        // Increase the growth points by 1
        growth++;

        // The seed will sprout into a seedling
        if (growth >= maxGrowth * 1 && cropState == CropState.Seed)
        {
            SwitchState(CropState.Seedling);
        }

        // Check if compost has been applied
        if (planted.status.Compost)
        {
            // Skip Seedling2 and Mature3 if compost is applied
            // Grow from seedling directly to mature
            if (growth >= maxGrowth * 2 && cropState == CropState.Seedling)
            {
                SwitchState(CropState.Mature);
            }

            // Grow from mature to mature2
            if (growth >= maxGrowth * 3 && cropState == CropState.Mature)
            {
                SwitchState(CropState.Mature2);
            }

            // Grow from mature2 directly to harvestable (skip Mature3)
            if (growth >= maxGrowth * 4 && cropState == CropState.Mature2)
            {
                SwitchState(CropState.Harvestable);
            }
        }
        else
        {
            // Normal growth process without compost
            // Grow from seedling to seedling2
            if (growth >= maxGrowth * 2 && cropState == CropState.Seedling)
            {
                SwitchState(CropState.Seedling2);
            }

            // Grow from seedling2 to mature
            if (growth >= maxGrowth * 3 && cropState == CropState.Seedling2)
            {
                SwitchState(CropState.Mature);
            }

            // Grow from mature to mature2
            if (growth >= maxGrowth * 4 && cropState == CropState.Mature)
            {
                SwitchState(CropState.Mature2);
            }

            // Grow from mature2 to mature3
            if (growth >= maxGrowth * 5 && cropState == CropState.Mature2)
            {
                SwitchState(CropState.Mature3);
            }

            // Grow from mature3 to harvestable
            if (growth >= maxGrowth * 6 && cropState == CropState.Mature3)
            {
                SwitchState(CropState.Harvestable);
            }
        }
        gameObject.SetActive(true);

        // Report watering for quest
        QuestManager.Instance.ReportAction(QuestData.QuestType.Water, seedToGrow.seedType);
    }


    // Function to handle the state changes
    void SwitchState(CropState stateToSwitch)
    {
        seed.SetActive(false);
        seedling.SetActive(false);
        seedling2.SetActive(false);
        mature.SetActive(false);
        mature2.SetActive(false);
        mature3.SetActive(false);
        harvestable.SetActive(false);

        switch (stateToSwitch)
        {
            case CropState.Seed:
                seed.SetActive(true);
                break;
            case CropState.Seedling:
                seedling.SetActive(true);
                break;
            case CropState.Seedling2:
                seedling2.SetActive(true);
                break;
            case CropState.Mature:
                planted.status.Compost = false;
                mature.SetActive(true);
                break;
            case CropState.Mature2:
                mature2.SetActive(true);
                break;
            case CropState.Mature3:
                mature3.SetActive(true);
                break;
            case CropState.Harvestable:
                harvestable.GetComponent<InteractableObject>().boost += planted.status.TotalPupuk;
                if (planted.status.LandRotation)
                {
                    harvestable.GetComponent<InteractableObject>().boost += 1;
                }
                planted.status.TotalPupuk = 0;
                planted.status.Compost = false;
                harvestable.SetActive(true);
                harvestable.transform.parent = null;
                if (seedToGrow.seedType == "Legume")
                {
                    planted.status.LandRotation = true;
                }
                else
                {
                    planted.status.LandRotation = false;
                }
                break;
        }

        cropState = stateToSwitch;
    }
}
