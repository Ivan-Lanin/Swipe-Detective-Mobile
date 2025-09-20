using DG.Tweening;
using UnityEngine;

public class TutorialSortingState : TutorialBaseState
{
    private TutorialStateManager tutorialStateManager;
    private TutorialSortingReferences sortingRefs;

    public override void EnterState(TutorialStateManager tutorialStateManager)
    {
        this.tutorialStateManager = tutorialStateManager;
        SetReferences(tutorialStateManager.sortingReferences);
        SetUpUI();
    }

    private void SetReferences(TutorialSortingReferences tutorialReferences)
    {
        sortingRefs = tutorialReferences;
        sortingRefs.dialogueContinueButton?.onClick.AddListener(OnDialogueContinueButtonClicked);
        sortingRefs.startButton?.onClick.AddListener(OnStartButtonClicked);
    }

    private void SetUpUI()
    {
        sortingRefs.tutorialCanvas.SetActive(true);
        sortingRefs.dialogueAnimator.SetTrigger("Show");
        sortingRefs.dialogueContinueButton.gameObject.SetActive(true);
        sortingRefs.dialogueText.text = DataManager.Instance.TutorialDialogueText.sortingTextFirst;
    }

    private void OnDialogueContinueButtonClicked()
    {

        sortingRefs.cluesArrow.SetActive(true);
        sortingRefs.startArrow.SetActive(true);
        sortingRefs.dialogueContinueButton.gameObject.SetActive(false);
        sortingRefs.dialogueText.text = DataManager.Instance.TutorialDialogueText.sortingTextSecond;
        sortingRefs.dialogueAnimator.SetTrigger("Show");
        sortingRefs.dialogueFrame.GetComponent<RectTransform>().DOAnchorPosY(-639, 0.5f).SetEase(Ease.OutBack);
    }

    private void OnStartButtonClicked()
    {
        sortingRefs.cluesArrow.SetActive(false);
        sortingRefs.startArrow.SetActive(false);
        sortingRefs.tutorialCanvas.SetActive(false);
        tutorialStateManager.SwitchState(tutorialStateManager.sortingHairlengthState);
    }

    public override void ExitState()
    {
        sortingRefs.tutorialCanvas.SetActive(false);
        sortingRefs.dialogueContinueButton?.onClick.RemoveListener(OnDialogueContinueButtonClicked);
        sortingRefs.startButton?.onClick.RemoveListener(OnStartButtonClicked);
    }
}
