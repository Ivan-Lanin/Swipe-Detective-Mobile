using UnityEngine;

public class ShowGuessController : MonoBehaviour
{
    private SuspectsManager suspectsManager;
    private Animator showGuessAnimator;

    private void Awake()
    {
        suspectsManager = FindAnyObjectByType<SuspectsManager>();
        showGuessAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        suspectsManager.onWrongGuess.AddListener(ShowWrongGuessAnimation);
        suspectsManager.onCorrectGuess.AddListener(ShowCorrectGuessAnimation);
    }

    private void ShowCorrectGuessAnimation()
    {
        showGuessAnimator.SetTrigger("Correct");
    }

    private void ShowWrongGuessAnimation()
    {
        showGuessAnimator.SetTrigger("Wrong");
    }

    private void OnDestroy()
    {
        suspectsManager.onWrongGuess.RemoveListener(ShowWrongGuessAnimation);
        suspectsManager.onCorrectGuess.RemoveListener(ShowCorrectGuessAnimation);
    }
}
