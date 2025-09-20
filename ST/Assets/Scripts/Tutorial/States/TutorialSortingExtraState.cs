using UnityEngine;
using DG.Tweening;

public class TutorialSortingExtraState : TutorialBaseState
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
        sortingRefs.sortingLevelManager.onWin.AddListener(OnWin);
    }

    private void SetUpUI()
    {
        sortingRefs.inputHandler.SetActive(false);
        sortingRefs.tutorialCanvas.SetActive(true);
        sortingRefs.dialogueAnimator.SetTrigger("Show");
        sortingRefs.dialogueContinueButton.gameObject.SetActive(true);
        sortingRefs.dialogueText.text = DataManager.Instance.TutorialDialogueText.sortingExtraText;
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

        (string suspectEyeColor, string correctEyeColor) = sortingRefs.suspectsManager.suspectFeatureService.GetExtras(currentSuspect, correctSuspect);

        suspectEyeColor = suspectEyeColor.ToLower();
        correctEyeColor = correctEyeColor.ToLower();

        SetDialogueTextHint(suspectEyeColor, correctEyeColor);
        SetSwipeHintAnimation(suspectEyeColor, correctEyeColor);
    }

    private void OnWin()
    {
        if (sortingRefs.sortingLevelManager.CurrentGameState != GameState.Win)
        {
            return;
        }
        tutorialStateManager.SwitchState(tutorialStateManager.prisonersCountState);
    }

    private void SetDialogueTextHint(string suspectExtra, string correctExtra)
    {
        if (tutorialStateManager.currentState != this)
        {
            Debug.LogWarning("State has changed, not setting dialogue text.");
            return;
        }

        string newText;
        string correctSwipeDirection = suspectExtra == correctExtra ? "right" : "left";
        newText = $"This suspect has {suspectExtra}. You need a suspect with {correctExtra}. Swipe {correctSwipeDirection}.";
        sortingRefs.dialogueText.text = newText;
        sortingRefs.dialogueAnimator.SetTrigger("Show");
    }

    private void SetSwipeHintAnimation(string suspectExtra, string correctExtra)
    {
        string triggerName = suspectExtra == correctExtra ? "SwipeRight" : "SwipeLeft";

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
        sortingRefs.sortingLevelManager.onWin.RemoveListener(OnWin);

        sortingRefs.tutorialCanvas.SetActive(false);
    }
}
