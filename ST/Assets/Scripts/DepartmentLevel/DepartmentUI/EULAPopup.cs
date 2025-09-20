using UnityEngine;
using UnityEngine.UI;

public class EULAPopup : MonoBehaviour
{
    [SerializeField] private MainCameraController mainCameraController;
    [SerializeField] private GameObject popup;
    [SerializeField] private Button OkButton;

    private void Awake()
    {
        if (DataManager.Instance.GameData.eulaAccepted == true)
        {
            Destroy(gameObject);
            return;
        }

        popup.SetActive(true);
        mainCameraController.enabled = false;
        OkButton.onClick?.AddListener(OnOkButtonClicked);
    }

    private void OnOkButtonClicked()
    {
        mainCameraController.enabled = true;
        DataManager.Instance.UpdateValue(Data.EULAAccepted, true);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        OkButton.onClick?.RemoveListener(OnOkButtonClicked);
    }
}
