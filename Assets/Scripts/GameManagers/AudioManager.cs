using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("BGM")]
    public AudioClip farmBgm;

    [Header("SFX")]
    public AudioClip shovelSfx;
    public AudioClip wateringCanSfx;
    public AudioClip compostSfx;
    public AudioClip pesticideSfx;
    public AudioClip plantingSfx;
    public AudioClip pickupSfx;
    public AudioClip shopClickSfx;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayBGM(farmBgm);
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null || bgmSource == null) return;

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;

        sfxSource.PlayOneShot(clip);
    }

    public void PlayToolSFX(EquipmentData.ToolType toolType)
    {
        switch (toolType)
        {
            case EquipmentData.ToolType.Shovel:
                PlaySFX(shovelSfx);
                break;

            case EquipmentData.ToolType.WateringCan:
                PlaySFX(wateringCanSfx);
                break;

            case EquipmentData.ToolType.Compost:
                PlaySFX(compostSfx);
                break;

            case EquipmentData.ToolType.Pesticide:
            case EquipmentData.ToolType.BotanicalPesticide:
            case EquipmentData.ToolType.BacillusThuringiensis:
                PlaySFX(pesticideSfx);
                break;
        }
    }

    public void PlayPickupSFX()
    {
        PlaySFX(pickupSfx);
    }

    public void PlayPlantingSFX()
    {
        PlaySFX(plantingSfx);
    }

    public void PlayShopClickSFX()
    {
        PlaySFX(shopClickSfx);
    }
}
