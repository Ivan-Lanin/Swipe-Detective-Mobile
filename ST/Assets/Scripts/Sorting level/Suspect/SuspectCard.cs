using TMPro;
using UnityEngine;

public class SuspectCard : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text numberText;
    [SerializeField] private TMP_Text dateText;
    [SerializeField] private GameObject parentObject;

    void Start()
    {
        string name = parentObject.name;
        string[] nameParts = name.Split(' ');
        // extract the name from the parent object's name if it contains more than two parts
        if (nameParts.Length > 2)
        {
            name = nameParts[1] + ' ' + nameParts[2];
        }
        nameText.text = name;
        dateText.text = System.DateTime.UtcNow.ToString("dd/MM");
        numberText.text = Random.Range(10000, 99999).ToString();
    }
}
