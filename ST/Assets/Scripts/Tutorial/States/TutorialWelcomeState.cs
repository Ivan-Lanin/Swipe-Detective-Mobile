using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialWelcomeState : TutorialBaseState
{
    private TutorialStateManager tutorialStateManager;
    private TutorialDepartmentReferences departmentRefs;

    public override void EnterState(TutorialStateManager tutorialStateManager)
    {
        if (!DataManager.Instance.GameData.eulaAccepted)
        {
            tutorialStateManager.StartCoroutineFromState(WaitForEULA(tutorialStateManager));
            return;
        }

        this.tutorialStateManager = tutorialStateManager;
        SetReferences(tutorialStateManager.departmentReferences);
        SetUpUI();
    }

    private void SetReferences(TutorialDepartmentReferences tutorialReferences)
    {
        departmentRefs = tutorialReferences;
        departmentRefs.dialogueContinueButton?.onClick.AddListener(OnDialogueContinueButtonClicked);
        departmentRefs.toTheSortingLevelButton?.onClick.AddListener(OnToTheSortingLevelButtonClicked);
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
        departmentRefs.tutorialPanel.SetActive(true);
        Color color = departmentRefs.tutorialPanelImage.color;
        color.a = 0;
        departmentRefs.tutorialPanelImage.color = color;

        departmentRefs.cameraController.enabled = false;
        departmentRefs.tutorialCanvas.SetActive(true);
        departmentRefs.toTheSortingLevelCanvas.SetActive(false);
        departmentRefs.toTheInterrogationLevelCanvas.SetActive(false);
        departmentRefs.menuCanvas.SetActive(false);
        departmentRefs.dialogueText.text = DataManager.Instance.TutorialDialogueText.welcomeTextFirst;
        JumpOut();
        departmentRefs.dialogueAnimator.SetTrigger("Show");
    }

    private void OnDialogueContinueButtonClicked()
    {
        departmentRefs.tutorialPanelImage.DOFade(0.75f, 0.5f);

        departmentRefs.dialogueAnimator.SetTrigger("Show");
        departmentRefs.dialogueContinueButton.gameObject.SetActive(false);
        departmentRefs.toTheSortingLevelCanvas.SetActive(true);
        departmentRefs.dialogueText.text = DataManager.Instance.TutorialDialogueText.welcomeTextSecond;
    }

    private void OnToTheSortingLevelButtonClicked()
    {
        tutorialStateManager.StartCoroutineFromState(WaitForSortingLevel(tutorialStateManager));
    }

    public override void ExitState()
    {
        departmentRefs.dialogueContinueButton?.onClick.RemoveListener(OnDialogueContinueButtonClicked);
        departmentRefs.toTheSortingLevelButton?.onClick.RemoveListener(OnToTheSortingLevelButtonClicked);
    }

    private IEnumerator WaitForEULA(TutorialStateManager tutorialStateManager)
    {
        while (!DataManager.Instance.GameData.eulaAccepted)
        {
            yield return null;
        }
        EnterState(tutorialStateManager);
    }

    private IEnumerator WaitForSortingLevel(TutorialStateManager tutorialStateManager)
    {
        while (tutorialStateManager.currentLevelName != "SuspectSortingLevel")
        {
            yield return null;
        }
        // Transition to the sorting state
        tutorialStateManager.SwitchState(tutorialStateManager.sortingState);
    }
}
