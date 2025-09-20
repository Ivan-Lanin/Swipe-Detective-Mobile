using TMPro;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SuspectsManager : MonoBehaviour
{
    public SuspectCollectionManager suspectCollectionManager;
    public SuspectFeatureService suspectFeatureService;
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private GameObject emeraldPrefab;
    [SerializeField] private Transform suspectViewAnchor;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TMP_Text progressCount;

    public List<GameObject> Suspects => suspectCollectionManager.Suspects;
    public GameObject CorrectSuspect => suspectCollectionManager.CorrectSuspect;
    public List<GameObject> SuspectsTemp { get; private set; }
    public int CurrentSuspectIndex { get; private set; } = 999;
    public Dictionary<GameObject, Animator> CachedAnimators { get; private set; } = new Dictionary<GameObject, Animator>();

    public UnityEvent onWrongGuess;
    public UnityEvent onCorrectGuess;
    public UnityEvent onNextSuspect;

    private const float ChanceToGetHeart = 0.25f;
    private const float ChanceToGetEmerald = 0.09f;

    void Awake()
    {
        SuspectsTemp = new List<GameObject>();
    }

    void Start()
    {
        SortingLevelManager.Instance.onWin.AddListener(StartKillerAngryAnimation);

        // Use SuspectCollectionManager to generate suspects
        suspectCollectionManager.GenerateSuspects();
        
        // Use SuspectFeatureService to generate features
        suspectFeatureService.InstantiateRandomFeatures(Suspects);

        // Use SuspectCollectionManager to shuffle suspects
        suspectCollectionManager.ShuffleSuspects();
        
        // Initialize SuspectsTemp with the generated suspects
        SuspectsTemp = new List<GameObject>(Suspects);
    }

    private void StartKillerAngryAnimation()
    {
        if (CurrentSuspectIndex < 0 || CurrentSuspectIndex >= SuspectsTemp.Count)
            return;

        Animator animator = GetSuspectAnimator(SuspectsTemp[CurrentSuspectIndex]);
        if (animator == null)
        {
            StartCoroutine(WaitForKillerAnimator());
        }
        else
        {
            animator.SetTrigger("Angry");
        }
    }

    public GameObject GetCurrentSuspect()
    {
        return SuspectsTemp[CurrentSuspectIndex];
    }

    public void DeleteSuspect(GameObject suspect)
    {
        suspectCollectionManager.DeleteSuspect(suspect);
        SortingLevelManager.Instance.CheckIfRoundIsOver();
        progressBar.DOValue(progressBar.value + 1f, 0.5f).SetEase(Ease.InOutQuad);
        progressCount.transform.DOShakeScale(0.5f, 0.3f, 10, 90, false)
            .OnComplete(() => progressCount.text = (16 - progressBar.value).ToString());
    }

    public void NextSuspect()
    {
        if (SuspectsTemp.Count == 0) return;

        int attempts = 0;
        int maxAttempts = SuspectsTemp.Count;
        
        do
        {
            if (CurrentSuspectIndex < SuspectsTemp.Count - 1)
            {
                CurrentSuspectIndex++;
            }
            else
            {
                CurrentSuspectIndex = 0;
                SuspectsTemp = new List<GameObject>(Suspects.Where(s => s != null));
                if (SuspectsTemp.Count == 0) return;
            }
            attempts++;
        }
        while (SuspectsTemp[CurrentSuspectIndex] == null && attempts < maxAttempts);

        if (SuspectsTemp[CurrentSuspectIndex] == null)
        {
            Debug.LogError("No valid suspects found!");
            return;
        }

        RemoveAllCollectibles(SuspectsTemp[CurrentSuspectIndex].transform);
        SuspectsTemp[CurrentSuspectIndex].GetComponent<SuspectController>().Activate(this);
        SuspectsTemp[CurrentSuspectIndex].transform.position = suspectViewAnchor.position;
        GetSuspectAnimator(SuspectsTemp[CurrentSuspectIndex]);
        InstantiateRandomCollectibles(SuspectsTemp[CurrentSuspectIndex].transform);
        onNextSuspect?.Invoke();
    }

    private Animator GetSuspectAnimator(GameObject suspect)
    {
        if (!CachedAnimators.TryGetValue(suspect, out Animator animator))
        {
            animator = suspect.GetComponentInChildren<Animator>();
            CachedAnimators[suspect] = animator;
        }
        return animator;
    }

    private void InstantiateRandomCollectibles(Transform currentSuspect)
    {
        if (Suspects.Count == 2 && currentSuspect.name.Contains('0')) return;

        float randomValue = Random.value;

        if (randomValue <= ChanceToGetHeart && SortingLevelManager.Instance.Health < 3)
        {
            AddHeart(currentSuspect);
        }

        else if (randomValue <= ChanceToGetEmerald && SortingLevelManager.Instance.Health >= 3)
        {
            AddEmerald(currentSuspect);
        }
    }

    private void RemoveAllCollectibles(Transform suspect)
    {
        if (suspect.GetComponentInChildren<Heart>() != null)
        {
            Destroy(suspect.GetComponentInChildren<Heart>().gameObject);
        }
        if (suspect.GetComponentInChildren<Emerald>() != null)
        {
            Destroy(suspect.GetComponentInChildren<Emerald>().gameObject);
        }
    }

    private void AddHeart(Transform suspect)
    {
        Instantiate(heartPrefab, suspect);
    }

    private void AddEmerald(Transform suspect)
    {
        if (DataManager.Instance.GameData.evidenceRoomLevel < 1) return;
        Instantiate(emeraldPrefab, suspect);
    }

    public bool CheckForCorrectGuess(bool guess)
    {
        bool result = false;

        switch (SortingLevelManager.Instance.CurrentGameState)
        {
            case GameState.Round1:
                result = CheckHairLength(guess);
                break;
            case GameState.Round2:
                result = CheckHairColor(guess);
                break;
            case GameState.Round3:
                result = CheckEyesColor(guess);
                break;
            case GameState.Round4:
                result = CheckExtra(guess);
                break;
        }

        if (result == false)
        {
            if (DataManager.Instance.GameData.tutorialState == TutorialState.Welcome) return false;
            onWrongGuess?.Invoke();
        }
        else if (result == true)
        {
            onCorrectGuess?.Invoke();
        }

        return result;
    }

    private bool CheckHairLength(bool guess)
    {
        (string objectToCheck, string referenceObject) = suspectFeatureService.GetHairLengths(
            SuspectsTemp[CurrentSuspectIndex], CorrectSuspect);
        objectToCheck = objectToCheck.Contains("wig") ? "no hair" : objectToCheck;
        referenceObject = referenceObject.Contains("wig") ? "no hair" : referenceObject;
        return objectToCheck == referenceObject ? guess : !guess;
    }

    private bool CheckHairColor(bool guess)
    {
        (string objectToCheck, string referenceObject) = suspectFeatureService.GetHairColors(
            SuspectsTemp[CurrentSuspectIndex], CorrectSuspect);
        return objectToCheck == referenceObject ? guess : !guess;
    }

    private bool CheckEyesColor(bool guess)
    {
        (string objectToCheck, string referenceObject) = suspectFeatureService.GetEyeColors(
            SuspectsTemp[CurrentSuspectIndex], CorrectSuspect);
        return objectToCheck == referenceObject ? guess : !guess;
    }

    private bool CheckExtra(bool guess)
    {
        (string objectToCheck, string referenceObject) = suspectFeatureService.GetExtras(
            SuspectsTemp[CurrentSuspectIndex], CorrectSuspect);
        return objectToCheck == referenceObject ? guess : !guess;
    }

    private void OnDestroy()
    {
        SortingLevelManager.Instance.onWin.RemoveListener(StartKillerAngryAnimation);
    }

    IEnumerator WaitForKillerAnimator()
    {
        yield return new WaitForSeconds(0.02f);
        StartKillerAngryAnimation();
    }
}
