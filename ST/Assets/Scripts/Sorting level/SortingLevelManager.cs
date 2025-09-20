using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SortingLevelManager : MonoBehaviour
{
    public int Health { get; private set; } = 3;
    public int Emeralds { get; private set; } = 0;
    public GameState CurrentGameState { get; private set; }

    [SerializeField] private SuspectsManager suspectsManager;
    [SerializeField] private SuspectFeatureRenderer suspectFeatureRenderer;
    [SerializeField] private Button startButton;
    [SerializeField] private TMP_Text nextRoundText;
    [SerializeField] private GameObject nextRoundPanel;
    [SerializeField] private PlayResultPanel playResultPanel;
    [SerializeField] private PlayRevivePanel playRevivePanel;
    [SerializeField] private AudioClip loseSound;
    [SerializeField] private AudioClip winSound;

    public UnityEvent onLoose;
    public UnityEvent onWin;
    public UnityEvent onNextRound;
    public UnityEvent onEmeraldCollect;
    private AudioSource audioSource;

    private readonly Dictionary<GameState, int> roundSuspectCounts = new Dictionary<GameState, int>
    {
        { GameState.Round1, 8 },
        { GameState.Round2, 4 },
        { GameState.Round3, 2 }
    };

    public static SortingLevelManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        suspectsManager.onWrongGuess.AddListener(OnDamage);
        audioSource = GetComponent<AudioSource>();
        startButton.onClick.AddListener(AdvanceToNextRound);
    }

    void Start()
    {
        StartCoroutine(WaitForGeneration());
    }

    private void OnDamage()
    {
        Health--;
        if (Health < 0)
        {
            throw new System.Exception("Health cannot be less than 0");
        }
        else if (Health == 0)
        {
            HandleLose();
        }
    }

    public void OnHeal()
    {
        if (Health > 2) return;
        Health++;
    }

    public void OnEmeraldCollect()
    {
        Emeralds++;
        onEmeraldCollect.Invoke();
    }

    public void CheckIfRoundIsOver()
    {
        if (roundSuspectCounts.TryGetValue(CurrentGameState, out int expectedCount) &&
            suspectsManager.Suspects.Count == expectedCount)
        {
            AdvanceToNextRound();
            StartCoroutine(nameof(NextRoundAnimation));
            return;
        }

        if (CurrentGameState == GameState.Round4 && suspectsManager.Suspects.Count == 1)
        {
            if (suspectsManager.Suspects[0].name.Contains("0") == false)
            {
                HandleLose();
            }
            else
            {
                HandleWin();
            }
        }
    }

    private void AdvanceToNextRound()
    {
        switch (CurrentGameState)
        {
            case GameState.Start:
                StartRound1();
                break;
            case GameState.Round1:
                StartRound2();
                break;
            case GameState.Round2:
                StartRound3();
                break;
            case GameState.Round3:
                StartRound4();
                break;
        }
        onNextRound.Invoke();
    }

    public void OnStart()
    {
        CurrentGameState = GameState.Start;
        startButton.gameObject.SetActive(true);
    }

    private void StartRound1()
    {
        CurrentGameState = GameState.Round1;
        suspectFeatureRenderer.RenderFeatureHint(CurrentGameState);
        startButton.gameObject.SetActive(false);
        suspectsManager.NextSuspect();
    }

    private void StartRound2()
    {
        CurrentGameState = GameState.Round2;
        suspectFeatureRenderer.RenderFeatureHint(CurrentGameState);
    }

    private void StartRound3()
    {
        CurrentGameState = GameState.Round3;
        suspectFeatureRenderer.RenderFeatureHint(CurrentGameState);
    }

    private void StartRound4()
    {
        CurrentGameState = GameState.Round4;
        suspectFeatureRenderer.RenderFeatureHint(CurrentGameState);
    }

    private void HandleLose()
    {
        CurrentGameState = GameState.Loose;
        audioSource.clip = loseSound;
        SFXManager.Instance.PlaySound(audioSource, SFXType.SortingManager);
        playRevivePanel.Appear();
        onLoose.Invoke();
    }

    private void HandleWin()
    {
        CurrentGameState = GameState.Win;
        audioSource.clip = winSound;
        SFXManager.Instance.PlaySound(audioSource, SFXType.SortingManager);
        playResultPanel.Appear(suspectsManager.CorrectSuspect, Health, Emeralds);
        onWin.Invoke();
    }

    private IEnumerator WaitForGeneration()
    {
        yield return new WaitUntil(() => suspectsManager.Suspects.Count > 0);
        OnStart();
    }

    public IEnumerator NextRoundAnimation()
    {
        nextRoundPanel.SetActive(true);

        GameObject curSuspect = suspectsManager.GetCurrentSuspect();
        curSuspect.GetComponent<SuspectController>().Deactivate();

        switch (CurrentGameState)
        {
            case GameState.Round2:
                nextRoundText.text = "Hair color";
                break;
            case GameState.Round3:
                nextRoundText.text = "Eyes color";
                break;
            case GameState.Round4:
                nextRoundText.text = "Extra";
                break;
        }

        Color textColor = new Color(nextRoundText.color.r, nextRoundText.color.g, nextRoundText.color.b, 0);
        float fadeSpeed = 2.5f;
        // show next level text
        while (nextRoundText.color.a < 1)
        {
            nextRoundText.color = new Color(textColor.r, textColor.g, textColor.b, nextRoundText.color.a + Time.deltaTime * fadeSpeed);
            yield return null;
        }
        if (CurrentGameState == GameState.Loose)
        {
            yield break;
        }
        // hide next level text
        while (nextRoundText.color.a > 0)
        {
            nextRoundText.color = new Color(textColor.r, textColor.g, textColor.b, nextRoundText.color.a - Time.deltaTime * fadeSpeed);
            yield return null;
        }
        nextRoundPanel.SetActive(false);
        curSuspect.GetComponent<SuspectController>().Activate(suspectsManager);
    }

    private void OnDestroy()
    {
        suspectsManager.onWrongGuess.RemoveListener(OnDamage);
        startButton.onClick.RemoveListener(AdvanceToNextRound);
    }
}

public enum GameState
{
    Start,
    Round1,
    Round2,
    Round3,
    Round4,
    Loose,
    Win
}
