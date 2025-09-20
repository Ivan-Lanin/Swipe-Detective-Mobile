using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SuspectFeatureRenderer : MonoBehaviour
{
    [SerializeField] private GameObject killerDummy;
    [SerializeField] private TMP_Text textFeatureType;
    [SerializeField] private TMP_Text textFeatureName;
    [SerializeField] private Image featureIcon;
    [SerializeField] private RawImage profileIcon;
    [SerializeField] private Sprite[] featureIcons;
    [SerializeField] private GameObject hintArrow;
    [SerializeField] private Transform[] hintArrowAnchors;

    private SuspectFeatureGenerator generator;
    private SuspectFeatures featuresOfTheKiller;

    private enum FeatureType
    {
        HairLength = 0,
        EyeColor = 1,
        HairColor = 2,
        Extra = 3
    }

    void Start()
    {
        generator = SuspectFeatureGenerator.Instance;
        StartCoroutine(WaitForGeneration());
    }

    public void RenderFeatureHint(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Round1:
                RenderHairLength();
                break;
            case GameState.Round2:
                RenderHairColor();
                break;
            case GameState.Round3:
                RenderPupils();
                break;
            case GameState.Round4:
                RenderExtra();
                break;
        }
    }

    private void RenderHairLength()
    {
        PrepareFeatureRendering(FeatureType.HairLength, "Hair Length");

        if (featuresOfTheKiller.hairLength != null)
        {
            featuresOfTheKiller.hairLength.SetActive(true);
            textFeatureName.text = GetHairLengthName(featuresOfTheKiller.hairLength.name);
        }
        else
        {
            textFeatureName.text = "Bald";
        }
    }

    private void RenderPupils()
    {
        PrepareFeatureRendering(FeatureType.EyeColor, "Eye Color");

        featuresOfTheKiller.eyes.SetActive(true);

        string pupilColor = featuresOfTheKiller.eyes.name
            .Replace("Pupils_", "")
            .Replace("(Clone)", "");
        textFeatureName.text = pupilColor;
    }

    private void RenderHairColor()
    {
        PrepareFeatureRendering(FeatureType.HairColor, "Hair Color");

        if (featuresOfTheKiller.hairColor != null)
        {
            featuresOfTheKiller.hairColor.SetActive(true);
            textFeatureName.text = GetHairColorName(featuresOfTheKiller.hairColor.name);
        }
        else
        {
            textFeatureName.text = "Bald (No wig)";
        }
    }

    private void RenderExtra()
    {
        PrepareFeatureRendering(FeatureType.Extra, "Extra");

        if (featuresOfTheKiller.extra != null)
        {
            featuresOfTheKiller.extra.SetActive(true);
            textFeatureName.text = featuresOfTheKiller.extra.name
                .Replace("(Clone)", "")
                .Replace("Extra_", "")
                .Replace("_", " ");
        }
        else
        {
            textFeatureName.text = "No extra features";
        }
    }

    private void PrepareFeatureRendering(FeatureType featureType, string featureTypeName)
    {
        textFeatureName.enabled = true;
        textFeatureType.enabled = true;
        featureIcon.enabled = true;

        HideAllFeatures();

        textFeatureType.text = featureTypeName;
        featureIcon.sprite = featureIcons[(int)featureType];
        hintArrow.transform.position = hintArrowAnchors[(int)featureType].position;
    }

    private string GetHairLengthName(string hairLengthName)
    {
        if (hairLengthName.Contains("Long")) return "Long";
        if (hairLengthName.Contains("Short")) return "Short";
        return "Bald";
    }

    private string GetHairColorName(string hairColorName)
    {
        string color = hairColorName.Contains("Blond") ? "Blond" :
                       hairColorName.Contains("Black") ? "Black" :
                       hairColorName.Contains("Red") ? "Red" : "Unknown";

        if (hairColorName.Contains("Wig"))
        {
            return $"{color} wig";
        }

        return color;
    }

    private void HideAllFeatures()
    {
        SetFeatureActive(featuresOfTheKiller.hairLength, false);
        SetFeatureActive(featuresOfTheKiller.hairColor, false);
        SetFeatureActive(featuresOfTheKiller.eyes, false);
        SetFeatureActive(featuresOfTheKiller.extra, false);
    }

    private void SetFeatureActive(GameObject feature, bool isActive)
    {
        if (feature != null)
        {
            feature.SetActive(isActive);
        }
    }

    private IEnumerator WaitForGeneration()
    {
        while (generator.killerFeatureIndexes == null)
        {
            yield return null;
        }
        featuresOfTheKiller = generator.InstantiateDummyFeatures(killerDummy, generator.killerFeatureIndexes);
    }
}
