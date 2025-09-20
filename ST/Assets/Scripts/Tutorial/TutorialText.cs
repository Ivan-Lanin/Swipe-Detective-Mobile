using UnityEngine;

[CreateAssetMenu(fileName = "TutorialText", menuName = "Scriptable Objects/TutorialText")]
public class TutorialText : ScriptableObject
{
    [SerializeField] public string welcomeTextFirst;
    [SerializeField] public string welcomeTextSecond;
    [SerializeField] public string sortingTextFirst;
    [SerializeField] public string sortingTextSecond;
    [SerializeField] public string sortingHairLengthText;
    [SerializeField] public string sortingHairColorText;
    [SerializeField] public string sortingEyeColorText;
    [SerializeField] public string sortingExtraText;
    [SerializeField] public string prisonersCountText;
    [SerializeField] public string sendPrisonersTextFirst;
    [SerializeField] public string sendPrisonersTextSecond;
    [SerializeField] public string buildEmeraldRoomTextFirst;
    [SerializeField] public string buildEmeraldRoomTextSecond;
    [SerializeField] public string emeraldInASortingTextFirst;
    [SerializeField] public string emeraldInASortingTextSecond;
}
