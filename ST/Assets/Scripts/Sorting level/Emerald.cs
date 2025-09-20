using System.Collections;
using UnityEngine;

public class Emerald : MonoBehaviour, ICollectable
{
    [SerializeField] private BirstFX burstFX;
    private const float maxScale = 1.1f;
    private const float scaleIncrement = 0.01f;
    private const int animationSpeed = 200;

    public void Collect()
    {
        if (burstFX == null)
        {
            Debug.LogWarning("BurstFX is not assigned.");
            return;
        }
        StartCoroutine(CollectAndDestroy());
        burstFX.gameObject.transform.SetParent(null);
        burstFX.Play();
    }

    public void Despawn()
    {
        Destroy(gameObject);
    }

    private IEnumerator CollectAndDestroy()
    {
        SortingLevelManager.Instance.OnEmeraldCollect();
        yield return CollectAnimation();
        Destroy(gameObject);
    }

    private IEnumerator CollectAnimation()
    {
        var meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogWarning("MeshRenderer is missing on Emerald.");
            yield break;
        }

        var material = meshRenderer.material;
        while (transform.localScale.x < maxScale)
        {
            ScaleUp();
            FadeOut(material);
            yield return null;
        }
    }

    private void ScaleUp()
    {
        transform.localScale += Vector3.one * scaleIncrement * Time.deltaTime * animationSpeed;
    }

    private void FadeOut(Material material)
    {
        var color = material.color;
        color.a -= (1 - scaleIncrement) * Time.deltaTime * animationSpeed;
        material.color = color;
    }
}
