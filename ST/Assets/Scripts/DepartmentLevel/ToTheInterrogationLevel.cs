using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ToTheInterrogationLevel : ToTheLevelBase
{
    [SerializeField] private DepartmentCameraAnimator cameraAnimator;

    private Button startInterrogationButton;

    protected override void Awake()
    {
        sceneName = "InterrogationLevel";
        startInterrogationButton = GetComponent<Button>();
        startInterrogationButton.onClick.AddListener(StartInterrogationLevel);

        base.Awake();
    }

    public void StartInterrogationLevel()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() => cameraAnimator.ZoomOnTransition());
        sequence.AppendInterval(0.25f);
        sequence.AppendCallback(() => StartLevel());
        sequence.Play();
    }

    protected override void OnDestroy()
    {
        startInterrogationButton.onClick.RemoveListener(StartInterrogationLevel);
    }
}
