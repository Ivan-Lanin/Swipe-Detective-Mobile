using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ToTheDepartmentLevel))]
public class PlayRevivePanel : FadeInPanel
{
    [SerializeField] Button continueButton;

    private ToTheDepartmentLevel toTheDepartmentLevelComponent;

    protected override void Awake()
    {
        base.Awake();
        toTheDepartmentLevelComponent = GetComponent<ToTheDepartmentLevel>();
        continueButton.onClick?.AddListener(OnContinueButtonClicked);
    }

    public override void Appear()
    {
        base.Appear();
        StartCoroutine(FadeIn(0.15f));
    }

    private void OnContinueButtonClicked()
    {
        DataManager.Instance.UpdateValue(Data.Energy, DataManager.Instance.GameData.energy - 1);
        toTheDepartmentLevelComponent.StartDepartmentLevel();
    }

    private void OnDestroy()
    {
        continueButton.onClick?.RemoveListener(OnContinueButtonClicked);
    }
}
