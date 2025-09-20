using UnityEngine;
using DG.Tweening;

public class TutorialSortingHairColorState : TutorialBaseState
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
        sortingRefs.wigArrow.SetActive(false);
        sortingRefs.inputHandler.SetActive(false);
        sortingRefs.tutorialCanvas.SetActive(true);
        sortingRefs.dialogueAnimator.SetTrigger("Show");
        sortingRefs.dialogueContinueButton.gameObject.SetActive(true);
        sortingRefs.dialogueText.text = DataManager.Instance.TutorialDialogueText.sortingHairColorText;
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

        (string suspectHairColor, string correctHairColor) = sortingRefs.suspectsManager.suspectFeatureService.GetHairColors(currentSuspect, correctSuspect);

        suspectHairColor = suspectHairColor.ToLower();
        correctHairColor = correctHairColor.ToLower();

        SetDialogueTextHint(suspectHairColor, correctHairColor);
        SetSwipeHintAnimation(suspectHairColor, correctHairColor);
    }

    private void OnNextRound()
    {
        if (sortingRefs.sortingLevelManager.CurrentGameState != GameState.Round3)
        {
            return;
        }
        tutorialStateManager.SwitchState(tutorialStateManager.sortingEyeColorState);
    }

    private void SetDialogueTextHint(string suspectHairColor, string correctHairColor)
    {
        if (tutorialStateManager.currentState != this)
        {
            Debug.LogWarning("State has changed, not setting dialogue text.");
            return;
        }

        string newText;
        suspectHairColor = suspectHairColor == "bald" ? "no hair" : suspectHairColor;
        correctHairColor = correctHairColor == "bald" ? "no hair" : correctHairColor;
        string correctSwipeDirection = suspectHairColor == correctHairColor ? "right" : "left";
        newText = $"This suspect has {suspectHairColor}. You need a suspect with {correctHairColor}. Swipe {correctSwipeDirection}.";
        sortingRefs.dialogueText.text = newText;
        sortingRefs.dialogueAnimator.SetTrigger("Show");
    }

    private void SetSwipeHintAnimation(string suspectHairColor, string correctHairColor)
    {
        string triggerName = suspectHairColor == correctHairColor ? "SwipeRight" : "SwipeLeft";

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

        ResetTriggers();
        sortingRefs.swipeHintAnimator.SetTrigger("Idle");
    }
}
