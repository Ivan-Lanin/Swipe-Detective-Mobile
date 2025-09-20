using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InterrogationControls : MonoBehaviour
{
    [SerializeField] private Button nextFeatureButton;
    [SerializeField] private Button previousFeatureButton;
    [SerializeField] private Button nextInformantButton;
    [SerializeField] private Button nextFeatureTypeButton;
    [SerializeField] private Button useXRayButton;
    [SerializeField] private Button catchALieButton;
    [SerializeField] private Button goBackButton;

    [SerializeField] private Image nextFeatureTypeIcon;
    [SerializeField] private Sprite[] nextFeatureTypeIcons;

    public UnityEvent onNextFeature;
    public UnityEvent onPreviousFeature;
    public UnityEvent onNextFeatureType;
    public UnityEvent onNextInformant;
    public UnityEvent onuseXRay;
    public UnityEvent oncatchALie;
    public UnityEvent ongoBack;

    private int currentFeatureTypeIndex = 0;

    public static InterrogationControls Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        nextFeatureButton.onClick.AddListener(() => onNextFeature.Invoke());
        previousFeatureButton.onClick.AddListener(() => onPreviousFeature.Invoke());
        nextFeatureTypeButton.onClick.AddListener(() => onNextFeatureType.Invoke());
        nextFeatureTypeButton.onClick.AddListener(SetNextFeatureTypeIcon);
        nextInformantButton.onClick.AddListener(() => onNextInformant.Invoke());
        useXRayButton.onClick.AddListener(OnXRayButtonClicked);
        catchALieButton.onClick.AddListener(() => oncatchALie.Invoke());
        goBackButton.onClick.AddListener(() => ongoBack.Invoke());
    }

    public void SetTabletControlsActiveState(bool isActive)
    {
        nextFeatureButton.gameObject.SetActive(isActive);
        previousFeatureButton.gameObject.SetActive(isActive);
        nextFeatureTypeButton.gameObject.SetActive(isActive);
    }

    private void SetNextFeatureTypeIcon()
    {
        currentFeatureTypeIndex = (currentFeatureTypeIndex + 1) % nextFeatureTypeIcons.Length;
        nextFeatureTypeIcon.sprite = nextFeatureTypeIcons[currentFeatureTypeIndex];
    }

    public void SetFeatureTypeIcon(int nextIndex = -1)
    {
        if (nextIndex <= 0 && nextIndex >= nextFeatureTypeIcons.Length)
        {
            return;
        }
        currentFeatureTypeIndex = nextIndex;
        nextFeatureTypeIcon.sprite = nextFeatureTypeIcons[currentFeatureTypeIndex];
    }

    public void SetNextInformantButtonActive(bool isActive)
    {
        nextInformantButton.gameObject.SetActive(isActive);
    }

    public void SetNextFeatureTypeButtonActive(bool isActive)
    {
        nextFeatureTypeButton.gameObject.SetActive(isActive);
    }

    public void SetUseXRayButtonActive(bool isActive)
    {
        useXRayButton.gameObject.SetActive(isActive);
    }

    public void SetGoBackButtonActive(bool isActive)
    {
        goBackButton.gameObject.SetActive(isActive);
        goBackButton.transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutBack);
    }

    private void OnXRayButtonClicked()
    {
        SetUseXRayButtonActive(false);
        onuseXRay.Invoke();
    }

    private void OnDestroy()
    {
        onNextFeature.RemoveAllListeners();
        onPreviousFeature.RemoveAllListeners();
        onNextFeatureType.RemoveAllListeners();
        onNextInformant.RemoveAllListeners();
        onuseXRay.RemoveAllListeners();
        oncatchALie.RemoveAllListeners();
        ongoBack.RemoveAllListeners();
    }
}
