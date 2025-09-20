using System.Collections.Generic;
using UnityEngine;

public class SuspectFeatures : ScriptableObject
{
    public GameObject hairLength;
    public GameObject hairColor;
    public GameObject eyes;
    public GameObject extra;

    public Dictionary<string, GameObject> features = new Dictionary<string, GameObject>();
}
