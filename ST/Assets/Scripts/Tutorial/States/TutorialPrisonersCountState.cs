using DG.Tweening;
using System.Collections;
using UnityEngine;

public class TutorialPrisonersCountState : TutorialBaseState
{
    private TutorialStateManager tutorialStateManager;
    private TutorialDepartmentReferences departmentRefs;

    public override void EnterState(TutorialStateManager tutorialStateManager)
    {
        this.tutorialStateManager = tutorialStateManager;
        if (tutorialStateManager.currentLevelName != "DepartmentLevel")
        {
            tutorialStateManager.StartCoroutineFromState(WaitForDepartmentLevel());
            return;
        }

        DataManager.Instance.UpdateValue(Data.TutorialState, TutorialState.PrisonersCount);
        SetReferences(tutorialStateManager.departmentReferences);
        SetUpUI();
    }

    private void SetReferences(TutorialDepartmentReferences tutorialReferences)
    {
        departmentRefs = tutorialReferences;
        departmentRefs.dialogueContinueButton?.onClick.AddListener(OnDialogueContinueButtonClicked);
        departmentRefs.toTheSortingLevelButton?.onClick.AddListener(StartNextState);
    }

    private void JumpOut()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(departmentRefs.dialogueFrame.GetComponent<RectTransform>().DOAnchorPosY(235, 0.5f).SetEase(Ease.OutBack));
        sequence.Join(departmentRefs.dialogueIcon.GetComponent<RectTransform>().DOAnchorPosY(140, 0.5f).SetEase(Ease.OutBack));
        sequence.AppendCallback(() => tutorialStateManager.PlayBlaBlaBlaSound());
        sequence.Append(departmentRefs.dialogueIcon.GetComponent<RectTransform>().DOAnchorPosY(98, 0.2f).SetEase(Ease.OutBack));
        sequence.Play();
    }

    private void HideDown()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(departmentRefs.dialogueFrame.GetComponent<RectTransform>().DOAnchorPosY(0, 0.5f).SetEase(Ease.InBack));
        sequence.AppendCallback(() => departmentRefs.tutorialCanvas.SetActive(false));
        sequence.Play();
    }

    private void SetUpUI()
    {
        departmentRefs.cameraController.enabled = false;
        departmentRefs.tutorialCanvas.SetActive(true);
        departmentRefs.menuCanvas.SetActive(false);
        JumpOut();
        departmentRefs.dialogueAnimator.SetTrigger("Show");
        departmentRefs.dialogueContinueButton.gameObject.SetActive(true);
        departmentRefs.dialogueText.text = DataManager.Instance.TutorialDialogueText.prisonersCountText;
    }

    private void OnDialogueContinueButtonClicked()
    {
        departmentRefs.menuCanvas.SetActive(true);
        tutorialStateManager.PlayBlaSound();
        HideDown();
        StartNextState();
    }

    private void StartNextState()
    {
        tutorialStateManager.SwitchState(tutorialStateManager.sendPrisonersState);
    }

    public override void ExitState()
    {
        departmentRefs.dialogueContinueButton?.onClick.RemoveListener(OnDialogueContinueButtonClicked);
        departmentRefs.toTheSortingLevelButton?.onClick.RemoveListener(StartNextState);
        departmentRefs.cameraController.enabled = true;
        DataManager.Instance.UpdateValue(Data.TutorialState, TutorialState.SendPrisoners);
    }

    private IEnumerator WaitForDepartmentLevel()
    {
        while (tutorialStateManager.currentLevelName != "DepartmentLevel")
        {
            yield return null;
        }
        EnterState(tutorialStateManager);
    }
}
