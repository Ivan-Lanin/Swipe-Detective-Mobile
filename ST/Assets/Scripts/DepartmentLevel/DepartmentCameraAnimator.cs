using DG.Tweening;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(MainCameraController))]
public class DepartmentCameraAnimator : MonoBehaviour
{
    private Vector3 prisonVeiewAnchor = new Vector3(14.8100004f, 44.5499992f, -55.4599991f);
    private Vector3 evidenceRoomViewAnchor = new Vector3(1.35500121f, 48.5f, -63.4249687f);
    private MainCameraController mainCameraController;
    private Camera cameraComponent;
    private Tween currentSizeTween;

    public bool isCameraMoving { get; private set; } = false;

    public static DepartmentCameraAnimator Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        mainCameraController = GetComponent<MainCameraController>();
        cameraComponent = GetComponent<Camera>();
        
        if (NewPrisonerTrigger.Instance != null)
        {
            StartCoroutine(SmoothMovement(prisonVeiewAnchor));
        }
    }

    public void MoveToPrisonView()
    {
        StartCoroutine(SmoothMovement(prisonVeiewAnchor));
    }

    public void MoveToEvidenceRoomView()
    {
        StartCoroutine(SmoothMovement(evidenceRoomViewAnchor));
    }

    private IEnumerator SmoothMovement(Vector3 targetPosition)
    {
        if (currentSizeTween != null && currentSizeTween.IsActive())
        {
            currentSizeTween.Kill();
        }

        mainCameraController.enabled = false;
        isCameraMoving = true;

        currentSizeTween = cameraComponent.DOOrthoSize(10f, 1f).SetEase(Ease.InOutQuad);

        float duration = 1.5f;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(startPosition, targetPosition, smoothT);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;

        isCameraMoving = false;

        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(0.7f);
        sequence.Append(currentSizeTween = cameraComponent.DOOrthoSize(18f, 1f).SetEase(Ease.InOutQuad).SetEase(Ease.Linear));
        sequence.AppendCallback(() =>
        {
            mainCameraController.enabled = true;
        });
        sequence.Play();
    }

    public void ZoomOnTransition()
    {
        mainCameraController.enabled = false;
        cameraComponent.DOOrthoSize(8f, 0.5f).SetEase(Ease.InOutQuad).SetEase(Ease.Linear);
    }

    private void OnDisable()
    {
        if (currentSizeTween != null && currentSizeTween.IsActive())
        {
            currentSizeTween.Kill();
        }
    }
}
