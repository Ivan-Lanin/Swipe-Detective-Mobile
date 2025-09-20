using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Button))]
public class ToTheSortingLevel : ToTheLevelBase
{
    [SerializeField] private TMP_Text sortingCostText;
    [SerializeField] private GameObject hintAnimationObject;
    [SerializeField] private DepartmentCameraAnimator cameraAnimator;

    private Button startSortingButton;

    protected override void Awake()
    {
        sceneName = "SuspectSortingLevel";
        base.Awake();

        startSortingButton = GetComponent<Button>();
        startSortingButton.onClick.AddListener(StartSortingLevel);

        sortingCostText.text = "-" + DataManager.Instance.GameData.sortingCost.ToString();
    }

    private void Start()
    {
        if (DataManager.Instance.GameData.tutorialState == TutorialState.Welcome)
        {
            hintAnimationObject.SetActive(true);
        }
        else
        {
            hintAnimationObject.SetActive(false);
        }
    }

    public void StartSortingLevel()
    {
        if (DataManager.Instance.GameData.isFirstLaunch)
        {
            DataManager.Instance.UpdateValue(Data.IsFirstLaunch, false);
        }

        if (DataManager.Instance.GameData.energy < DataManager.Instance.GameData.sortingCost)
        {
            Debug.Log("Not enough energy to start the sorting level.");
            return;
        }
        if (DataManager.Instance.GetPrisonersCount() >= 4)
        {
            Debug.Log("Prison is full. Cannot start sorting level.");
            return;
        }

        startSortingButton.interactable = false;
        DataManager.Instance.UpdateValue(Data.Energy, DataManager.Instance.GameData.energy - DataManager.Instance.GameData.sortingCost);

        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() => cameraAnimator.ZoomOnTransition());
        sequence.AppendInterval(0.25f);
        sequence.AppendCallback(() => StartLevel());
        sequence.Play();
    }

    protected override void OnDestroy()
    {
        startSortingButton.onClick.RemoveListener(StartSortingLevel);
        base.OnDestroy();
    }
}
