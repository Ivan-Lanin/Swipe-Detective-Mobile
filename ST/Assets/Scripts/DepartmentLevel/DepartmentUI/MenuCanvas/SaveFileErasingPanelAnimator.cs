using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SaveFileErasingPanelAnimator : MonoBehaviour
{
    [SerializeField] Image panel;
    [SerializeField] Image saveFileIcon;
    [SerializeField] Image gearsIcon;

    public UnityEvent onAnimationComplete;

    private void OnEnable()
    {
        StartAnimation();
    }

    private void StartAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(panel.DOFade(0.3f, 0.5f).SetEase(Ease.InBack));
        sequence.Join(saveFileIcon.DOFade(1, 0.5f).SetEase(Ease.InBack));
        sequence.Join(gearsIcon.DOFade(1, 0.5f).SetEase(Ease.InBack));
        sequence.Append(saveFileIcon.rectTransform.DOAnchorPosX(0, 0.5f).SetEase(Ease.OutBack));
        sequence.Join(gearsIcon.rectTransform.DOAnchorPosX(0, 0.5f).SetEase(Ease.OutBack));
        sequence.Append(gearsIcon.transform.DORotate(new Vector3(0, 0, 720), 1.5f, RotateMode.FastBeyond360).SetEase(Ease.Linear));
        sequence.AppendCallback(() => onAnimationComplete?.Invoke());
        sequence.Play();
    }
}