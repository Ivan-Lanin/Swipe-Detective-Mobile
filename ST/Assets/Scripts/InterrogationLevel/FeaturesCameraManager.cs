using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FeaturesCameraManager : MonoBehaviour
{
    [SerializeField] private RenderTexture featuresUITexture;
    [SerializeField] private RenderTexture firstTestimonyTexture;
    [SerializeField] private RenderTexture secondTestimonyTexture;
    [SerializeField] private RenderTexture thirdTestimonyTexture;

    private Camera featuresCamera;

    public static FeaturesCameraManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        featuresCamera = GetComponent<Camera>();
        featuresCamera.enabled = false;
    }

    public void RenderTextureForUI()
    {
        featuresCamera.targetTexture = featuresUITexture;
        featuresCamera.enabled = true;
        featuresCamera.Render();
        featuresCamera.enabled = false;
    }

    public void RenderTextureForTestimony(int testimonyNumber)
    {
        switch (testimonyNumber)
        {
            case 0:
                featuresCamera.targetTexture = firstTestimonyTexture;
                break;
            case 1:
                featuresCamera.targetTexture = secondTestimonyTexture;
                break;
            case 2:
                featuresCamera.targetTexture = thirdTestimonyTexture;
                break;
            default:
                Debug.LogError("Invalid testimony number");
                break;
        }

        featuresCamera.enabled = true;
        featuresCamera.Render();
        featuresCamera.enabled = false;
    }
}
