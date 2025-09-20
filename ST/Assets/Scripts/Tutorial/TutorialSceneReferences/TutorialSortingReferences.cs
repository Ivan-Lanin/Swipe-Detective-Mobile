using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialSortingReferences : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject tutorialCanvas;
    public TMP_Text dialogueText;
    public GameObject dialogueFrame;
    public Slider rotationSlider;

    [Header("Controls")]
    public Button startButton;
    public Button dialogueContinueButton;
    public BeginDragTrigger wigArrowTrigger;
    public GameObject inputHandler;

    [Header("Sorting Hint Arrows")]
    public GameObject cluesArrow;
    public GameObject startArrow;
    public GameObject wigArrow;

    [Header("Script components")]
    public SuspectsManager suspectsManager;
    public SortingLevelManager sortingLevelManager;

    [Header("Animators")]
    public Animator swipeHintAnimator;
    public Animator dialogueAnimator;

    public static TutorialSortingReferences Instance { get; private set; }

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
