using System.Collections.Generic;
using UnityEngine;

public class SuspectCollectionManager : MonoBehaviour
{
    [SerializeField] private GameObject suspectPrefab;
    
    public List<GameObject> Suspects { get; private set; } = new List<GameObject>();
    public GameObject CorrectSuspect { get; private set; }

    private const int suspectsCount = 16;
    private readonly string[] names =
        { "Alice", "Bob", "Charlie", "David", "Eve", "Frank", "Grace", "Hank", "Ivy", "Jack", "Kate", "Liam", "Mia", "Nina", "Owen", "Pam", "Quinn", "Rex",
                "Sara", "Tom", "Uma", "Vince", "Wendy", "Xander", "Yara", "Zane", "Zoe", "Alex", "Ben", "Cody", "Diana", "Ella", "Finn", "Gina", "Holly", "Ian", 
                "Jill", "Kurt", "Lana", "Max", "Nora", "Oscar", "Penny", "Quincy", "Riley", "Sam", "Tina", "Ulysses", "Violet", "Walter", "Xena", "Yuri", "Zara",
                "Zack", "Ivan", "Johan", "Klaus"
            };
    private readonly string[] surnames =
        { "Smith", "Johnson", "Williams", "Jones", "Brown", "Davis", "Miller", "Wilson", "Moore", "Taylor", "Anderson", "Thomas", "Jackson",
                "White", "Harris", "Martin", "Thompson", "Garcia", "Martinez", "Robinson", "Clark", "Rodriguez", "Lewis", "Lee", "Walker", "Hall", "Allen", 
                "Lanin", "King", "Scott", "Green", "Baker", "AdAMS", "Nelson", "Hill", "Ramirez", "Campbell", "Mitchell", "Roberts", "Carter", "Phillips"
            };

    public void GenerateSuspects()
    {
        Suspects.Clear();
        for (int i = 0; i < suspectsCount; i++)
        {
            GameObject suspect = Instantiate(suspectPrefab, transform);
            suspect.name = i + " " + names[Random.Range(0, names.Length)] + " " + surnames[Random.Range(0, surnames.Length)];
            Suspects.Add(suspect);
        }
        CorrectSuspect = Suspects.Count > 0 ? Suspects[0] : null;
    }

    public void DeleteSuspect(GameObject suspect)
    {
        if (Suspects.Contains(suspect))
        {
            Suspects.Remove(suspect);
            Destroy(suspect);
        }
    }

    public void ShuffleSuspects()
    {
        int n = Suspects.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            GameObject temp = Suspects[i];
            Suspects[i] = Suspects[j];
            Suspects[j] = temp;
        }
    }
}