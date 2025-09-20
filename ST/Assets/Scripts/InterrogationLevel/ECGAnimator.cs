using DG.Tweening;
using TMPro;
using UnityEngine;

public class ECGAnimator : MonoBehaviour
{
    [SerializeField] private GameObject[] ecgLongSegments;
    [SerializeField] private GameObject[] ecgShortSegments;
    [SerializeField] private Transform ecgRightFrame;
    [SerializeField] private TMP_Text heartRateText;
    [SerializeField] private float scrollSpeed = 2f;

    private GameObject[] currentSegments;
    private int currentLongSegmentIndex = 0;
    private int currentShortSegmentIndex = 0;
    private Color originalHeartRateColor;
    private Color highHeartRateColor = new Color(0.772f, 0.145f, 0.129f, 1f);
    private bool isLying;
    public bool IsLying => isLying;

    private void Awake()
    {
        currentSegments = (GameObject[])ecgLongSegments.Clone();
        currentLongSegmentIndex = 2;
        originalHeartRateColor = heartRateText.color;
    }

    private void FixedUpdate()
    {
        AnimateECG();
    }

    private void AnimateECG()
    {
        if (currentSegments[2].transform.localPosition.z <= ecgRightFrame.localPosition.z)
        {
            if (isLying)
            {
                if (currentShortSegmentIndex == ecgShortSegments.Length - 1) currentShortSegmentIndex = 0;
                else currentShortSegmentIndex += 1;
                currentSegments[0] = currentSegments[1];
                currentSegments[1] = currentSegments[2];
                currentSegments[2] = ecgShortSegments[currentShortSegmentIndex];
            }
            else
            {
                if (currentLongSegmentIndex == ecgLongSegments.Length - 1) currentLongSegmentIndex = 0;
                else currentLongSegmentIndex += 1;
                currentSegments[0] = currentSegments[1];
                currentSegments[1] = currentSegments[2];
                currentSegments[2] = ecgLongSegments[currentLongSegmentIndex];
            }

            Renderer renderer1 = currentSegments[1].GetComponent<Renderer>();
            float renderer1SizeX = renderer1.bounds.size.x;
            Renderer renderer2 = currentSegments[2].GetComponent<Renderer>();
            float renderer2SizeX = renderer2.bounds.size.x;

            currentSegments[2].transform.position = new Vector3(
                currentSegments[1].transform.position.x + (renderer1SizeX / 2) + (renderer2SizeX / 2),
                currentSegments[1].transform.position.y,
                currentSegments[1].transform.position.z);

        }

        foreach (var segment in currentSegments)
        {
            segment.transform.position += Vector3.left * scrollSpeed * Time.deltaTime;
        }
    }

    public void SetIsLying(bool isLying)
    {
        this.isLying = isLying;
        if (isLying) AnimateHeartRate(105f);
        else AnimateHeartRate(85f);
    }

    private void AnimateHeartRate(float targetBPM)
    {
        float currentBPM;
        float.TryParse(heartRateText.text.Split(' ')[0], out currentBPM);
        if (currentBPM == targetBPM)
        {
            return;
        }

        Color currentColor = heartRateText.color;
        Color targetColor = currentBPM == 85f ? highHeartRateColor : originalHeartRateColor;

        float duration = 1f;

        DOTween.To(() => currentBPM, x => currentBPM = x, targetBPM, duration).OnUpdate(() =>
        {
            heartRateText.text = Mathf.RoundToInt(currentBPM).ToString() + " bpm";
        });

        heartRateText.DOColor(targetColor, duration);
    }
}