using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class HealthPanel : MonoBehaviour
{
    [SerializeField] private Image heart1;
    [SerializeField] private Image heart2;
    [SerializeField] private Image heart3;
    [SerializeField] private SuspectsManager suspectsManager;

    private int health = 3;
    private Animator animator;

    public void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Start()
    {
        suspectsManager.onWrongGuess.AddListener(OnDamage);
    }

    public void OnDamage()
    {
        health = Mathf.Max(0, health - 1);
        animator.ResetTrigger("HeartDrop");
        animator.SetTrigger("HeartDrop");
    }

    public void OnHeal()
    {
        animator.ResetTrigger("HeartHeal");
        animator.SetTrigger("HeartHeal");
        health = Mathf.Min(3, health + 1);
    }

    void OnDestroy()
    {
        suspectsManager.onWrongGuess.RemoveListener(OnDamage);
    }
}
