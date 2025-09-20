using System.Collections;
using UnityEngine;

public class Heart : MonoBehaviour, ICollectable
{
    [SerializeField] private BirstFX burstFX;

    private HealthPanel healthPanel;
    private GuessPanels guessPanels;

    private const float maxScale = 1.1f;
    private const float scaleIncrement = 0.01f;
    private const int animationSpeed = 200;


    void Awake()
    {
        healthPanel = FindFirstObjectByType<HealthPanel>();
        guessPanels = FindFirstObjectByType<GuessPanels>();
    }

    public void Collect()
    {
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
        yield return StartCoroutine(CollectAnimation());

        healthPanel.OnHeal();
        guessPanels.ShowHealPanel();
        SortingLevelManager.Instance.OnHeal();

        Destroy(gameObject);
    }

    private IEnumerator CollectAnimation()
    {
        var meshRenderer = GetComponent<MeshRenderer>();
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
        material.color -= new Color(0, 0, 0, scaleIncrement) * Time.deltaTime * animationSpeed;
    }
}
