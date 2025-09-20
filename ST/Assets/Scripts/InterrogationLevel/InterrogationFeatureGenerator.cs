using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum InterrogationFeatureType
{
    Hair,
    EyeColor,
    Extra,
}
public class InterrogationFeatureGenerator : MonoBehaviour
{
    [Header("Feature Prefab References")]
    [SerializeField] private GameObject[] haircuts;
    [SerializeField] private GameObject[] eyesColors;
    [SerializeField] private GameObject[] extras;

    public IReadOnlyList<GameObject> Haircuts => Array.AsReadOnly(haircuts);
    public IReadOnlyList<GameObject> EyesColors => Array.AsReadOnly(eyesColors);
    public IReadOnlyList<GameObject> Extras => Array.AsReadOnly(extras);

    public int[] currentHairIndexes { get; private set; }
    public int[] currentEyesColorsIndexes { get; private set; }
    public int[] currentExtrasIndexes { get; private set; }
    public const int amountOfIndexesToChoseFrom = 3;

    public int[] killerFeatureIndexes { get; private set; }
    public int[] firstTestimonyIndexes { get; private set; }
    public int[] secondTestimonyIndexes { get; private set; }
    public int[] thirdTestimonyIndexes { get; private set; }

    private const int HAIR_INDEX = 0;
    private const int EYE_COLOR_INDEX = 1;
    private const int EXTRA_FEATURE_INDEX = 2;
    private const int FEATURE_TYPE_COUNT = 3;

    public static InterrogationFeatureGenerator Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        if (haircuts == null || eyesColors == null || extras == null)
        {
            Debug.LogError("One or more feature arrays are null");
            enabled = false;
            return;
        }

        if (haircuts.Length == 0 || eyesColors.Length == 0 || extras.Length == 0)
        {
            Debug.LogError("One or more feature arrays are empty");
            enabled = false;
        }
    }

    public void GenerateRandomFeatureIndexes()
    {
        PickFeaturesIndexesToChoseFrom();
        GenerateKillerFeatureIndexes();
        GenerateTestimonyFeatureIndexes();
    }

    private void PickFeaturesIndexesToChoseFrom()
    {
        if (haircuts.Length < amountOfIndexesToChoseFrom ||
            eyesColors.Length < amountOfIndexesToChoseFrom ||
            extras.Length < amountOfIndexesToChoseFrom)
        {
            Debug.LogError("Not enough features in one or more arrays to pick unique indexes.");
            return;
        }
        currentHairIndexes = PickFeaturesIndexToChoseFrom(haircuts);
        currentEyesColorsIndexes = PickFeaturesIndexToChoseFrom(eyesColors);
        currentExtrasIndexes = PickFeaturesIndexToChoseFrom(extras);
    }

    private int[] PickFeaturesIndexToChoseFrom(GameObject[] feachuresList)
    {
        int[] currentFeatureIndexes = { -1, -1, -1 };
        int randomIndex;
        for (int i = 0; i < amountOfIndexesToChoseFrom; i++)
        {
            do
            {
                randomIndex = Random.Range(0, feachuresList.Length);
            }
            while (currentFeatureIndexes.Contains(randomIndex));

            currentFeatureIndexes[i] = randomIndex;
        }
        return currentFeatureIndexes;
    }

    private void GenerateKillerFeatureIndexes()
    {
        killerFeatureIndexes = new int[FEATURE_TYPE_COUNT];
        killerFeatureIndexes[HAIR_INDEX] = currentHairIndexes[Random.Range(0, amountOfIndexesToChoseFrom)];
        killerFeatureIndexes[EYE_COLOR_INDEX] = currentEyesColorsIndexes[Random.Range(0, amountOfIndexesToChoseFrom)];
        killerFeatureIndexes[EXTRA_FEATURE_INDEX] = currentExtrasIndexes[Random.Range(0, amountOfIndexesToChoseFrom)];
    }

    private void GenerateTestimonyFeatureIndexes()
    {
        int[][] testimonyFeatureIndexes = new int[3][];
        int[][] currentFeatureIndexes = new int[3][]
        {
            currentHairIndexes,
            currentEyesColorsIndexes,
            currentExtrasIndexes
        };
        for (int i = 0; i < testimonyFeatureIndexes.Length; i++)
        {
            testimonyFeatureIndexes[i] = (int[])killerFeatureIndexes.Clone();
        }

        int randomIndex;
        int pickedIndex;
        for (int i = 0; i < testimonyFeatureIndexes.Length; i++)
        {
            do
            {
                randomIndex = Random.Range(0, currentFeatureIndexes.Length);
                pickedIndex = currentFeatureIndexes[i][randomIndex];
            }
            while (pickedIndex == testimonyFeatureIndexes[i][i]);
            testimonyFeatureIndexes[i][i] = pickedIndex;
        }

        testimonyFeatureIndexes = testimonyFeatureIndexes.OrderBy(x => Random.value).ToArray();

        firstTestimonyIndexes = testimonyFeatureIndexes[0];
        secondTestimonyIndexes = testimonyFeatureIndexes[1];
        thirdTestimonyIndexes = testimonyFeatureIndexes[2];
    }
}
