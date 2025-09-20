using System.Collections;
using UnityEngine;

public class RandomEventManager : MonoBehaviour
{
    [SerializeField] private GameObject[] randomEventPrefabs;
    [SerializeField] private Transform[] spawnPlanes;

    public static RandomEventManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        StartCoroutine(SpawnTimer());
    }

    private void SpawnRandomEvent()
    {
        Transform plane = spawnPlanes[Random.Range(0, spawnPlanes.Length)];
        Vector3 spawnPoint = GetPointOnPlane(plane);
        GameObject randomEventPrefab = randomEventPrefabs[Random.Range(0, randomEventPrefabs.Length)];
        GameObject randomEvent = Instantiate(randomEventPrefab, spawnPoint, Quaternion.identity);
        randomEvent.transform.SetParent(transform, false);
    }
    
    private Vector3 GetPointOnPlane(Transform plane)
    {
        Mesh mesh = plane.GetComponent<MeshFilter>().mesh;
        Bounds localBounds = mesh.bounds;
        
        float localX = Random.Range(localBounds.min.x, localBounds.max.x);
        float localZ = Random.Range(localBounds.min.z, localBounds.max.z);
        Vector3 localPoint = new Vector3(localX, localBounds.center.y, localZ);
        
        Vector3 worldPoint = plane.TransformPoint(localPoint);
        return worldPoint;
    }

    public void OnRandomEventDestroyed()
    {
        StartCoroutine(SpawnTimer());
    }

    private IEnumerator SpawnTimer()
    {
        while (DataManager.Instance.GetPrisonersCount() >= 4)
        {
            yield return new WaitForSeconds(2f);
        }


        yield return new WaitForSeconds(Random.Range(3f, 6f));
        SpawnRandomEvent();
    }
}
