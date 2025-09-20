using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour
{
    [SerializeField] private TMP_Text emeraldAmountText;
    [SerializeField] private TMP_Text energyAmountText;
    [SerializeField] private TMP_Text goldAmountText;
    [SerializeField] private Button addEnergyButton;

    private int emeraldAmount;
    private int energyAmount;
    private int goldAmount;

    private void Awake()
    {
        UpdateLabels();
        DataManager.Instance.OnResourceDataUpdated += UpdateLabels;
        addEnergyButton.onClick.AddListener(OnAddEnergyButtonClicked);
    }

    private void UpdateLabels()
    {
        emeraldAmount = DataManager.Instance.GameData.collectedEmeralds;
        emeraldAmountText.text = emeraldAmount.ToString();

        energyAmount = DataManager.Instance.GameData.energy;
        energyAmountText.text = energyAmount.ToString() + "/20";

        goldAmount = DataManager.Instance.GameData.gold;
        goldAmountText.text = goldAmount.ToString();
    }

    private void OnAddEnergyButtonClicked()
    {
        DataManager.Instance.UpdateValue(Data.Energy, energyAmount + DataManager.Instance.GameData.sortingCost );
    }

    private void OnDestroy()
    {
        DataManager.Instance.OnResourceDataUpdated -= UpdateLabels;
    }
}
