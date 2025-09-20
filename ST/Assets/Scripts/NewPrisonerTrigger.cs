using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPrisonerTrigger : MonoBehaviour
{
    public static NewPrisonerTrigger Instance { get; private set; }

    private Dictionary<string, int> disposableTransitionData;

    /// <summary>
    /// Will destroy itself after returning the transition data.
    /// </summary>
    public Dictionary<string, int> DisposableTransitionData
    {
        get
        {
            StartCoroutine(DestroySelfNextFrame());
            return disposableTransitionData;
        }
        set { disposableTransitionData = value; }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator DestroySelfNextFrame()
    {
        yield return new WaitForEndOfFrame();
        Destroy(gameObject);
    }
}
