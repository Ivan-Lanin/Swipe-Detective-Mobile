using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum TutorialState
{
    Welcome,
    Sorting,
    SortingHairlength,
    SortingHaircolor,
    SortingEyecolor,
    SortingExtraFeatures,
    PrisonersCount,
    SendPrisoners,
    BuildEmeraldRoom,
    EmeraldInASorting,
    Completed
}

[RequireComponent(typeof(AudioSource))]
public class TutorialStateManager : MonoBehaviour
{
    public TutorialBaseState currentState;
    public TutorialWelcomeState welcomeState = new TutorialWelcomeState();
    public TutorialSortingState sortingState = new TutorialSortingState();
    public TutorialSortingHairlengthState sortingHairlengthState = new TutorialSortingHairlengthState();
    public TutorialSortingHairColorState sortingHairColorState = new TutorialSortingHairColorState();
    public TutorialSortingEyeColorState sortingEyeColorState = new TutorialSortingEyeColorState();
    public TutorialSortingExtraState sortingExtraState = new TutorialSortingExtraState();
    public TutorialPrisonersCountState prisonersCountState = new TutorialPrisonersCountState();
    public TutorialSendPrisonersState sendPrisonersState = new TutorialSendPrisonersState();
    public TutorialBuildEmeraldRoomState buildEmeraldRoomState = new TutorialBuildEmeraldRoomState();
    public TutorialEmeraldInASortingState emeraldInASortingState = new TutorialEmeraldInASortingState();
    public TutorialCompletedState completedState = new TutorialCompletedState();

    public TutorialDepartmentReferences departmentReferences;
    public TutorialSortingReferences sortingReferences;
    public string currentLevelName;

    [Header("AudioClips")]
    [SerializeField] private AudioClip blaBlaBlaSound;
    [SerializeField] private AudioClip blaSound;

    private Dictionary<TutorialState, TutorialBaseState> stateDictionary;
    private AudioSource tutorialAudioSource;

    public static TutorialStateManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        tutorialAudioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ClearTutorialReferences();
        if (scene.name == "DepartmentLevel")
        {
            departmentReferences = TutorialDepartmentReferences.Instance;
        }
        else if (scene.name == "SuspectSortingLevel")
        {
            sortingReferences = TutorialSortingReferences.Instance;
        }
        else
        {
            Debug.LogError("Scene not recognized for tutorial references.");
            return;
        }
        currentLevelName = scene.name;
    }

    void Start()
    {
        InitializeStateDictionary();
        currentState = stateDictionary[DataManager.Instance.GameData.tutorialState];
        currentState.EnterState(this);
    }

    private void InitializeStateDictionary()
    {
        stateDictionary = new Dictionary<TutorialState, TutorialBaseState>
        {
            { TutorialState.Welcome, welcomeState },
            { TutorialState.PrisonersCount, prisonersCountState },
            { TutorialState.Sorting, sortingState },
            { TutorialState.SendPrisoners, sendPrisonersState },
            { TutorialState.BuildEmeraldRoom, buildEmeraldRoomState },
            { TutorialState.EmeraldInASorting, emeraldInASortingState },
            { TutorialState.Completed, completedState }
        };
    }

    private void ClearTutorialReferences()
    {
        departmentReferences = null;
        sortingReferences = null;
    }

    public void PlayBlaBlaBlaSound()
    {
        tutorialAudioSource.Stop();
        tutorialAudioSource.clip = blaBlaBlaSound;
        SFXManager.Instance.PlaySound(tutorialAudioSource, SFXType.Tutorial);
    }

    public void PlayBlaSound()
    {
        tutorialAudioSource.Stop();
        tutorialAudioSource.clip = blaSound;
        SFXManager.Instance.PlaySound(tutorialAudioSource, SFXType.Tutorial);
    }

    public void SwitchState(TutorialBaseState newState)
    {
        currentState.ExitState();
        currentState = newState;
        currentState.EnterState(this);
    }

    public Coroutine StartCoroutineFromState(IEnumerator routine)
    {
        return StartCoroutine(routine);
    }

    public void StopCoroutineFromState(Coroutine coroutine)
    {
        StopCoroutine(coroutine);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
