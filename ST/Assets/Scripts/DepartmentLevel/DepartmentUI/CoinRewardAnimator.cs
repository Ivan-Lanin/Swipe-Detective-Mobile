using DG.Tweening;
using TMPro;
using UnityEngine;

public class CoinRewardAnimator : MonoBehaviour
{
    [SerializeField] private GameObject pileOfCoins;
    [SerializeField] private GameObject[] coins;
    [SerializeField] private GameObject goldPanel;
    [SerializeField] private TMP_Text goldPanelText;
    [SerializeField] private GameObject targetPoint;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private int coinsPerCollection;

    private Vector2[] initialPositions;
    private Quaternion[] initialRotation;
    private int coinsAmount;

    void Awake()
    {
        coinsAmount = coins.Length;
    }

    public void CollectCoinsAnimation(Vector3 initialPos)
    {
        SetInitialPosition(initialPos);

        pileOfCoins.SetActive(true);
        audioSource.time = 0.5f;
        SFXManager.Instance.PlaySound(audioSource, SFXType.Coins);
        float delay = 0f;
        int coinsCollected = 0;
        int totalGold;

        // Calculate the longest animation duration to determine when to reset positions
        float totalAnimationDuration = (coinsAmount - 1) * 0.1f + 1.8f;

        Vector3 targetWorldPos = targetPoint.transform.position;
        Canvas canvas = GetComponentInParent<Canvas>();
        
        for (int i = 0; i < coinsAmount; i++)
        {
            Transform coinTransform = pileOfCoins.transform.GetChild(i);
            
            coinTransform.DOScale(1f, 0.3f).SetDelay(delay).SetEase(Ease.OutBack);

            int coinIndex = i;
            
            // Move to target position
            coinTransform.DOMove(targetWorldPos, 0.8f)
                .SetDelay(delay + 0.5f)
                .SetEase(Ease.InBack)
                .onComplete = () =>
                {
                    coinsCollected += coinsPerCollection;
                    totalGold = DataManager.Instance.GameData.gold + coinsCollected;
                    if (coinIndex == coinsAmount - 1)
                    {
                        DataManager.Instance.UpdateValue(Data.Gold, totalGold);
                    }
                };

            coinTransform.DORotate(Vector3.zero, 0.5f)
                .SetDelay(delay + 0.5f)
                .SetEase(Ease.Flash);

            coinTransform.DOScale(0f, 0.3f)
                .SetDelay(delay + 1.5f)
                .SetEase(Ease.OutBack);

            delay += 0.1f;
        }

        // Animate gold panel
        goldPanel.transform.DOScale(1.1f, 0.1f)
            .SetLoops(10, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetDelay(1.2f);

        DOVirtual.DelayedCall(totalAnimationDuration, ResetCoinsPositions);
    }

    private void ResetCoinsPositions()
    {
        for (int i = 0; i < coinsAmount; i++)
        {
            RectTransform coinTransform = pileOfCoins.transform.GetChild(i).GetComponent<RectTransform>();
            coinTransform.anchoredPosition = initialPositions[i];
            coinTransform.rotation = initialRotation[i];
        }
    }

    private void SetInitialPosition(Vector3 newInitialPos)
    {
        pileOfCoins.transform.position = newInitialPos;
        initialPositions = new Vector2[coinsAmount];
        initialRotation = new Quaternion[coinsAmount];

        for (int i = 0; i < coinsAmount; i++)
        {
            initialPositions[i] = pileOfCoins.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition;
            initialRotation[i] = pileOfCoins.transform.GetChild(i).GetComponent<RectTransform>().rotation;
        }
    }
}
