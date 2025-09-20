using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RandomEventCrime : MonoBehaviour
{
    [SerializeField] Button arrestCriminalButton;
    [SerializeField] ParticleSystem[] spawnVFXs;
    [SerializeField] GameObject criminal;
    [SerializeField] GameObject victim;

    private PrisonerSpawner prisonerSpawner;
    public UnityEvent onCrimeSpawned;
    public UnityEvent onCrimeDestroyed;

    private void OnEnable()
    {
        prisonerSpawner = FindFirstObjectByType<PrisonerSpawner>();

        arrestCriminalButton.onClick.AddListener(ArrestCriminal);
        onCrimeDestroyed.AddListener(RandomEventManager.Instance.OnRandomEventDestroyed);

        foreach (var spawnVFX in spawnVFXs)
        {
            spawnVFX.Play();
        }

        onCrimeSpawned.Invoke();
    }

    private void ArrestCriminal()
    {
        foreach (var spawnVFX in spawnVFXs)
        {
            spawnVFX.Play();
        }
        criminal.SetActive(false);
        victim.SetActive(false);
        arrestCriminalButton.interactable = false;

        DepartmentCameraAnimator.Instance.MoveToPrisonView();

        StartCoroutine(DestroyAfterVFX());

        AddPrisoner();
    }

    private void AddPrisoner()
    {
        int prisonersCount = DataManager.Instance.GetPrisonersCount();
        if (prisonersCount == 4) return;
        Dictionary<string, int> newPrisoner = new Dictionary<string, int>
        {
            { "hairLength", -1 },
            { "hairColor", -1 },
            { "extra", 0 }
        };
        prisonerSpawner.SpawnNewPrisoner(newPrisoner);
    }

    private IEnumerator DestroyAfterVFX()
    {
        while (!spawnVFXs[1].isStopped)
        {
            yield return null;
        }
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (RandomEventManager.Instance != null && RandomEventManager.Instance.gameObject.activeInHierarchy)
        {
            onCrimeDestroyed.Invoke();
        }
        arrestCriminalButton.onClick.RemoveListener(ArrestCriminal);
        onCrimeDestroyed.RemoveListener(RandomEventManager.Instance.OnRandomEventDestroyed);
    }
}
