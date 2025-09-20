using UnityEngine;

[ExecuteInEditMode]
public class VignettePostProcess : MonoBehaviour
{
    public Material vignetteMaterial;

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (vignetteMaterial != null)
        {
            Graphics.Blit(src, dest, vignetteMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
