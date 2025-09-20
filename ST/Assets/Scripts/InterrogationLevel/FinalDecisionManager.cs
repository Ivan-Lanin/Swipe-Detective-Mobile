using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ToTheDepartmentLevel))]
public class FinalDecisionManager : MonoBehaviour
{
    [SerializeField] Image decisionPanel;
    [SerializeField] Transform firstTestimony;
    [SerializeField] Transform secondTestimony;
    [SerializeField] Transform thirdTestimony;
    [SerializeField] RectTransform tablet;

    private List<GameObject> killerFeatures = new List<GameObject>();
    private ToTheDepartmentLevel toTheDepartmentLevelComponent;

    private InterrogationControls Controls => InterrogationControls.Instance;
    private InterrogationFeatureRenderer FeatureRenderer => InterrogationFeatureRenderer.Instance;
    private InterrogationFeatureGenerator FeatureGenerator => InterrogationFeatureGenerator.Instance;

    public static FinalDecisionManager Instance { get; private set; }

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
        toTheDepartmentLevelComponent = GetComponent<ToTheDepartmentLevel>();
    }

    private void Start()
    {
        Controls.onNextFeature.AddListener(() => StartCoroutine(CheckDecisionOnNextFrame()));
        Controls.onPreviousFeature.AddListener(() => StartCoroutine(CheckDecisionOnNextFrame()));
        Controls.onNextFeatureType.AddListener(() => StartCoroutine(CheckDecisionOnNextFrame()));
        Controls.ongoBack.AddListener(OnGoBackClicked);
    }

    public void RollIn()
    {
        killerFeatures = GetKillerFeatures();
        decisionPanel.gameObject.SetActive(true);
        decisionPanel.transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutBack);
    }

    private void CheckDecision()
    {
        if (decisionPanel.gameObject.activeSelf == false || FeatureRenderer.FeaturesOnTablet.Any(f => f == null)) return;

        List<GameObject> currentFeatures = GetCurrentFeatures();
        for (int i = 0; i < FeatureRenderer.FeaturesOnTablet.Count; i++)
        {
            string currentFeatureName = currentFeatures[i].name.Replace("(Clone)", "").Trim();

            if (currentFeatureName != killerFeatures[i].name)
            {
                return;
            }
        }
        FinalAnimation();
    }

    private List<GameObject> GetKillerFeatures()
    {
        if (FeatureGenerator == null)
        {
            Debug.LogError("FeatureGenerator is null!");
            return new List<GameObject>();
        }
        if (FeatureGenerator.killerFeatureIndexes == null)
        {
            Debug.LogError("killerFeatureIndexes is null!");
            return new List<GameObject>();
        }
        if (FeatureGenerator.Haircuts == null || FeatureGenerator.EyesColors == null || FeatureGenerator.Extras == null)
        {
            Debug.LogError("One or more feature lists are null!");
            return new List<GameObject>();
        }

        List<GameObject> killerFeatures = new List<GameObject>();
        killerFeatures.Add(FeatureGenerator.Haircuts[FeatureGenerator.killerFeatureIndexes[0]]);
        killerFeatures.Add(FeatureGenerator.EyesColors[FeatureGenerator.killerFeatureIndexes[1]]);
        killerFeatures.Add(FeatureGenerator.Extras[FeatureGenerator.killerFeatureIndexes[2]]);
        return killerFeatures;
    }

    private List<GameObject> GetCurrentFeatures()
    {
        List<GameObject> currentFeatures = new List<GameObject>();
        currentFeatures.Add(FeatureRenderer.FeaturesOnTablet[0]);
        currentFeatures.Add(FeatureRenderer.FeaturesOnTablet[1]);
        currentFeatures.Add(FeatureRenderer.FeaturesOnTablet[2]);
        return currentFeatures;
    }

    private void FinalAnimation()
    {
        Color color = new Color32(16, 255, 0, 150);
        Controls.SetTabletControlsActiveState(false);
        Sequence animation = DOTween.Sequence();

        animation.Append(firstTestimony.DOShakePosition(1.5f, new Vector3(15f, 15f, 0f), 15, 90f));
        animation.Join(secondTestimony.DOShakePosition(1.5f, new Vector3(15f, 15f, 0f), 15, 90f));
        animation.Join(thirdTestimony.DOShakePosition(1.5f, new Vector3(15f, 15f, 0f), 15, 90f));
        animation.Join(tablet.DOShakePosition(1.5f, new Vector3(15f, 15f, 0f), 15, 90f));
        animation.Join(decisionPanel.DOColor(color, 0.5f));

        animation.Append(firstTestimony.DOLocalMoveX(-2000f, 1f).SetRelative().SetEase(Ease.OutCirc));
        animation.Join(secondTestimony.DOLocalMoveX(2000f, 1f).SetRelative().SetEase(Ease.OutCirc));
        animation.Join(thirdTestimony.DOLocalMoveY(2000f, 1f).SetRelative().SetEase(Ease.OutCirc));

        animation.Join(tablet.DOLocalMove(Vector3.zero, 1f).SetEase(Ease.OutBack));
        animation.Join(tablet.DOScale(Vector3.one * 1.5f, 1f).SetEase(Ease.OutBack));
        animation.Join(tablet.DOLocalRotate(new Vector3(0f, 0f, -360f), 1f, RotateMode.LocalAxisAdd).SetEase(Ease.OutBack));

        animation.AppendCallback(() => Controls.SetGoBackButtonActive(true));


        animation.Play();
    }

    private void OnGoBackClicked()
    {
        toTheDepartmentLevelComponent.StartDepartmentLevel();
    }

    private IEnumerator CheckDecisionOnNextFrame()
    {
        yield return null;
        CheckDecision();
    }

    private void OnDestroy()
    {
        if (Controls != null)
        {
            Controls.onNextFeature.RemoveListener(() => StartCoroutine(CheckDecisionOnNextFrame()));
            Controls.onPreviousFeature.RemoveListener(() => StartCoroutine(CheckDecisionOnNextFrame()));
            Controls.onNextFeatureType.RemoveListener(() => StartCoroutine(CheckDecisionOnNextFrame()));
            Controls.ongoBack.RemoveListener(OnGoBackClicked);
        }
    }
}
