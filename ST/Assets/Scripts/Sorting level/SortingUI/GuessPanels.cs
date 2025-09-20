using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GuessPanels : MonoBehaviour
{
    [SerializeField] Image correctPanel;
    [SerializeField] Image incorrectPanel;

    private SuspectsManager suspectsManager;
    private float fadeDuration = 0.20f;

    private void Awake()
    {
        suspectsManager = FindAnyObjectByType<SuspectsManager>();
    }

    private void Start()
    {
        suspectsManager.onWrongGuess.AddListener(ShowIncorrectPanel);
    }

    private void ShowIncorrectPanel()
    {
        StartCoroutine(FadeInAndOut(incorrectPanel));
    }

    /// <summary>
    /// Displays the green panel with a fade-in and fade-out effect.
    /// </summary>
    public void ShowHealPanel()
    {
        StartCoroutine(FadeInAndOut(correctPanel));
    }

    private IEnumerator FadeInAndOut(Image panelImage)
    {
        Color color = panelImage.color;

        while (color.a < 0.3f)
        {
            color.a += Time.deltaTime / fadeDuration;
            panelImage.color = color;
            yield return null;
        }
        while (color.a > 0)
        {
            color.a -= Time.deltaTime / fadeDuration;
            panelImage.color = color;
            yield return null;
        }
    }

    private void OnDestroy()
    {
        if (suspectsManager != null)
        {
            suspectsManager.onWrongGuess.RemoveListener(ShowIncorrectPanel);
        }
    }
}
