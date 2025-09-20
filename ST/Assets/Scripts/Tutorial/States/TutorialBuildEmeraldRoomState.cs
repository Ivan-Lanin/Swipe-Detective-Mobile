using DG.Tweening;
using System.Collections;
using UnityEngine;

public class TutorialBuildEmeraldRoomState : TutorialBaseState
{
    private TutorialStateManager tutorialStateManager;
    private TutorialDepartmentReferences departmentRefs;
    private Coroutine waitForMoneyCoroutine;

    public override void EnterState(TutorialStateManager tutorialStateManager)
    {
        this.tutorialStateManager = tutorialStateManager;
        if (DataManager.Instance.GameData.gold < 30)
        {
            waitForMoneyCoroutine = tutorialStateManager.StartCoroutineFromState(WaitForEnoughMoney());
            return;
        }

        SetReferences(tutorialStateManager.departmentReferences);
        SetUpUI();
    }

    private void SetReferences(TutorialDepartmentReferences tutorialReferences)
    {
        departmentRefs = tutorialReferences;
        departmentRefs.buildEmeraldRoomButton?.onClick.AddListener(OnBuildEmeraldRoomClicked);
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

    private void SetUpUI()
    {
        DepartmentCameraAnimator.Instance.MoveToEvidenceRoomView();
        departmentRefs.tutorialCanvas.SetActive(true);
        JumpOut();
        departmentRefs.dialogueAnimator.SetTrigger("Show");
        departmentRefs.toTheSortingLevelCanvas.SetActive(false);
        departmentRefs.dialogueContinueButton.gameObject.SetActive(false);
        departmentRefs.dialogueText.text = DataManager.Instance.TutorialDialogueText.buildEmeraldRoomTextFirst;
    }

    private void OnBuildEmeraldRoomClicked()
    {
        departmentRefs.dialogueAnimator.SetTrigger("Show");
        departmentRefs.toTheSortingLevelCanvas.SetActive(true);
        departmentRefs.dialogueText.text = DataManager.Instance.TutorialDialogueText.buildEmeraldRoomTextSecond;
    }

    private void StartNextState()
    {
        tutorialStateManager.SwitchState(tutorialStateManager.emeraldInASortingState);
    }

    public override void ExitState()
    {
        if (waitForMoneyCoroutine != null)
        {
            tutorialStateManager.StopCoroutineFromState(waitForMoneyCoroutine);
            waitForMoneyCoroutine = null;
        }


        departmentRefs.dialogueContinueButton?.onClick.RemoveListener(OnBuildEmeraldRoomClicked);
        departmentRefs.cameraController.enabled = true;
        DataManager.Instance.UpdateValue(Data.TutorialState, TutorialState.EmeraldInASorting);
    }

    private IEnumerator WaitForEnoughMoney()
    {
        while (DataManager.Instance.GameData.gold < 30)
        {
            yield return null;
        }
        EnterState(tutorialStateManager);
    }
}
