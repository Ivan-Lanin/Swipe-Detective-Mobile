using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class InformantsManager : MonoBehaviour
{
    [SerializeField] InformantReactionManager[] informants;

    private InformantReactionManager currentInformant;
    public int currentInformantIndex { get; private set; } = 0;
    public InformantReactionManager[] Informants => informants;

    public UnityEvent<int> onNextInformantReady;
    public UnityEvent<int> decisionPanelReady;

    public static InformantsManager Instance { get; private set; }

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

        currentInformant = informants[currentInformantIndex];
    }
    private void Start()
    {
        InterrogationControls.Instance.onNextInformant.AddListener(NextInformant);
        InterrogationControls.Instance.onNextFeature.AddListener(() => StartCoroutine(WaitForFeatureRendererSetup()));
        InterrogationControls.Instance.onPreviousFeature.AddListener(() => StartCoroutine(WaitForFeatureRendererSetup()));
        StartCoroutine(WaitForGeneratorReady());
    }

    private void SetTestimonies()
    {
        informants[0].SetTestimonyOnce(InterrogationFeatureGenerator.Instance.firstTestimonyIndexes);
        informants[1].SetTestimonyOnce(InterrogationFeatureGenerator.Instance.secondTestimonyIndexes);
        informants[2].SetTestimonyOnce(InterrogationFeatureGenerator.Instance.thirdTestimonyIndexes);
    }

    private void SetBullhead(int predeterminedIndex = -1)
    {
        if (predeterminedIndex < 0)
        {
            predeterminedIndex = Random.Range(0, informants.Length);
        }
        for (int i = 0; i < informants.Length; i++)
        {
            informants[i].SetIsBullhead(i == predeterminedIndex);
        }
    }

    private void NextInformant()
    {
        if (currentInformantIndex >= informants.Length - 1)
        {
            InterrogationControls.Instance.SetNextInformantButtonActive(false);
            InterrogationControls.Instance.SetFeatureTypeIcon(0);
            FinalDecisionManager.Instance.RollIn();
            decisionPanelReady.Invoke(currentInformantIndex);
            currentInformantIndex = currentInformantIndex + 1;
            currentInformant.gameObject.transform.DOLocalMoveX(-4f, 1f).SetRelative().OnComplete(() => Destroy(gameObject));
            return;
        }
        InformantReactionManager nextInformant = informants[currentInformantIndex + 1];
        InterrogationControls.Instance.SetNextInformantButtonActive(false);
        InterrogationControls.Instance.SetFeatureTypeIcon(0);

        nextInformant.gameObject.SetActive(true);
        Sequence currentInformantSequence = DOTween.Sequence();
        currentInformantSequence.AppendCallback(() => InterrogationControls.Instance.SetTabletControlsActiveState(false));
        currentInformantSequence.Append(currentInformant.gameObject.transform.DOLocalMoveX(-4f, 1f).SetRelative());
        currentInformantSequence.Join(nextInformant.gameObject.transform.DOLocalMoveX(-4f, 1f).SetRelative());
        currentInformantSequence.AppendCallback(() => currentInformant.gameObject.SetActive(false));
        currentInformantSequence.AppendCallback(() => InterrogationControls.Instance.SetTabletControlsActiveState(true));
        currentInformantSequence.AppendCallback(() => onNextInformantReady.Invoke(currentInformantIndex));
        currentInformantSequence.AppendCallback(() => currentInformantIndex = currentInformantIndex + 1);
        currentInformantSequence.AppendCallback(() => currentInformant = informants[currentInformantIndex]);
        currentInformantSequence.Play();
    }

    private IEnumerator WaitForFeatureRendererSetup()
    {
        yield return null;

        InterrogationFeatureType displayedFeatureType = InterrogationFeatureRenderer.Instance.DisplayedFeatureType;
        GameObject feature = InterrogationFeatureRenderer.Instance.DisplayedFeature;

        currentInformant.ReactOnFeature(displayedFeatureType, feature);
    }

    private IEnumerator WaitForGeneratorReady()
    {
        while (InterrogationFeatureGenerator.Instance.firstTestimonyIndexes == null)
        {
            yield return null;
        }

        SetTestimonies();
        SetBullhead(2);
    }

    private void OnDestroy()
    {
        InterrogationControls.Instance.onNextInformant.RemoveListener(NextInformant);
        InterrogationControls.Instance.onNextFeature.RemoveListener(() => StartCoroutine(WaitForFeatureRendererSetup()));
        InterrogationControls.Instance.onPreviousFeature.RemoveListener(() => StartCoroutine(WaitForFeatureRendererSetup()));
    }
}
