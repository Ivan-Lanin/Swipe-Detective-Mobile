using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PrisonerSpawner : MonoBehaviour
{
    [Header("Feature Prefab References")]
    [SerializeField] private GameObject[] hairLongColors;
    [SerializeField] private GameObject[] hairShortColors;
    [SerializeField] private GameObject[] extras;

    [SerializeField] private GameObject prisonerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private PrisonManager prisonManager;

    // Hair length indices
    private const int HAIR_LENGTH_BOLD = -1;
    private const int HAIR_LENGTH_LONG = 0;
    private const int HAIR_LENGTH_SHORT = 1;

    private Dictionary<string, int> prisonerFeatures;

    private void Start()
    {
        if (NewPrisonerTrigger.Instance != null)
        {
            prisonerFeatures = NewPrisonerTrigger.Instance.DisposableTransitionData;
            SpawnNewPrisoner(prisonerFeatures);
        }

    }

    public void SpawnPrisonersFromData(Dictionary<string, int>[] prisoners)
    {
        foreach (Dictionary<string, int> prisoner in prisoners)
        {
            if (prisoner != null) SpawnPrisoner(prisoner);
        }
    }

    public void SpawnNewPrisoner(Dictionary<string, int> features)
    {
        GameObject newPrisoner = SpawnPrisoner(features);
        prisonManager.AddNewPrisoner(features);
        StartCoroutine(newPrisoner.GetComponent<Prisoner>().StartSpawnAnimation());
    }

    private GameObject SpawnPrisoner(Dictionary<string, int> features)
    {
        GameObject spawnedPrisoner = Instantiate(prisonerPrefab, spawnPoint.position, spawnPoint.rotation);
        spawnedPrisoner.transform.SetParent(spawnPoint);

        if (features["hairLength"] == HAIR_LENGTH_LONG)
        {
            SpawnBodyPart(hairLongColors[features["hairColor"]], spawnedPrisoner);
        }
        else if (features["hairLength"] == HAIR_LENGTH_SHORT)
        {
            SpawnBodyPart(hairShortColors[features["hairColor"]], spawnedPrisoner);
        }
        else if (features["hairLength"] == HAIR_LENGTH_BOLD)
        {
            // Bold is bold. Wigs are not allowed in this prison
        }

        if (features["extra"] != -1)
        {
            SpawnBodyPart(extras[features["extra"]], spawnedPrisoner);
        }

        return spawnedPrisoner;
    }

    private void SpawnBodyPart(GameObject bodyPart, GameObject prisoner)
    {
        Prisoner prisonerComponent = prisoner.GetComponent<Prisoner>();
        GameObject spawnedBodyPart = Instantiate(bodyPart, prisonerComponent.bodyPartsParentObject.transform);
        prisonerComponent.AddBodyPart(spawnedBodyPart);
    }
}
