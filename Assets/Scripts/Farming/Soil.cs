using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soil : MonoBehaviour, ITimeTracker
{
    public enum LandStatus
    {
        Soil, Compost,Curved,CurvedCompost,Watered, Growing, Harvested, Default, Stick
    }

    public enum PestType
    {
        None,
        PathogenicFungi,
        Aphids,
        Armyworm,
        Cutworm,
        CabbageWorm,
        SpiderMites
    }
    
    public LandStatus landStatus;
    public Stat status;
    public UIHara Hara;

    public Material DrySoilMat, WetSoilMat, GrowingMat, HarvestedMat, DefaultMat;
    public Material UsableUV, WateredUV;

    new Renderer renderer;
    private MeshFilter filter;

    public Mesh Default, Compost, Curved,CurvedCompost;

    //The selection gameobject to enable when the player is selecting the land
    public GameObject select;

    //Cache the time the land was watered
    [SerializeField]
    public GameTimestamp timeWatered;
    [SerializeField]
    public GameTimestamp timeWatered2;



    [Header("Crops")]
    //The crop prefab to instantiate
    public GameObject cropPrefab;

    [Header("Pest Control")]
    [Range(0f, 1f)]
    // There is a 30% base chance each crop-day that pests will attack.
    [SerializeField] private float dailyPestAttackChance = 0.3f;
    [Range(0f, 1f)]

    // Crop rotation lowers the pest attack chance to 60% of the normal value, so pests become less likely.
    [SerializeField] private float cropRotationPestMultiplier = 0.6f;
    [Range(0.1f, 5f)]

    // Pathogenic fungi are more likely to appear during the rainy season, so their chance is multiplied by 3.0x.
    [SerializeField] private float rainySeasonFungiMultiplier = 3.0f;
    [Range(0.1f, 5f)]

    // Pathogenic fungi are less likely to appear during the dry season, so their chance is multiplied by 0.2x.
    [SerializeField] private float summerSeasonFungiMultiplier = 0.2f;
    [SerializeField] private PestType activePest = PestType.None;
    public PestType ActivePest => activePest;
    public bool HasPests => activePest != PestType.None;

    //The crop currently planted on the land
    CropBehaviour cropPlanted = null;
   
    // Start is called before the first frame update

    void Start()
    {
        status = GetComponent<Stat>();

        filter = GetComponent<MeshFilter>();

        //Get renderer component
        renderer = GetComponent<Renderer>();

        //Set the land to soil by default
        SwitchLandStatus(LandStatus.Default);

        //Deselect the land by default
        Select(false);

        //Add this to TimeManager's listener list
        TimeManager.Instance.RegisterTracker(this);
    }

    public void SwitchLandStatus(LandStatus statusToSwitch)
    {
        //Set land status accordingly
        landStatus = statusToSwitch;

        Material currentMaterial = renderer.material;

        Material materialToSwitch = DrySoilMat;
       
        //Declare what material to switch to
        switch (statusToSwitch)
        {
            case LandStatus.Soil:
                //Switch to the soil material
                materialToSwitch = UsableUV;
                changeToCurved();
                break;

            case LandStatus.Compost:
                //Switch mesh and material to compost
                if (filter.mesh.name == "Cube.011 Instance")
                {
                    filter.mesh = CurvedCompost;
                }
                else
                {
                    filter.mesh = Compost;

                }
                timeWatered = TimeManager.Instance.GetGameTimestamp();
                //if (currentMaterial.name == "WateredUV (Instance)") materialToSwitch = WateredUV;
                
                Debug.Log(currentMaterial.name);
                materialToSwitch = UsableUV;
                break;

            case LandStatus.Curved:
                materialToSwitch = UsableUV;
                filter.mesh = Curved;
                break;

            case LandStatus.CurvedCompost:
                
                filter.mesh = CurvedCompost;
                break;

            case LandStatus.Watered:
                //Switch to Watered material
                
                materialToSwitch = WateredUV;
                //Cache the time it was watered
                timeWatered = TimeManager.Instance.GetGameTimestamp();
                break;

            case LandStatus.Growing:
                //Switch to Growing material
                materialToSwitch = GrowingMat;
                break;

            case LandStatus.Harvested:
                //Switch to Harvested Material 
                materialToSwitch = HarvestedMat;
                break;

            case LandStatus.Default:
                //Switch to Default Material & Mesh
                materialToSwitch = DefaultMat;
                filter.mesh = Default;
                break;
        }
   
        //Get the renderer to apply change
        renderer.material = materialToSwitch;
    }

    public void Select(bool toggle)
    {
        // Enable or disable the selection indicator
        select.SetActive(toggle);

        // Show the UI only when the land is selected (toggle is true)
        if (toggle)
        {
            // Check if a crop is planted
            if (cropPlanted != null)
            {
                // Check if the planted crop requires a trellis
                if (DatabaseBibit.Instance.CheckTreli(cropPlanted.seedToGrow))
                {
                    UIManager.Instance.OpenUI(status.Water, status.Compost, activePest);
                }
                else
                {
                    UIManager.Instance.OpenUI(status.Water, status.Compost, activePest);
                }
            }
            
        }
        else
        {
            // Hide the UI when deselecting the land
            UIManager.Instance.CloseUI();
        }
    }

    public void changeToCompost()
    {
        filter.mesh = Compost;
    }

    public void changeToCurved()
    {
        filter.mesh = Curved;
    }

    public void changeToCurvedCompost() 
    {
        filter.mesh = CurvedCompost;
    }

    public void changeToDefault()
    {
        filter.mesh = Default;
    }

    //When the player presses the interact button while selecting this land
    public void Interact()
    {
        //Check the player's tool slot
        ItemData toolSlot = InventoryManager.Instance.GetEquippedSlotItem(InventorySlot.InventoryType.Tool);
        //If there's nothing equipped, return
        if(!InventoryManager.Instance.SlotEquipped(InventorySlot.InventoryType.Tool))
        {
            return;
        }

        //Try casting the itemdata in the toolslot as equipmentdata
        EquipmentData equipmentTool = toolSlot as EquipmentData;

        //Check if it is of type EquipmentData
        if(equipmentTool != null)
        {
            //Get the tool type
            EquipmentData.ToolType toolType = equipmentTool.toolType;
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayToolSFX(toolType);
            }

            switch (toolType)
            {
                case EquipmentData.ToolType.Shovel:
                    
                    //Remove the crop from the soil
                    if(cropPlanted != null)
                    {
                        Destroy(cropPlanted.gameObject);
                        activePest = PestType.None;
                        SwitchLandStatus(LandStatus.Default);

                        break;
                    }
                    SwitchLandStatus(LandStatus.Curved);
                    break;

                case EquipmentData.ToolType.WateringCan:
                    status.Water = true;
                    SwitchLandStatus(LandStatus.Watered);

                    // Report watering for quest
                    if (cropPlanted != null && cropPlanted.seedToGrow != null)
                    {
                        QuestManager.Instance.ReportAction(QuestData.QuestType.Water, cropPlanted.seedToGrow.seedType);
                    }
                    break;

                case EquipmentData.ToolType.Compost:
                    
                    if(status.Compost == false)
                    {
                        status.Compost = true;
                        status.TotalPupuk += 1;
                    }

                    SwitchLandStatus(LandStatus.Compost);
                    if(status.Water ==true)
                    {
                        SwitchLandStatus(LandStatus.Watered);
                    }
                    break;

                case EquipmentData.ToolType.Stick:
                    SwitchLandStatus(LandStatus.Stick);
                    break;

                case EquipmentData.ToolType.Sickle:
                    SwitchLandStatus(LandStatus.Harvested);
                    Destroy(cropPlanted.gameObject);
                    activePest = PestType.None;
                    SwitchLandStatus(LandStatus.Default);
                    break;

                case EquipmentData.ToolType.Pesticide:
                    ControlPests(EquipmentData.ToolType.Pesticide);
                    break;

                case EquipmentData.ToolType.Ladybug:
                    ControlPests(EquipmentData.ToolType.Ladybug);
                    break;

                case EquipmentData.ToolType.BotanicalPesticide:
                    ControlPests(EquipmentData.ToolType.BotanicalPesticide);
                    break;

                case EquipmentData.ToolType.BacillusThuringiensis:
                    ControlPests(EquipmentData.ToolType.BacillusThuringiensis);
                    break;

                case EquipmentData.ToolType.PH:
                    if (cropPlanted != null)
                    {
                        if (DatabaseBibit.Instance.CheckTreli(cropPlanted.seedToGrow))
                        {
                            UIManager.Instance.OpenUI(status.Water, status.Compost, activePest);
                        }
                        else
                        {
                            UIManager.Instance.OpenUI(status.Water, status.Compost, activePest);
                        }
                    }
                    else
                    {
                        UIManager.Instance.OpenUI(status.Water, status.Compost, activePest);
                    
                    }
                        
                    break;
            }
            //We don't need to check for seeds if we have already confirmed the tool to be a equipment
            return;
        }
        //Try asting the itemdata in the toolslot as SeedData
        SeedData seedTool = toolSlot as SeedData;
        
        ///Conditions for the player to able to plant a seed
        ///1: He is holding a tool of type SeedData
        ///2: The Land State must be either watered or farmland
        ///3. There isn't already a crop that has been planted
        if (seedTool != null && (landStatus == LandStatus.Curved || (landStatus == LandStatus.Compost && filter.mesh.name == "Cube.002 Instance"))&& cropPlanted == null)
        {
            //Instantiate the crop object parented to the land
            GameObject cropObject = Instantiate(cropPrefab, transform);

            //Access the CropBehaviour of the crop we're going to plant
            cropPlanted = cropObject.GetComponent<CropBehaviour>();

            //Plant it with the seed's information
            cropPlanted.Plant(seedTool, this);
            activePest = PestType.None;
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayPlantingSFX();
            }

            //Consume the item
            InventoryManager.Instance.ConsumeItem(InventoryManager.Instance.GetEquippedSlot(InventorySlot.InventoryType.Tool));
        }
    }

    private bool wasRainingLastTick = false;
    public void ClockUpdate(GameTimestamp timestamp)
    {
        timeWatered2 = timestamp;
        bool isRaining = WeatherManager.Instance != null && WeatherManager.Instance.IsRaining();

        // Detect the moment it starts raining
        if (isRaining && !wasRainingLastTick)
        {
            status.Water = true;
            SwitchLandStatus(LandStatus.Watered);
            timeWatered = TimeManager.Instance.GetGameTimestamp();

            if (cropPlanted != null && cropPlanted.seedToGrow != null)
            {
                QuestManager.Instance.ReportAction(QuestData.QuestType.Water, cropPlanted.seedToGrow.seedType);
            }

            //Debug.Log("Soil watered due to rain.");
        }

        // Detect the moment rain stops
        if (!isRaining && wasRainingLastTick)
        {
            status.Water = false;
            SwitchLandStatus(LandStatus.Soil);
            //Debug.Log("Rain stopped. Soil dried immediately.");
        }

        // Store the rain state for next tick
        wasRainingLastTick = isRaining;

        // Grow Logic
        if (landStatus == LandStatus.Watered || landStatus == LandStatus.Compost)
        {
            int hoursElapsed = GameTimestamp.CompareTimestamps(timeWatered, timestamp);
            if (hoursElapsed >= 1)
            {
                if (cropPlanted != null)
                {
                    TryApplyPestAttack(timestamp);

                    if (!HasPests)
                    {
                        cropPlanted.Grow();
                    }
                    else
                    {
                        Debug.Log("[PestControl] Crop growth paused because pests are attacking. Use pesticide to control pests.");
                    }
                }
                SwitchLandStatus(LandStatus.Soil);
                status.Water = false;
            }
        }

    }

    private void TryApplyPestAttack(GameTimestamp currentTimestamp)
    {
        if (cropPlanted == null || HasPests)
        {
            return;
        }

        float chance = dailyPestAttackChance;

        // Crop rotation with Pueraria/legume lowers daily pest chance.
        if (status.LandRotation)
        {
            chance *= cropRotationPestMultiplier;
        }

        if (Random.value <= chance)
        {
            activePest = RollPestType(currentTimestamp != null ? currentTimestamp.season : GameTimestamp.Season.Panas);
            Debug.Log($"[PestControl] {activePest} attacked this crop. Daily chance used: {chance:P0}");
        }
    }

    private PestType RollPestType(GameTimestamp.Season currentSeason)
    {
        List<PestType> pestPool = GetPestPoolForCurrentCrop();
        if (pestPool.Count == 0)
        {
            return PestType.None;
        }

        float totalWeight = 0f;
        List<float> weights = new List<float>(pestPool.Count);
        foreach (PestType pest in pestPool)
        {
            float weight = 1f;
            if (pest == PestType.PathogenicFungi)
            {
                if (currentSeason == GameTimestamp.Season.Hujan)
                {
                    weight *= rainySeasonFungiMultiplier;
                }
                else
                {
                    weight *= summerSeasonFungiMultiplier;
                }
            }

            weights.Add(weight);
            totalWeight += weight;
        }

        float roll = Random.value * totalWeight;
        for (int i = 0; i < pestPool.Count; i++)
        {
            if (roll < weights[i])
            {
                return pestPool[i];
            }

            roll -= weights[i];
        }

        return pestPool[pestPool.Count - 1];
    }

    private List<PestType> GetPestPoolForCurrentCrop()
    {
        List<PestType> pests = new List<PestType>();
        if (cropPlanted == null || cropPlanted.seedToGrow == null)
        {
            return pests;
        }

        // Jagung: ulat grayak, ulat tanah.
        if (IsCropMatch("jagung"))
        {
            pests.Add(PestType.Armyworm);
            pests.Add(PestType.Cutworm);
            return pests;
        }

        // Kubis: ulat kubis, kutu daun.
        if (IsCropMatch("kubis"))
        {
            pests.Add(PestType.CabbageWorm);
            pests.Add(PestType.Aphids);
            return pests;
        }

        // Kacang panjang: kutu daun, kutu laba-laba, ulat grayak.
        if (IsCropMatch("kacang panjang", "legume"))
        {
            pests.Add(PestType.Aphids);
            pests.Add(PestType.SpiderMites);
            pests.Add(PestType.Armyworm);
            return pests;
        }

        // Kentang: kutu daun, ulat tanah, kutu laba-laba, jamur patogen.
        if (IsCropMatch("kentang"))
        {
            pests.Add(PestType.Aphids);
            pests.Add(PestType.Cutworm);
            pests.Add(PestType.SpiderMites);
            pests.Add(PestType.PathogenicFungi);
            return pests;
        }

        // Pueraria/Puera: kutu daun.
        if (IsCropMatch("pueraria", "puera"))
        {
            pests.Add(PestType.Aphids);
            return pests;
        }

        // Fallback for unknown crops.
        pests.Add(PestType.PathogenicFungi);
        pests.Add(PestType.Aphids);
        pests.Add(PestType.Armyworm);
        return pests;
    }

    private bool IsCropMatch(params string[] keywords)
    {
        if (cropPlanted == null || cropPlanted.seedToGrow == null)
        {
            return false;
        }

        string seedType = cropPlanted.seedToGrow.seedType != null ? cropPlanted.seedToGrow.seedType.ToLowerInvariant() : string.Empty;
        string seedName = cropPlanted.seedToGrow.name != null ? cropPlanted.seedToGrow.name.ToLowerInvariant() : string.Empty;

        foreach (string keyword in keywords)
        {
            string key = keyword.ToLowerInvariant();
            if (seedType.Contains(key) || seedName.Contains(key))
            {
                return true;
            }
        }

        return false;
    }

    private void ControlPests(EquipmentData.ToolType toolType)
    {
        if (cropPlanted == null)
        {
            return;
        }

        if (!HasPests)
        {
            Debug.Log("[PestControl] No pests to control on this soil.");
            return;
        }

        if (toolType == EquipmentData.ToolType.Pesticide && activePest == PestType.PathogenicFungi)
        {
            activePest = PestType.None;
            return;
        }

        if ((toolType == EquipmentData.ToolType.BacillusThuringiensis || toolType == EquipmentData.ToolType.BotanicalPesticide) &&
            (activePest == PestType.Armyworm || activePest == PestType.Cutworm || activePest == PestType.CabbageWorm))
        {
            activePest = PestType.None;
            return;
        }

        if ((toolType == EquipmentData.ToolType.Ladybug || toolType == EquipmentData.ToolType.BotanicalPesticide) &&
            (activePest == PestType.Aphids || activePest == PestType.SpiderMites))
        {
            activePest = PestType.None;
            return;
        }

        if ((toolType == EquipmentData.ToolType.BotanicalPesticide || toolType == EquipmentData.ToolType.Ladybug) &&
            (activePest == PestType.Aphids || activePest == PestType.SpiderMites || activePest == PestType.Armyworm || activePest == PestType.Cutworm || activePest == PestType.CabbageWorm))
        {
            activePest = PestType.None;
            return;
        }

        Debug.Log($"[PestControl] {toolType} is not effective against {activePest}.");
    }

    
}
