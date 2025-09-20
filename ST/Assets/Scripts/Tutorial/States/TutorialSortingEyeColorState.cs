using UnityEngine;
using DG.Tweening;

public class TutorialSortingEyeColorState : TutorialBaseState
{
    private TutorialSortingReferences sortingRefs;
    private TutorialStateManager tutorialStateManager;

    public override void EnterState(TutorialStateManager tutorialStateManager)
    {
        SetReferences(tutorialStateManager.sortingReferences);
        this.tutorialStateManager = tutorialStateManager;
        SetUpUI();
    }

    private void SetReferences(TutorialSortingReferences tutorialReferences)
    {
        sortingRefs = tutorialReferences;
        sortingRefs.dialogueContinueButton?.onClick.AddListener(OnDialogueContinueButtonClicked);
        sortingRefs.suspectsManager.onNextSuspect.AddListener(OnNextSuspect);
        sortingRefs.sortingLevelManager.onNextRound.AddListener(OnNextRound);
    }

    private void SetUpUI()
    {
        sortingRefs.inputHandler.SetActive(false);
        sortingRefs.tutorialCanvas.SetActive(true);
        sortingRefs.dialogueAnimator.SetTrigger("Show");
        sortingRefs.dialogueContinueButton.gameObject.SetActive(true);
        sortingRefs.dialogueText.text = DataManager.Instance.TutorialDialogueText.sortingEyeColorText;
        sortingRefs.dialogueFrame.GetComponent<RectTransform>().DOAnchorPosY(0, 0.5f).SetEase(Ease.OutBack);
        tutorialStateManager.PlayBlaSound();
    }

    private void OnDialogueContinueButtonClicked()
    {
        sortingRefs.dialogueFrame.GetComponent<RectTransform>().DOAnchorPosY(-639, 0.5f).SetEase(Ease.OutBack);
        sortingRefs.dialogueContinueButton.gameObject.SetActive(false);
        OnNextSuspect();
        sortingRefs.inputHandler.SetActive(true);
    }

    private void OnNextSuspect()
    {
        GameObject currentSuspect = sortingRefs.suspectsManager.GetCurrentSuspect();
        GameObject correctSuspect = sortingRefs.suspectsManager.CorrectSuspect;

        (string suspectEyeColor, string correctEyeColor) = sortingRefs.suspectsManager.suspectFeatureService.GetEyeColors(currentSuspect, correctSuspect);

        suspectEyeColor = suspectEyeColor.ToLower();
        correctEyeColor = correctEyeColor.ToLower();

        SetDialogueTextHint(suspectEyeColor, correctEyeColor);
        SetSwipeHintAnimation(suspectEyeColor, correctEyeColor);
    }

    private void OnNextRound()
    {
        if (sortingRefs.sortingLevelManager.CurrentGameState != GameState.Round4)
        {
            return;
        }
        tutorialStateManager.SwitchState(tutorialStateManager.sortingExtraState);
    }

    private void SetDialogueTextHint(string suspectEyeColor, string correctEyeColor)
    {
        if (tutorialStateManager.currentState != this)
        {
            Debug.LogWarning("State has changed, not setting dialogue text.");
            return;
        }

        string newText;
        suspectEyeColor += " eyes";
        correctEyeColor += " eyes";
        string correctSwipeDirection = suspectEyeColor == correctEyeColor ? "right" : "left";
        newText = $"This suspect has {suspectEyeColor}. You need a suspect with {correctEyeColor}. Swipe {correctSwipeDirection}.";
        sortingRefs.dialogueText.text = newText;
        sortingRefs.dialogueAnimator.SetTrigger("Show");
    }

    private void SetSwipeHintAnimation(string suspectEyeColor, string correctEyeColor)
    {
        string triggerName = suspectEyeColor == correctEyeColor ? "SwipeRight" : "SwipeLeft";

        ResetTriggers();
        sortingRefs.swipeHintAnimator.SetTrigger(triggerName);
    }

    private void ResetTriggers()
    {
        sortingRefs.swipeHintAnimator.ResetTrigger("SwipeLeft");
        sortingRefs.swipeHintAnimator.ResetTrigger("SwipeRight");
        sortingRefs.swipeHintAnimator.ResetTrigger("Idle");
    }

    public override void ExitState()
    {
        sortingRefs.dialogueContinueButton?.onClick.RemoveListener(OnDialogueContinueButtonClicked);
        sortingRefs.suspectsManager.onNextSuspect.RemoveListener(OnNextSuspect);
        sortingRefs.sortingLevelManager.onNextRound.RemoveListener(OnNextRound);
        sortingRefs.tutorialCanvas.SetActive(false);
    }
}
