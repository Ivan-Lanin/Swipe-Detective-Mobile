using UnityEngine;

public class SuspectAudioHandler : MonoBehaviour
{
    [SerializeField] private AudioClip[] swipe;

    private AudioSource audioSource;



    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayRandomSwipeSound()
    {
        audioSource.clip = swipe[Random.Range(0, swipe.Length)];
        SFXManager.Instance.PlaySound(audioSource, SFXType.SuspectSorting);
    }
}
