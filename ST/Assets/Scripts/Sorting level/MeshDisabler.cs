using UnityEngine;

public class MeshDisabler : MonoBehaviour
{
    private Collider trigger;

    void Awake()
    {
        trigger = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Disable all of the childrens of the other object except the collider
        foreach (Transform child in other.transform)
        {
            if (child != other.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Enable all of the childrens of the other object except the collider
        foreach (Transform child in other.transform)
        {
            if (child != other.transform)
            {
                child.gameObject.SetActive(true);
            }
        }
    }
}
