using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class EULAAnimationController : MonoBehaviour
{
    [SerializeField] private Button termsButton;

    private Animator animator;
    private bool isOut = false;

    private void Awake()
    {
        termsButton.onClick?.AddListener(OnTermsButtonClicked);
        animator = GetComponent<Animator>();
    }

    private void OnTermsButtonClicked()
    {
        isOut = !isOut;
        animator.SetBool("IsOut", isOut);
    }

    private void OnDestroy()
    {
        termsButton.onClick?.RemoveListener(OnTermsButtonClicked);
    }
}
