using System.Collections;
using UnityEngine;

public class AnimatorCullingController : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the GameObject. Please add an Animator component.");
        }
    }

    void Update()
    {
        if (!IsInvoking("CheckIfObjectIsOnScreen"))
        {
            StartCoroutine(CheckIfObjectIsOnScreen());
        }
    }

    private bool ObjectIsOnScreen()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        return screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1;
    }

    private IEnumerator CheckIfObjectIsOnScreen()
    {
        yield return new WaitForSeconds(0.5f);
        animator.enabled = ObjectIsOnScreen();
    }
}
