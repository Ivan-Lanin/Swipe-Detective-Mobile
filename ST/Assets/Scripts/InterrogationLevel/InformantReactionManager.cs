using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class InformantReactionManager : MonoBehaviour
{
    private int[] testimony = null;
    private bool isBullhead;
    private int reefusalIndex = -1;
    private int refusalCount = 0;
    private const int FEATURES_TO_CHOSE_FROM_COUNT = 3;

    private Animator animator;
    private InterrogationFeatureRenderer FeatureRenderer => InterrogationFeatureRenderer.Instance;
    private InterrogationControls InterrogationControls => InterrogationControls.Instance;
    private InformantsManager InformantsManager => InformantsManager.Instance;
    private InterrogationFeatureGenerator InterrogationFeatureGenerator => InterrogationFeatureGenerator.Instance;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetTestimonyOnce(int[] testimony)
    {
        if (this.testimony == null) this.testimony = testimony;
    }

    public void SetIsBullhead(bool isBullhead)
    {
        this.isBullhead = isBullhead;
        reefusalIndex = isBullhead ? UnityEngine.Random.Range(0, 3) : -1;
    }

    public void ReactOnFeature(InterrogationFeatureType featureType, GameObject feature)
    {
        int expectedIndex;
        if (isBullhead && (int)featureType == reefusalIndex)
        {
            expectedIndex = -1;
            refusalCount++;
            if (refusalCount >= FEATURES_TO_CHOSE_FROM_COUNT
                && !XRayManager.Instance.IsWorking)
            {
                InterrogationControls.SetUseXRayButtonActive(true);
            }
        }
        else
        {
            expectedIndex = testimony[(int)featureType];
        }

        int featureIndex = featureType switch
        {
            InterrogationFeatureType.Hair => FindIndexByName(InterrogationFeatureGenerator.Haircuts, feature),
            InterrogationFeatureType.EyeColor => FindIndexByName(InterrogationFeatureGenerator.EyesColors, feature),
            InterrogationFeatureType.Extra => FindIndexByName(InterrogationFeatureGenerator.Extras, feature),
            _ => -1
        };

        if (featureIndex == expectedIndex)
        {
            animator.SetTrigger("Yes");
        }
        else
        {
            animator.SetTrigger("No");
            if (XRayManager.Instance.IsWorking && featureIndex == testimony[(int)featureType])
            {
                XRayManager.Instance.SetHeartRate(BPMDirection.Up);
                animator.SetTrigger("RushHeartbeat");
            }
            else if (XRayManager.Instance.IsWorking && featureIndex != testimony[(int)featureType])
            {
                XRayManager.Instance.SetHeartRate(BPMDirection.Down);
                if (!animator.GetCurrentAnimatorStateInfo(1).IsName("CalmHeartbeat"))
                {
                    animator.SetTrigger("CalmHeartbeat");
                }
            }
        }
    }

    private int FindIndexByName(IReadOnlyList<GameObject> prefabs, GameObject instance)
    {
        string instanceName = instance.name.Replace("(Clone)", "").Trim();
        for (int i = 0; i < prefabs.Count; i++)
        {
            if (prefabs[i].name == instanceName)
                return i;
        }
        return -1;
    }
}