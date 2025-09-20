using UnityEngine;

public class MainThemeAudio : MonoBehaviour
{
    public static MainThemeAudio Instance { get; private set; }
    public AudioSource audioSource;

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
        audioSource = GetComponent<AudioSource>();
    }
}
