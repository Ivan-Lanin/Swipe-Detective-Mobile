using UnityEngine;

public class MobileUtilsScript : MonoBehaviour
{
    [SerializeField] int targetFrameRate;

    void Awake()
    {
        Application.targetFrameRate = targetFrameRate;
    }
}