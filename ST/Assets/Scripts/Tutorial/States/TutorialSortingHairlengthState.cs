using UnityEngine;

public class TutorialSortingHairlengthState : TutorialBaseState
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
        sortingRefs.wigArrowTrigger.onBeginDrag.AddListener(HideWigArrow);
        sortingRefs.suspectsManager.onNextSuspect.AddListener(OnNextSuspect);
        sortingRefs.sortingLevelManager.onNextRound.AddListener(OnNextRound);
    }

    private void SetUpUI()
    {
        sortingRefs.inputHandler.SetActive(false);
        sortingRefs.tutorialCanvas.SetActive(true);
        sortingRefs.dialogueAnimator.SetTrigger("Show");
        sortingRefs.dialogueContinueButton.gameObject.SetActive(true);
        sortingRefs.dialogueText.text = DataManager.Instance.TutorialDialogueText.sortingHairLengthText;
    }

    private void OnDialogueContinueButtonClicked()
    {
        sortingRefs.dialogueContinueButton.gameObject.SetActive(false);
        OnNextSuspect();
        sortingRefs.inputHandler.SetActive(true);
    }

    private void OnNextSuspect()
    {
        GameObject currentSuspect = sortingRefs.suspectsManager.GetCurrentSuspect();
        GameObject correctSuspect = sortingRefs.suspectsManager.CorrectSuspect;

        (string suspectHairLength, string correctHairLength) = sortingRefs.suspectsManager.suspectFeatureService.GetHairLengths(currentSuspect, correctSuspect);
        
        suspectHairLength = suspectHairLength.ToLower();
        correctHairLength = correctHairLength.ToLower();

        SetDialogueTextHint(suspectHairLength, correctHairLength);
        SetSwipeHintAnimation(suspectHairLength, correctHairLength);
    }

    private void OnNextRound()
    {
        if (sortingRefs.sortingLevelManager.CurrentGameState != GameState.Round2)
        {
            return;
        }
        tutorialStateManager.SwitchState(tutorialStateManager.sortingHairColorState);
    }

    private void SetDialogueTextHint(string suspectHairLength, string correctHairLength)
    {
        if (tutorialStateManager.currentState != this)
        {
            Debug.LogWarning("State has changed, not setting dialogue text.");
            return;
        }

        string newText;
        string suspectText = suspectHairLength;
        string correctText = correctHairLength.Contains("wig") ? "no hair" : correctHairLength;
        suspectHairLength = suspectHairLength.Contains("wig") ? "no hair" : suspectHairLength;
        string correctSwipeDirection = suspectHairLength == correctText ? "right" : "left";
        newText = $"This suspect has {suspectText}. You need a suspect with {correctText}. Swipe {correctSwipeDirection}.";
        sortingRefs.dialogueText.text = newText;
        sortingRefs.dialogueAnimator.SetTrigger("Show");

        if (suspectText.Contains("wig")) sortingRefs.wigArrow.SetActive(true);
        else sortingRefs.wigArrow.SetActive(false);
    }

    private void SetSwipeHintAnimation(string suspectHairLength, string correctHairLength)
    {
        correctHairLength = correctHairLength.Contains("wig") ? "no hair" : correctHairLength;
        suspectHairLength = suspectHairLength.Contains("wig") ? "no hair" : suspectHairLength;
        string triggerName = suspectHairLength == correctHairLength ? "SwipeRight" : "SwipeLeft";

        ResetTriggers();
        sortingRefs.swipeHintAnimator.SetTrigger(triggerName);
    }

    private void HideWigArrow()
    {
        sortingRefs.wigArrow.SetActive(false);
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
        sortingRefs.wigArrowTrigger.onBeginDrag.RemoveListener(HideWigArrow);

        sortingRefs.suspectsManager.onNextSuspect.RemoveListener(OnNextSuspect);
        sortingRefs.sortingLevelManager.onNextRound.RemoveListener(OnNextRound);

        ResetTriggers();
        sortingRefs.swipeHintAnimator.SetTrigger("Idle");
    }
}
