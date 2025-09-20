using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class PrisonerWandering : MonoBehaviour
{
    private GameObject targetBoundariesCube;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private bool isWaiting = false;

    private void Awake()
    {
        targetBoundariesCube = GameObject.Find("PrisonersWanderingZone");
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        Wander();
    }

    private void Update()
    {
        if (!isWaiting && navMeshAgent.remainingDistance < 0.5f)
        {
            StartCoroutine(WaitForNextWanderTarget());
        }
    }

    private Vector3 SetRandomTarget()
    {
        // Get the actual size of the box collider
        Vector3 size = targetBoundariesCube.GetComponent<BoxCollider>().size;

        // Generate a random point within the collider bounds (in local space)
        Vector3 randomPoint = new Vector3(
            Random.Range(-size.x / 2, size.x / 2),
            Random.Range(-size.y / 2, size.y / 2),
            Random.Range(-size.z / 2, size.z / 2)
        );

        // Convert to world space
        Vector3 worldPoint = targetBoundariesCube.transform.TransformPoint(randomPoint);
        return worldPoint;
    }

    private void Wander()
    {
        Vector3 destination = SetRandomTarget();
        navMeshAgent.SetDestination(destination);
        animator.SetBool("isWalking", true);
    }

    private IEnumerator WaitForNextWanderTarget()
    {
        isWaiting = true;
        animator.SetBool("isWalking", false);
        yield return new WaitForSeconds(2f);
        Wander();
        isWaiting = false; // Reset flag after wandering
    }
}
