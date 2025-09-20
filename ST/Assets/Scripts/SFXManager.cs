using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    private Dictionary<SFXType, float> volumeMultipliers = new Dictionary<SFXType, float>
    {
        { SFXType.Construction, 0f },
        { SFXType.Coins, 0f },
        { SFXType.Tutorial, 0f },
        { SFXType.PrisonerSpawn, 0f },
        { SFXType.SuspectSorting, 0f },
        { SFXType.SortingManager, 0f }
    };

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        CheckVolumeMultipliersImplementation();
    }

    private void CheckVolumeMultipliersImplementation()
    {
        // Ensure volumeMultipliers contains all SFXType values
        foreach (SFXType sfxType in System.Enum.GetValues(typeof(SFXType)))
        {
            if (!volumeMultipliers.ContainsKey(sfxType))
            {
                Debug.LogError($"SFXType {sfxType} is not implemented in volumeMultipliers.");
                return;
            }
        }
    }

    public void PlaySound(AudioSource source, SFXType sfxType)
    {
        float volumeMultiplier = 1f;
        if (volumeMultipliers[sfxType] == 0f) volumeMultipliers[sfxType] = source.volume;
        volumeMultiplier = volumeMultipliers[sfxType];

        source.volume = DataManager.Instance.GameData.sfxVolume * volumeMultiplier;
        source.Play();
    }
}

public enum SFXType
{
    Construction,
    Coins,
    Tutorial,
    PrisonerSpawn,
    SuspectSorting,
    SortingManager
}
