using UnityEngine;

public class FeaturesCameraUpdater : MonoBehaviour
{
    private Camera featuresCamera;

    private void Awake()
    {
        featuresCamera = GetComponent<Camera>();
    }

    private void Start()
    {
        SortingLevelManager.Instance.onNextRound.AddListener(RenderPas);
    }

    private void RenderPas()
    {
        featuresCamera.enabled = true;
        featuresCamera.Render();
        featuresCamera.enabled = false;
    }
}
