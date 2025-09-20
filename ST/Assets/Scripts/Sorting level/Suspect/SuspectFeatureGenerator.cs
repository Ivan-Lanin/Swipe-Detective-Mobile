using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Handles the generation and management of suspect visual features
/// </summary>
public class SuspectFeatureGenerator : MonoBehaviour
{
    [Header("Feature Prefab References")]
    [SerializeField] private GameObject[] hairLengths;
    [SerializeField] private GameObject[] hairLongColors;
    [SerializeField] private GameObject[] hairShortColors;
    [SerializeField] private GameObject[] eyesColors;
    [SerializeField] private GameObject[] extras;

    private GameObject[] wigs;

    public int[] killerFeatureIndexes { get; private set; }

    public static SuspectFeatureGenerator Instance { get; private set; }

    private const float KILLER_HAIR_LENGTH_RATIO = 0.5f; // half of suspects share killer's hair length
    private const float KILLER_HAIR_COLOR_RATIO = 0.25f; // quarter of suspects share killer's hair color
    private const float KILLER_EYE_COLOR_RATIO = 0.125f; // eighth of suspects share killer's eye color

    // Feature type indices
    private const int HAIR_LENGTH_INDEX = 0;
    private const int HAIR_COLOR_INDEX = 1;
    private const int EYE_COLOR_INDEX = 2;
    private const int EXTRA_FEATURE_INDEX = 3;
    private const int FEATURE_TYPE_COUNT = 4;

    // Hair length indices
    private const int HAIR_LENGTH_BOLD = -1;
    private const int HAIR_LENGTH_LONG = 0;
    private const int HAIR_LENGTH_SHORT = 1;

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

        if (hairLengths.Length == 0 || hairLongColors.Length == 0 || hairShortColors.Length == 0 || eyesColors.Length == 0 || extras.Length == 0)
        {
            throw new System.Exception("At least one of the feature arrays is empty");
        }

