using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Prisoner : MonoBehaviour
{
    [SerializeField] private GameObject[] initialBodyParts;
    [SerializeField] private ParticleSystem spawnVFX;
    [SerializeField] private AudioSource audioSource;

    private List<GameObject> bodyParts;
    private PrisonerWandering prisonerWanderingComponent;
    private float VFXCoverTime = 0.3f;

    public GameObject bodyPartsParentObject;

    private void Awake()
    {
        bodyParts = new List<GameObject>(initialBodyParts);
        prisonerWanderingComponent = GetComponent<PrisonerWandering>();
        audioSource = GetComponent<AudioSource>();
    }

    public void AddBodyPart(GameObject bodyPart)
    {
        bodyParts.Add(bodyPart);
    }

    public IEnumerator StartSpawnAnimation()
    {
        prisonerWanderingComponent.enabled = false;
        foreach (GameObject bodyPart in bodyParts)
        {
            bodyPart.SetActive(false);
        }

        while (DepartmentCameraAnimator.Instance.isCameraMoving)
        {
            yield return null;
        }

        spawnVFX.Play();
        SFXManager.Instance.PlaySound(audioSource, SFXType.PrisonerSpawn);
        yield return new WaitForSeconds(VFXCoverTime);

        prisonerWanderingComponent.enabled = true;
        foreach (GameObject bodyPart in bodyParts)
        {
            bodyPart.SetActive(true);
        }
    }
}
