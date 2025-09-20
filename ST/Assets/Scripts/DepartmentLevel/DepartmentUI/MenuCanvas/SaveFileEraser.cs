using DG.Tweening;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveFileEraser : MonoBehaviour
{
    [SerializeField] private string departmentLevelSceneName = "DepartmentLevel";
    [SerializeField] private TutorialDepartmentReferences departmentRefs;
    [SerializeField] private GameObject saveFileErasingPanelAnimator;

    private Button deleteSaveFileButton;
    private string saveFilePath;

    private void Awake()
    {
        deleteSaveFileButton = GetComponent<Button>();
        saveFileErasingPanelAnimator.GetComponent<SaveFileErasingPanelAnimator>().onAnimationComplete.AddListener(OnDeletingAnimationComplete);
        deleteSaveFileButton.onClick.AddListener(OnDeleteSaveFileButtonPressed);
        departmentRefs.dialogueContinueButton.onClick.AddListener(ConfirmDeleteSaveFile);
        saveFilePath = Application.persistentDataPath + "/gamedata.json";
    }

    private void OnDeleteSaveFileButtonPressed()
    {
        deleteSaveFileButton.interactable = false;
        departmentRefs.tutorialCanvas.SetActive(true);
        departmentRefs.tutorialCanvas.GetComponent<Canvas>().sortingOrder = 2;
        departmentRefs.dialogueText.text = "Ummm... Are you sure you want to delete your save file? There is no Ctrl+Z here!";
        departmentRefs.dialogueContinueButton.gameObject.GetComponentInChildren<TMP_Text>().text = "Delete!";
        AYSJumpOut();
    }

    private void AYSJumpOut()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(departmentRefs.dialogueFrame.GetComponent<RectTransform>().DOAnchorPosY(235, 0.5f).SetEase(Ease.OutBack));
        sequence.Join(departmentRefs.dialogueIcon.GetComponent<RectTransform>().DOAnchorPosY(140, 0.5f).SetEase(Ease.OutBack));
        sequence.Append(departmentRefs.dialogueIcon.GetComponent<RectTransform>().DOAnchorPosY(98, 0.2f).SetEase(Ease.OutBack));
        sequence.Play();
    }

    private void AYSHideDown()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(departmentRefs.dialogueFrame.GetComponent<RectTransform>().DOAnchorPosY(-150f, 0.5f).SetEase(Ease.InBack));
        sequence.AppendCallback(() => departmentRefs.tutorialCanvas.SetActive(false));
        sequence.Play();
    }

    public void ConfirmDeleteSaveFile()
    {
        saveFileErasingPanelAnimator.SetActive(true);
        AYSHideDown();
    }

    public void OnDeletingAnimationComplete()
    {
        DeleteSaveFile();
    }

    private void DeleteSaveFile()
    {
        if (System.IO.File.Exists(saveFilePath))
        {
            System.IO.File.Delete(saveFilePath);
            Debug.Log("Save file deleted: " + saveFilePath);
        }
        else
        {
            Debug.LogWarning("Save file not found: " + saveFilePath);
        }

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        DestroyAllDontDestroyOnLoadObjects();

        SceneManager.LoadScene(departmentLevelSceneName);
    }

    private void DestroyAllDontDestroyOnLoadObjects()
    {
        GameObject[] persistentObjects = FindObjectsByType<GameObject>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        )
        .Where(go => go.scene.name == "DontDestroyOnLoad")
        .ToArray();

        foreach (GameObject obj in persistentObjects)
        {
            Destroy(obj);
            Debug.Log($"Destroyed persistent object: {obj.name}");
        }
    }

    public void OnSettingsExit()
    {
        departmentRefs.dialogueContinueButton.interactable = false;
        departmentRefs.tutorialCanvas.GetComponent<Canvas>().sortingOrder = 0;
        AYSHideDown();
        departmentRefs.dialogueContinueButton.interactable = true;
        deleteSaveFileButton.interactable = true;
    }

    private void OnDestroy()
    {
        deleteSaveFileButton.onClick.RemoveListener(OnDeleteSaveFileButtonPressed);
        departmentRefs.dialogueContinueButton.onClick.RemoveListener(ConfirmDeleteSaveFile);
        saveFileErasingPanelAnimator.GetComponent<SaveFileErasingPanelAnimator>().onAnimationComplete.RemoveListener(OnDeletingAnimationComplete);
    }
}
