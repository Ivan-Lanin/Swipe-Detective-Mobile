using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Handles suspect feature comparison and analysis logic
/// </summary>
public class SuspectFeatureService : MonoBehaviour
{
    [SerializeField] private SuspectFeatureGenerator featureGenerator;

    public void InstantiateRandomFeatures(List<GameObject> suspects)
    {
        featureGenerator.InstantiateRandomFeatures(suspects);
    }

    public (string, string) GetHairLengths(GameObject currentSuspect, GameObject referenceSuspect)
    {
        (Transform objectToCheck, Transform referenceObject) = GetTransforms("Hair", currentSuspect, referenceSuspect);
        string objectToCheckName = GetHairLength(objectToCheck);
        string referenceObjectName = GetHairLength(referenceObject);
        return (objectToCheckName, referenceObjectName);
    }

    public (string, string) GetHairColors(GameObject currentSuspect, GameObject referenceSuspect)
    {
        (Transform objectToCheck, Transform referenceObject) = GetTransforms("Hair", currentSuspect, referenceSuspect);
        string objectToCheckName = GetHairColor(objectToCheck);
        string referenceObjectName = GetHairColor(referenceObject);
        return (objectToCheckName, referenceObjectName);
    }

    public (string, string) GetEyeColors(GameObject currentSuspect, GameObject referenceSuspect)
    {
        (Transform objectToCheck, Transform referenceObject) = GetTransforms("Pupils", currentSuspect, referenceSuspect);
        string objectToCheckName = GetEyeColor(objectToCheck);
        string referenceObjectName = GetEyeColor(referenceObject);
        return (objectToCheckName, referenceObjectName);
    }

    public (string, string) GetExtras(GameObject currentSuspect, GameObject referenceSuspect)
    {
        (Transform objectToCheck, Transform referenceObject) = GetTransforms("Extra", currentSuspect, referenceSuspect);
        string objectToCheckName = GetExtra(objectToCheck);
        string referenceObjectName = GetExtra(referenceObject);
        return (objectToCheckName, referenceObjectName);
    }

    private string GetHairLength(Transform hairObject)
    {
        if (hairObject == null)
        {
            return "no hair";
        }
        else if (hairObject.name.Contains("Wig"))
        {
            return "no hair (" + hairObject.name.Split('_')[1].ToLower() + " wig)";
        }
        else
        {
            return hairObject.name.Split('_')[1].ToLower() + " hair";
        }
    }

    private string GetHairColor(Transform hairObject)
    {
        if (hairObject == null)
        {
            return "Bald";
        }

        string[] nameParts = hairObject.name.Split('_');
        if (nameParts.Length < 3)
        {
            Debug.LogWarning($"Invalid hair object name format: {hairObject.name}");
            return "Unknown";
        }

        string hairColor = nameParts[2];

        hairColor = hairColor
            .Replace("(Clone)", "")
            .Replace("(Wig)", "")
            .ToLower();

        if (hairObject.name.Contains("Wig"))
        {
            hairColor = hairObject.name.Split('_')[1] + hairColor + " wig";
        }
        else
        {
            hairColor += " hair";
        }
        return hairColor;
    }

    private string GetEyeColor(Transform eyeObject)
    {
        return eyeObject.name.Split('_')[1].Replace("(Clone)", "").ToLower();
    }

    private string GetExtra(Transform extraObject)
    {
        if (extraObject == null)
        {
            return "no extra";
        }
        else
        {
            return extraObject.name.Replace("Extra_", "").Replace("_", " ").Replace("(Clone)", "").ToLower();
        }
    }

    private (Transform, Transform) GetTransforms(string name, GameObject currentSuspect, GameObject referenceSuspect)
    {
        List<Transform> objectToCheckList = new List<Transform>();
        List<Transform> referenceObjectList = new List<Transform>();
        currentSuspect.GetComponentsInChildren(objectToCheckList);
        referenceSuspect.GetComponentsInChildren(referenceObjectList);
        Transform objectToCheck = objectToCheckList.FirstOrDefault(c => c.name.Contains(name));
        Transform referenceObject = referenceObjectList.FirstOrDefault(c => c.name.Contains(name));
        return (objectToCheck, referenceObject);
    }
}