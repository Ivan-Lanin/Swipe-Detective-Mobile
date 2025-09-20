using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FadeInPanel : MonoBehaviour
{
    [SerializeField] protected GameObject[] panelsToActivate;

    protected CanvasGroup canvasGroup;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }



    public virtual void Appear()
    {
        foreach (GameObject panel in panelsToActivate)
        {
            panel.SetActive(true);
        }
    }

    protected IEnumerator FadeIn(float delay)
    {
        yield return new WaitForSeconds(delay);

        float fadeSpeed = 5f;
        while (canvasGroup.alpha < 1)
        {
            // Mathf.Min to ensure the alpha value does not exceed 1
            canvasGroup.alpha = Mathf.Min(canvasGroup.alpha + fadeSpeed * Time.deltaTime, 1);
            yield return null;
        }
    }
}
