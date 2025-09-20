using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ECGAnimator))]
public class XRayManager : MonoBehaviour
{
    private InterrogationControls controls;
    private Animator animator;
    private ECGAnimator ecgAnimator;
    private bool isWorking = false;

    public bool IsWorking => isWorking;

    public static XRayManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        animator = GetComponent<Animator>();
        ecgAnimator = GetComponent<ECGAnimator>();
    }

    private void Start()
    {
        controls = InterrogationControls.Instance;
        controls.onuseXRay.AddListener(ToggleXRayActive);
        controls.oncatchALie.AddListener(CatchALie);
        controls.onNextFeatureType.AddListener(OnNext);
        controls.onNextInformant.AddListener(OnNext);
    }

    private void CatchALie()
    {
        if (!isWorking)
        {
            return;
        }
        if (ecgAnimator.IsLying)
        {
            ToggleXRayActive();
        }
    }

    private void OnNext()
    {
        if (isWorking)
        {
            ToggleXRayActive();
        }
    }

    private void ToggleXRayActive()
    {
        isWorking = !isWorking;
        if (isWorking)
        {
            ecgAnimator.gameObject.SetActive(true);
        }
        animator.SetBool("isXRayActive", !animator.GetBool("isXRayActive"));
        if (!isWorking)
        {
            StartCoroutine(ResetAndDisable());
        }
    }

    public void SetHeartRate(BPMDirection bpm)
    {
        if (!isWorking)
        {
            return;
        }

        ecgAnimator.SetIsLying(bpm == BPMDirection.Up);
    }

    private IEnumerator ResetAndDisable()
    {
        ecgAnimator.SetIsLying(false);
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            yield return null;
        }
        ecgAnimator.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (controls != null)
        {
            controls.onuseXRay.RemoveListener(ToggleXRayActive);
            controls.oncatchALie.RemoveListener(CatchALie);
            controls.onNextFeatureType.RemoveListener(OnNext);
            controls.onNextInformant.RemoveListener(OnNext);
        }
    }
}

public enum BPMDirection
{
    Up,
    Down
}