        wigs = hairLongColors.Concat(hairShortColors).ToArray();
    }

    /// <summary>
    /// Destroys all features attached to a suspect
    /// </summary>
    public void DestroyFeatures(SuspectFeatures suspect)
    {
        foreach (var feature in suspect.features)
        {
            Destroy(feature.Value);
        }
    }

    private int[][] GenerateRandomFeatureIndexesForSuspects(int count)
    {
        int[][] featureIndexes = new int[FEATURE_TYPE_COUNT][];

        // Hair length
        int[] hairLengthIndexes = new int[count];
        // The first suspect is the killer
        hairLengthIndexes[0] = GenerateRandomHairLengthIndex();
        // The first half of the suspects will have the same hair length to get to the second round of the game
        for (int i = 1; i < count * KILLER_HAIR_LENGTH_RATIO; i++)
        {
            hairLengthIndexes[i] = hairLengthIndexes[0];
        }
        // The other half will have random hair length
        for (int i = (int)(count * KILLER_HAIR_LENGTH_RATIO); i < count; i++)
        {
            hairLengthIndexes[i] = GenerateRandomHairLengthIndex();
            while (hairLengthIndexes[i] == hairLengthIndexes[0])
            {
                hairLengthIndexes[i] = GenerateRandomHairLengthIndex();
            }
        }
        featureIndexes[HAIR_LENGTH_INDEX] = hairLengthIndexes;

        // Hair color
        int[] hairColorIndexes = new int[count];
        hairColorIndexes[0] = GenerateRandomHairColorIndex(hairLengthIndexes[0]);
        for (int i = 1; i < count * KILLER_HAIR_COLOR_RATIO; i++)
        {
            hairColorIndexes[i] = hairColorIndexes[0];
        }

        for (int i = (int)(count * KILLER_HAIR_COLOR_RATIO); i < count; i++)
        {
            hairColorIndexes[i] = GenerateRandomHairColorIndex(hairLengthIndexes[i]);
            int attempts = 0;
            int maxAttempts = 100;
            while (hairColorIndexes[i] == hairColorIndexes[0] && attempts < maxAttempts)
            {
                hairColorIndexes[i] = GenerateRandomHairColorIndex(hairLengthIndexes[i]);
                attempts++;
            }
            if (attempts == maxAttempts)
            {
                throw new System.Exception("Max attempts reached while generating hair color index");
            }
        }
        featureIndexes[HAIR_COLOR_INDEX] = hairColorIndexes;

        // Eyes color
        int[] eyesIndexes = new int[count];
        eyesIndexes[0] = GenerateRandomEyesIndex();
        for (int i = 1; i < count * KILLER_EYE_COLOR_RATIO; i++)
        {
            eyesIndexes[i] = eyesIndexes[0];
        }
        for (int i = (int)(count * KILLER_EYE_COLOR_RATIO); i < count; i++)
        {
            eyesIndexes[i] = GenerateRandomEyesIndex();
            while (eyesIndexes[i] == eyesIndexes[0])
            {
                eyesIndexes[i] = GenerateRandomEyesIndex();
            }
        }
        featureIndexes[EYE_COLOR_INDEX] = eyesIndexes;

        // Extra features
        int[] extraIndexes = new int[count];
        extraIndexes[0] = GenerateRandomExtraIndex();
        for (int i = 1; i < count; i++)
        {
            extraIndexes[i] = GenerateRandomExtraIndex();
        }
        while (extraIndexes[0] == extraIndexes[1])
        {
            extraIndexes[1] = GenerateRandomExtraIndex();
        }
        featureIndexes[EXTRA_FEATURE_INDEX] = extraIndexes;

        killerFeatureIndexes = new int[4] { hairLengthIndexes[0], hairColorIndexes[0], eyesIndexes[0], extraIndexes[0] };

        return featureIndexes;
    }

    /// <summary>
    /// Creates random features for a list of suspects, with the first being the killer
    /// </summary>
    public void InstantiateRandomFeatures(List<GameObject> suspects)
    {
        int[][] featureIndexes = GenerateRandomFeatureIndexesForSuspects(suspects.Count);

        for (int i = 0; i < suspects.Count; i++)
        {
            InstantiateFeatures(suspects[i], featureIndexes, i);
        }
    }

    /// <summary>
    /// Instantiates features on a suspect based on feature indices
    /// </summary>
    private void InstantiateFeatures(GameObject suspect, int[][] featureIndexes, int suspectIndex)
    {
        // Hair length and color
        if (featureIndexes[HAIR_LENGTH_INDEX][suspectIndex] == HAIR_LENGTH_LONG)
        {
            Instantiate(hairLongColors[featureIndexes[1][suspectIndex]], suspect.transform);
        }
        else if (featureIndexes[HAIR_LENGTH_INDEX][suspectIndex] == HAIR_LENGTH_SHORT)
        {
            Instantiate(hairShortColors[featureIndexes[1][suspectIndex]], suspect.transform);
        }
        else if (featureIndexes[HAIR_LENGTH_INDEX][suspectIndex] == HAIR_LENGTH_BOLD && featureIndexes[HAIR_COLOR_INDEX][suspectIndex] >= 0)
        {
            GameObject thisWig = Instantiate(wigs[featureIndexes[HAIR_COLOR_INDEX][suspectIndex]], suspect.transform);
            thisWig.name += " (Wig)";
        }

        // Eye color
        Instantiate(eyesColors[featureIndexes[EYE_COLOR_INDEX][suspectIndex]], suspect.transform);

        // Extra
        if (featureIndexes[EXTRA_FEATURE_INDEX][suspectIndex] != -1)
        {
            Instantiate(extras[featureIndexes[EXTRA_FEATURE_INDEX][suspectIndex]], suspect.transform);
        }
    }

    /// <summary>
    /// Creates features for a dummy with killer's feature indices
    /// </summary>
    public SuspectFeatures InstantiateDummyFeatures(GameObject suspect, int[] featureIndexes) 
    {
        SuspectFeatures features = ScriptableObject.CreateInstance<SuspectFeatures>();

        // Hair length
        if (featureIndexes[HAIR_LENGTH_INDEX] >= 0)
        {
            GameObject hairLength = Instantiate(hairLengths[featureIndexes[HAIR_LENGTH_INDEX]], suspect.transform);
            features.features.Add("hairLength", hairLength);
            features.hairLength = hairLength;
        }
        else
        {
            features.features.Add("hairLength", null);
        }

        // Hair color
        if (featureIndexes[HAIR_LENGTH_INDEX] == HAIR_LENGTH_LONG)
        {
            GameObject hairColor = Instantiate(hairLongColors[featureIndexes[1]], suspect.transform);
            features.features.Add("hairColor", hairColor);
            features.hairColor = hairColor;
        }
        else if (featureIndexes[HAIR_LENGTH_INDEX] == HAIR_LENGTH_SHORT)
        {
            GameObject hairColor = Instantiate(hairShortColors[featureIndexes[1]], suspect.transform);
            features.features.Add("hairColor", hairColor);
            features.hairColor = hairColor;
        }
        else if (featureIndexes[HAIR_LENGTH_INDEX] == HAIR_LENGTH_BOLD && featureIndexes[HAIR_COLOR_INDEX] >= 0)
        {
            GameObject hairColor = Instantiate(wigs[featureIndexes[HAIR_COLOR_INDEX]], suspect.transform);
            hairColor.name += " (Wig)";
            features.features.Add("hairColor", hairColor);
            features.hairColor = hairColor;
        }
        else
        {
            features.features.Add("hairColor", null);
        }

        // Eyes color
        GameObject eyes = Instantiate(eyesColors[featureIndexes[EYE_COLOR_INDEX]], suspect.transform);
        features.features.Add("eyes", eyes);
        features.eyes = eyes;

        // Extra
        if (featureIndexes[3] != -1)
        {
            GameObject extra = Instantiate(extras[featureIndexes[EXTRA_FEATURE_INDEX]], suspect.transform);
            features.features.Add("extra", extra);
            features.extra = extra;
        }
        else
        {
            features.features.Add("extra", null);
        }

        return features;
    }

    public Dictionary<string, int> GetPrisonerFeatureIndexes()
    {
        Dictionary<string, int> prisonerFeatures = new Dictionary<string, int>();
        prisonerFeatures.Add("hairLength", killerFeatureIndexes[HAIR_LENGTH_INDEX]);
        prisonerFeatures.Add("hairColor", killerFeatureIndexes[HAIR_COLOR_INDEX]);
        prisonerFeatures.Add("extra", killerFeatureIndexes[EXTRA_FEATURE_INDEX]);
        return prisonerFeatures;
    }

    private int GenerateRandomHairLengthIndex()
    {

        if (Random.Range(0, 3) == 0)
        {
            return HAIR_LENGTH_BOLD;
        }
        else
        {
            return Random.Range(0, hairLengths.Length);
        }
    }

    private int GenerateRandomHairColorIndex(int hairLengthIndex)
    {
        if (hairLengthIndex == HAIR_LENGTH_BOLD)
        {
            int baldType = Random.Range(0, 2);
            if (baldType == 0)
            {
                return HAIR_LENGTH_BOLD;
            }
            else
            {
                return Random.Range(0, wigs.Length);
            }
        }
        else if (hairLengthIndex == HAIR_LENGTH_LONG)
        {
            return Random.Range(0, hairLongColors.Length);
        }
        else if (hairLengthIndex == HAIR_LENGTH_SHORT)
        {
            return Random.Range(0, hairShortColors.Length);
        }
        else
        {
            throw new System.Exception("Invalid hair length index");
        }
    }

    private int GenerateRandomEyesIndex()
    {
        return Random.Range(0, eyesColors.Length);
    }

    private int GenerateRandomExtraIndex()
    {
        if (Random.Range(0, 4) == 0)
        {
            return -1;
        }
        return Random.Range(0, extras.Length);
    }
}
