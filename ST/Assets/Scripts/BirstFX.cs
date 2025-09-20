using System.Collections;
using UnityEngine;

public class BirstFX : MonoBehaviour
{
    private ParticleSystem birst;

    void Awake()
    {
        birst = GetComponent<ParticleSystem>();
    }

    public void Play()
    {
        birst.Play();
        StartCoroutine(DestroyAfterEmmision());
    }

    private IEnumerator DestroyAfterEmmision()
    {
        while (birst.isPlaying)
        {
            yield return null;
        }
        Destroy(gameObject);
    }
}
