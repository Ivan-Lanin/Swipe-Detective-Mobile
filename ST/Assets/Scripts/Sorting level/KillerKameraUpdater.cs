using UnityEngine;

public class KillerKameraUpdater : MonoBehaviour
{
    private Camera killerCamera;

    private void Awake()
    {
        killerCamera = GetComponent<Camera>();
        killerCamera.enabled = false;
        SortingLevelManager.Instance.onWin.AddListener(EnableKillerCamera);
    }

    private void EnableKillerCamera()
    {
        killerCamera.enabled = true;
    }
}
