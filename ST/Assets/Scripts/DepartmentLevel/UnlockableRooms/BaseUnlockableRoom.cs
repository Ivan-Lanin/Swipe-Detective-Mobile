using DG.Tweening;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public abstract class BaseUnlockableRoom : MonoBehaviour
{
    [SerializeField] public Button buildButton;
    [SerializeField] public ParticleSystem BuildButtonVFX;
    [SerializeField] public TMP_Text buildPriceText;
    [SerializeField] protected GameObject unlocked;
    [SerializeField] protected GameObject blueprint;
    [SerializeField] protected GameObject lockedParts;
    [SerializeField] protected GameObject buildIcon;
    [SerializeField] protected AnimationCurve buildIconAnimationCurve;

    protected Animator animator;
    protected Vector2 buildIconAnimationScaleTarget = new Vector2(2.5f, 2.5f);
    

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public virtual void ShowBlueprint()
    {
        blueprint.SetActive(true);
        lockedParts.SetActive(true);
        unlocked.SetActive(false);
    }
    public virtual void ShowRoom(int level)
    {
        if (level <= 0)
        {
            Debug.LogError("Invalid level for Evidence Room: " + level);
            return;
        }
        unlocked.SetActive(true);
        lockedParts.SetActive(false);
        blueprint.SetActive(false);
    }
    public virtual void BuildRoom()
    {
        Vector3 originalHammerIconsScale = buildIcon.transform.localScale;

        buildButton.interactable = false;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(buildIcon.transform.DOLocalRotate(new Vector3(0, 0, 360), 0.4f, RotateMode.FastBeyond360));
        sequence.Join(buildIcon.transform.DOPunchScale(new Vector3(1.5f, 1.5f, 0), 0.4f, 1, 0.5f));
        sequence.Append(buildButton.transform.parent.DOScale(Vector3.zero, 0.15f));

        sequence.Play().onComplete += () =>
        {
            BuildButtonVFX.Play();
            blueprint.SetActive(false);
            animator.SetTrigger("Build");
            unlocked.SetActive(true);
        };
    }
}
