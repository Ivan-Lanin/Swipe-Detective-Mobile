using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Transition : MonoBehaviour
{
    private const string _fadeInTrigger = "FadeIn";
    private const string _fadeOutTrigger = "FadeOut";

    private string currentLevelName;

    public event Action<string> onFadeInFinished;
    public event Action onFadeOutFinished;

    private Animator _animator;

    public static Transition Instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        if (Instance == null)
        {
            GameObject transitionObj = Resources.Load<GameObject>("Transitions/TransitionCanvas");
            if (transitionObj != null)
            {
                Instantiate(transitionObj);
            }
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _animator = GetComponent<Animator>();
        DontDestroyOnLoad(gameObject);
    }

    public void FadeIn(string levelName)
    {
        currentLevelName = levelName;
        _animator.ResetTrigger(_fadeInTrigger);
        _animator.SetTrigger(_fadeInTrigger);
    }

    public void FadeOut()
    {
        _animator.ResetTrigger(_fadeOutTrigger);
        _animator.SetTrigger(_fadeOutTrigger);
    }

    public void OnFullFadeIn()
    {
        onFadeInFinished?.Invoke(currentLevelName);
        currentLevelName = null;
    }

    public void OnFullFadeOut()
    {
        onFadeOutFinished?.Invoke();
    }
}
