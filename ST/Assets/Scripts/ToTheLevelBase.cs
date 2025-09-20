using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class ToTheLevelBase : MonoBehaviour
{
    protected string sceneName = "SceneName";
    protected Transition transition;

    protected virtual void Awake()
    {
        transition = GameObject.Find("Transition_Image").GetComponent<Transition>();
        transition.onFadeInFinished += OnFadeInFinished;
    }

    protected virtual private void OnFadeInFinished(string level)
    {
        if (level != sceneName) return;
        SceneManager.LoadScene(sceneName);
    }

    protected virtual void StartLevel()
    {
        transition.FadeIn(sceneName);
    }

    protected virtual void OnDestroy()
    {
        transition.onFadeInFinished -= OnFadeInFinished;
    }
}
