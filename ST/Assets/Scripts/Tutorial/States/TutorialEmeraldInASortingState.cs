using DG.Tweening;
using System.Collections;
using UnityEngine;

public class TutorialEmeraldInASortingState : TutorialBaseState
{
    private TutorialSortingReferences sortingRefs;
    private TutorialStateManager tutorialStateManager;

    private bool isOnCollectingEmeraldStage = false;
    private Emerald currentEmerald = null;

    public override void EnterState(TutorialStateManager tutorialStateManager)
    {
        this.tutorialStateManager = tutorialStateManager;
        if (tutorialStateManager.currentLevelName != "SuspectSortingLevel")
        {
            tutorialStateManager.StartCoroutineFromState(WaitForSortingLevel());
            return;
        }
        SetReferences(tutorialStateManager.sortingReferences);
    }

    private void SetReferences(TutorialSortingReferences tutorialReferences)
    {
        sortingRefs = tutorialReferences;
        sortingRefs.suspectsManager.onNextSuspect.AddListener(OnNextSuspect);
        sortingRefs.rotationSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnNextSuspect()
    {
        Emerald emeraldRef = sortingRefs.suspectsManager.GetCurrentSuspect().GetComponentInChildren<Emerald>();
        if (emeraldRef != null)
        {
            SetUpUI();
            SetSliderHintAnimation();
            SortingLevelManager.Instance.onEmeraldCollect.AddListener(OnEmeraldCollected);
            currentEmerald = emeraldRef;
        }
        else currentEmerald = null;
    }

    private void SetUpUI()
    {
        sortingRefs.dialogueFrame.GetComponent<RectTransform>().DOAnchorPosY(-639, 0.5f).SetEase(Ease.OutBack);
        sortingRefs.dialogueContinueButton.gameObject.SetActive(false);
        sortingRefs.inputHandler.SetActive(false);
        sortingRefs.tutorialCanvas.SetActive(true);
        sortingRefs.dialogueAnimator.SetTrigger("Show");
        sortingRefs.dialogueText.text = DataManager.Instance.TutorialDialogueText.emeraldInASortingTextFirst;
    }

    private void OnSliderValueChanged(float value)
    {
        if (currentEmerald == null) return;
        if (value > -75 && value < 100) return;
        if (isOnCollectingEmeraldStage) return;

        isOnCollectingEmeraldStage = true;

        tutorialStateManager.PlayBlaSound();

        Sequence sequence = DOTween.Sequence();
        sequence.Append(sortingRefs.dialogueFrame.GetComponent<RectTransform>().DOAnchorPosY(-600, 0.15f));
        sequence.Append(sortingRefs.dialogueFrame.GetComponent<RectTransform>().DOAnchorPosY(-678, 0.15f));
        sequence.Append(sortingRefs.dialogueFrame.GetComponent<RectTransform>().DOAnchorPosY(-639, 0.15f).SetEase(Ease.InBack));
        sequence.Play();

        ResetTriggers();
        sortingRefs.swipeHintAnimator.SetTrigger("Idle");

        sortingRefs.dialogueAnimator.SetTrigger("Show");
        sortingRefs.dialogueText.text = DataManager.Instance.TutorialDialogueText.emeraldInASortingTextSecond;
        sortingRefs.inputHandler.SetActive(true);
        sortingRefs.inputHandler.GetComponent<SortingLevelInputHandler>().allowDragging = false;
        sortingRefs.rotationSlider.enabled = false;
    }

    private void OnEmeraldCollected()
    {
        sortingRefs.inputHandler.GetComponent<SortingLevelInputHandler>().allowDragging = true;
        StartNextState();
    }

    private void SetSliderHintAnimation()
    {
        ResetTriggers();
        sortingRefs.swipeHintAnimator.SetTrigger("Slider");
    }

    private void ResetTriggers()
    {
        sortingRefs.swipeHintAnimator.ResetTrigger("SwipeLeft");
        sortingRefs.swipeHintAnimator.ResetTrigger("SwipeRight");
        sortingRefs.swipeHintAnimator.ResetTrigger("Slider");
        sortingRefs.swipeHintAnimator.ResetTrigger("Idle");
    }

    private void StartNextState()
    {
        tutorialStateManager.SwitchState(tutorialStateManager.completedState);
    }

    public override void ExitState()
    {
        sortingRefs.rotationSlider.enabled = true;

        sortingRefs.suspectsManager.onNextSuspect.RemoveListener(OnNextSuspect);
        sortingRefs.rotationSlider.onValueChanged.RemoveListener(OnSliderValueChanged);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(sortingRefs.dialogueFrame.GetComponent<RectTransform>().DOAnchorPosY(-2000, 0.5f));
        sequence.AppendCallback(() => sortingRefs.tutorialCanvas.SetActive(false));
        sequence.Play();
        DataManager.Instance.UpdateValue(Data.TutorialState, TutorialState.Completed);
    }

    private IEnumerator WaitForSortingLevel()
    {
        while (tutorialStateManager.currentLevelName != "SuspectSortingLevel")
        {
            yield return null;
        }
        EnterState(tutorialStateManager);
    }
}
