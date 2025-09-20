using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialSendPrisonersState : TutorialBaseState
{
    private TutorialStateManager tutorialStateManager;
    private TutorialDepartmentReferences departmentRefs;
    private Coroutine waitForFullPrisonCoroutine;

    public override void EnterState(TutorialStateManager tutorialStateManager)
    {
        this.tutorialStateManager = tutorialStateManager;
        if (DataManager.Instance.GetPrisonersCount() < 4)
        {
            waitForFullPrisonCoroutine = tutorialStateManager.StartCoroutineFromState(WaitForFullPrison());
            return;
        }

        SetReferences(tutorialStateManager.departmentReferences);
        SetUpUI();
    }

    private void SetReferences(TutorialDepartmentReferences tutorialReferences)
    {
        departmentRefs = tutorialReferences;
        departmentRefs.sendPrisonersButton?.onClick.AddListener(OnSendPrisonersClicked);
        departmentRefs.toTheSortingLevelButton?.onClick.AddListener(StartNextState);
        departmentRefs.dialogueContinueButton?.onClick.AddListener(OnDialogueContinueButtonClicked);
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
        departmentRefs.cameraController.enabled = true;
        departmentRefs.tutorialCanvas.SetActive(true);
        JumpOut();
        departmentRefs.dialogueAnimator.SetTrigger("Show");
        departmentRefs.toTheSortingLevelCanvas.SetActive(false);
        departmentRefs.dialogueContinueButton.gameObject.SetActive(false);
        departmentRefs.dialogueText.text = DataManager.Instance.TutorialDialogueText.sendPrisonersTextFirst;
    }

    private void OnSendPrisonersClicked()
    {
        departmentRefs.dialogueAnimator.SetTrigger("Show");
        departmentRefs.toTheSortingLevelCanvas.SetActive(true);
        departmentRefs.dialogueContinueButton.gameObject.SetActive(true);
        departmentRefs.dialogueText.text = DataManager.Instance.TutorialDialogueText.sendPrisonersTextSecond;
    }

    private void OnDialogueContinueButtonClicked()
    {
        tutorialStateManager.PlayBlaSound();
        HideDown();
        StartNextState();
    }

    private void StartNextState()
    {
        tutorialStateManager.SwitchState(tutorialStateManager.buildEmeraldRoomState);
    }

    public override void ExitState()
    {
        if (waitForFullPrisonCoroutine != null)
        {
            tutorialStateManager.StopCoroutine(waitForFullPrisonCoroutine);
            waitForFullPrisonCoroutine = null;
        }

        departmentRefs.dialogueContinueButton?.onClick.RemoveListener(OnSendPrisonersClicked);
        departmentRefs.cameraController.enabled = true;
        DataManager.Instance.UpdateValue(Data.TutorialState, TutorialState.BuildEmeraldRoom);
    }

    private IEnumerator WaitForFullPrison()
    {
        while (DataManager.Instance.GetPrisonersCount() < 4)
        {
            yield return new WaitForSeconds(1f);
        }
        SetReferences(tutorialStateManager.departmentReferences);
        SetUpUI();
    }
}
