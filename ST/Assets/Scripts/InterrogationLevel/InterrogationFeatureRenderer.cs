using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InterrogationFeatureRenderer : MonoBehaviour
{
    [SerializeField] private Transform killerDummy;
    [SerializeField] private Transform featuresPosition;
    [SerializeField] private Camera featuresCamera;

    private InterrogationFeatureGenerator generator;
    private List<GameObject> currentHaircuts = new List<GameObject>();
    private List<GameObject> currentEyesColors = new List<GameObject>();
    private List<GameObject> currentExtras = new List<GameObject>();
    private List<GameObject> killerFeatures = new List<GameObject>();
    private List<GameObject> featuresOnTablet;

    private int displayedFeatureIndex = 0;
    private int interrogationFeatureTypeCount;

    public GameObject DisplayedFeature { get; private set; }
    public InterrogationFeatureType DisplayedFeatureType { get; private set; } = InterrogationFeatureType.Hair;
    public List<GameObject> FeaturesOnTablet => featuresOnTablet;

    private int GetFeatureTypeIndex() => (int)DisplayedFeatureType;

    public static InterrogationFeatureRenderer Instance { get; private set; }

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

        featuresCamera.enabled = false;
        interrogationFeatureTypeCount = System.Enum.GetValues(typeof(InterrogationFeatureType)).Length;
    }

    private void Start()
    {
        InterrogationControls.Instance.onNextFeature.AddListener(ShowNextFeature);
        InterrogationControls.Instance.onPreviousFeature.AddListener(ShowPreviousFeature);
        InterrogationControls.Instance.onNextFeatureType.AddListener(NextFeatureType);
        InformantsManager.Instance.onNextInformantReady.AddListener(RenderTextureAndReset);
        InformantsManager.Instance.decisionPanelReady.AddListener(RenderTextureAndReset);

        StartCoroutine(WaitForGeneratorReady());
    }

    private void ShowNextFeature()
    {
        ShowFeature(1);
    }

    private void ShowPreviousFeature()
    {
        ShowFeature(-1);
    }

    private void ShowFeature(int direction)
    {
        List<GameObject> featureList = GetCurrentFeatureList();
        int count = featureList.Count;
        displayedFeatureIndex = (displayedFeatureIndex + direction + count) % count;
        SwapDummyFeatures(featureList[displayedFeatureIndex]);
        featuresOnTablet[GetFeatureTypeIndex()] = DisplayedFeature;
        FeaturesCameraManager.Instance.RenderTextureForUI();
    }

    private void NextFeatureType()
    {
        featuresOnTablet[GetFeatureTypeIndex()] = DisplayedFeature;
        DisplayedFeatureType = (InterrogationFeatureType)((GetFeatureTypeIndex() + 1) % interrogationFeatureTypeCount);
        displayedFeatureIndex = 0;

        List<GameObject> featureList = GetCurrentFeatureList();
        if (featuresOnTablet[GetFeatureTypeIndex()] != null)
        {
            SwapDummyFeatures(featureList[displayedFeatureIndex], featuresOnTablet[GetFeatureTypeIndex()]);
        }
        else
        {
            AddDummyFeature(featureList[displayedFeatureIndex]);
        }

        featuresOnTablet[GetFeatureTypeIndex()] = DisplayedFeature;
        FeaturesCameraManager.Instance.RenderTextureForUI();

        if (InformantsManager.Instance.currentInformantIndex >= InformantsManager.Instance.Informants.Length) return;
        if (IsFeaturesOnTabletSet()) InterrogationControls.Instance.SetNextInformantButtonActive(true);
    }

    private List<GameObject> GetCurrentFeatureList()
    {
        return DisplayedFeatureType switch
        {
            InterrogationFeatureType.Hair => currentHaircuts,
            InterrogationFeatureType.EyeColor => currentEyesColors,
            InterrogationFeatureType.Extra => currentExtras,
            _ => null
        };
    }

    private void SwapDummyFeatures(GameObject newFeature)
    {
        DisplayedFeature.transform.position = featuresPosition.position;
        newFeature.transform.position = killerDummy.position;
        DisplayedFeature = newFeature;
    }

    private void SwapDummyFeatures(GameObject newFeature, GameObject currentFeature)
    {
        currentFeature.transform.position = featuresPosition.position;
        newFeature.transform.position = killerDummy.position;
        DisplayedFeature = newFeature;
    }

    private void AddDummyFeature(GameObject newFeature)
    {
        newFeature.transform.position = killerDummy.position;
        DisplayedFeature = newFeature;
    }

    private void RenderTextureAndReset(int informantIndex)
    {
        FeaturesCameraManager.Instance.RenderTextureForTestimony(informantIndex);
        ResetCurrentFeaturesPosition();
        SetFeaturesForRenderer();
    }

    private bool IsFeaturesOnTabletSet()
    {
        if (featuresOnTablet.Any(f => f == null)) return false;
        return true;
    }

    private void SetFeaturesForRenderer()
    {
        featuresOnTablet = new List<GameObject>(new GameObject[interrogationFeatureTypeCount]);
        DisplayedFeatureType = InterrogationFeatureType.Hair;
        displayedFeatureIndex = 0;

        if (currentHaircuts.Count == 0) InstantiateCurrentFeatures();

        DisplayedFeature = currentHaircuts[0];
        FeaturesCameraManager.Instance.RenderTextureForUI();
    }

    private void InstantiateCurrentFeatures()
    {
        GameObject currentFeature;
        foreach (int index in generator.currentHairIndexes)
        {
            currentFeature = Instantiate(generator.Haircuts[index], featuresPosition);
            currentHaircuts.Add(currentFeature);
            if (index == generator.killerFeatureIndexes[0])
            {
                killerFeatures.Add(currentFeature);
            }
        }
        foreach (int index in generator.currentEyesColorsIndexes)
        {
            currentFeature = Instantiate(generator.EyesColors[index], featuresPosition);
            currentEyesColors.Add(currentFeature);
            if (index == generator.killerFeatureIndexes[1])
            {
                killerFeatures.Add(currentFeature);
            }
        }
        foreach (int index in generator.currentExtrasIndexes)
        {
            currentFeature = Instantiate(generator.Extras[index], featuresPosition);
            currentExtras.Add(currentFeature);
            if (index == generator.killerFeatureIndexes[2])
            {
                killerFeatures.Add(currentFeature);
            }
        }
    }

    private void ResetCurrentFeaturesPosition()
    {
        foreach (GameObject haircut in currentHaircuts)
        {
            haircut.transform.position = featuresPosition.position;
        }
        foreach (GameObject eyeColor in currentEyesColors)
        {
            eyeColor.transform.position = featuresPosition.position;
        }
        foreach (GameObject extra in currentExtras)
        {
            extra.transform.position = featuresPosition.position;
        }
    }

    private IEnumerator WaitForGeneratorReady()
    {
        while (InterrogationFeatureGenerator.Instance == null)
        {
            yield return null;
        }

        generator = InterrogationFeatureGenerator.Instance;

        generator.GenerateRandomFeatureIndexes();

        while (generator.currentExtrasIndexes == null)
        {
            yield return null;
        }

        SetFeaturesForRenderer();
    }

    private void OnDestroy()
    {
        InterrogationControls.Instance.onNextFeature.RemoveListener(ShowNextFeature);
        InterrogationControls.Instance.onPreviousFeature.AddListener(ShowPreviousFeature);
        InterrogationControls.Instance.onNextFeatureType.AddListener(NextFeatureType);
        InformantsManager.Instance.onNextInformantReady.AddListener(RenderTextureAndReset);
        InformantsManager.Instance.decisionPanelReady.AddListener(RenderTextureAndReset);
    }
}
