using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialDepartmentReferences : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject tutorialPanel;
    public Image tutorialPanelImage;
    public GameObject tutorialCanvas;
    public TMP_Text dialogueText;
    public GameObject dialogueFrame;
    public GameObject toTheSortingLevelCanvas;
    public GameObject toTheInterrogationLevelCanvas;
    public Image dialogueIcon;
    public GameObject menuCanvas;

    [Header("Controls")]
    public Button dialogueContinueButton;
    public Button toTheSortingLevelButton;
    public Button sendPrisonersButton;
    public Button buildEmeraldRoomButton;
    public MainCameraController cameraController;

    [Header("Animators")]
    public Animator dialogueAnimator;

    public static TutorialDepartmentReferences Instance { get; private set; }

    private void Awake()
    {
        InitializeSingleton();
    }

    public void InitializeSingleton()
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
    }
}
