using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ RequireComponent(typeof(CoinRewardAnimator))]
public class DailyReward : MonoBehaviour
{
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private Button dailyRewardButton;
    [SerializeField] private GameObject chestIcon;
    [SerializeField] private GameObject animationTargetAnchor;
    [SerializeField] private Image menuPanel;
    [SerializeField] private Image chestImage;
    [SerializeField] private Sprite chestOpened;
    [SerializeField] private Sprite chestClosed;

    private CoinRewardAnimator coinRewardAnimator;
    private bool isRewardCollected = false;

    private void Awake()
    {
        dailyRewardButton.onClick.AddListener(CollectDilyReward);
        coinRewardAnimator = GetComponent<CoinRewardAnimator>();
    }

    private void Start()
    {
        StartCoroutine(NextRevardCountDown());
    }

    private void CollectDilyReward()
    {
        if (isRewardCollected)
        {
            Debug.Log("Daily reward already collected today.");
            return;
        }
        isRewardCollected = true;
        DataManager.Instance.UpdateValue(Data.LastDailyRewardDate, DateTime.UtcNow.ToString("yyyy-MM-dd"));
        StartCoroutine(NextRevardCountDown());
        DoDailyChestAnimation();
    }

    private void DoDailyChestAnimation()
    {
        Vector3 initialPos = chestIcon.GetComponent<RectTransform>().position;
        Vector3 targetWorldPos = animationTargetAnchor.transform.position;
        menuPanel.gameObject.SetActive(true);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(menuPanel.DOFade(0.75f, 0.5f).SetEase(Ease.InOutQuad));
        sequence.Join(chestIcon.GetComponent<RectTransform>().DOMove(targetWorldPos, 0.5f).SetEase(Ease.OutBack));
        sequence.Join(chestIcon.GetComponent<RectTransform>().DOScale(2f, 0.5f).SetEase(Ease.OutBack));
        sequence.Join(chestIcon.GetComponent<RectTransform>().DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.OutBack));
        sequence.Append(chestIcon.GetComponent<RectTransform>().DOScale(2.5f, 0.25f).SetEase(Ease.OutBack));
        sequence.AppendCallback(() =>
        {
            chestImage.sprite = chestOpened;
            coinRewardAnimator.CollectCoinsAnimation(targetWorldPos);
        });
        sequence.Append(chestIcon.GetComponent<RectTransform>().DOScale(2f, 0.25f).SetEase(Ease.OutBack));

        sequence.Append(chestIcon.GetComponent<RectTransform>().DOMove(initialPos, 0.5f).SetEase(Ease.OutBack));
        sequence.Join(chestIcon.GetComponent<RectTransform>().DOScale(1f, 0.5f).SetEase(Ease.OutBack));
        sequence.Join(chestIcon.GetComponent<RectTransform>().DORotate(new Vector3(0, 0, -360), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.OutBack));
        sequence.Join(menuPanel.DOFade(0f, 0.5f).SetEase(Ease.InOutQuad));
        sequence.AppendCallback(() =>
        {
            menuPanel.gameObject.SetActive(false);
        });
        sequence.Play();
    }

    private IEnumerator NextRevardCountDown()
    {
        DateTime lastRewardDate = DateTime.Parse(DataManager.Instance.GameData.lastDailyRewardDate);
        if (lastRewardDate.Date < DateTime.UtcNow.Date)
        {
            countdownText.text = "COLLECT!";
            chestImage.sprite = chestClosed;
            isRewardCollected = false;
            yield break;
        }

        TimeSpan timeUntilNextReward = lastRewardDate.AddDays(1) - DateTime.UtcNow;
        while (timeUntilNextReward.TotalSeconds > 0)
        {
            timeUntilNextReward = lastRewardDate.AddDays(1) - DateTime.UtcNow;
            timeUntilNextReward = timeUntilNextReward.Add(TimeSpan.FromMinutes(1));
            countdownText.text = $"{timeUntilNextReward.Hours:D1}h:{timeUntilNextReward.Minutes:D1}m";
            chestImage.sprite = chestOpened;
            isRewardCollected = true;
            yield return new WaitForSeconds(60f);
        }
    }

    private void OnDestroy()
    {
        dailyRewardButton.onClick.RemoveListener(CollectDilyReward);
    }
}
