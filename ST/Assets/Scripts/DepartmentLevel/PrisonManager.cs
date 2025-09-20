using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;

[RequireComponent(typeof(CoinRewardAnimator))]
public class PrisonManager : MonoBehaviour
{
    [SerializeField] private PrisonerSpawner prisonerSpawner;
    [SerializeField] private GameObject prison;
    [SerializeField] private TMP_Text prisonerCounterText;
    [SerializeField] private Animator prisonersCanvasAnimator;
    [SerializeField] private Button sendButton;
    [SerializeField] private ParticleSystem despawnVFX;
    

    private Dictionary<string, int> firstPrisoner = null;
    private Dictionary<string, int> secondPrisoner = null;
    private Dictionary<string, int> thirdPrisoner = null;
    private Dictionary<string, int> fourthPrisoner = null;
    private Dictionary<string, int>[] prisoners = new Dictionary<string, int>[4];
    private CoinRewardAnimator coinRewardAnimator;

    private void Awake()
    {
        coinRewardAnimator = GetComponent<CoinRewardAnimator>();
        sendButton.onClick.AddListener(OnSendButtonPressed);

        if (DataManager.Instance.GetPrisonersCount() == 0)
        {
            return;
        }

        if (DataManager.Instance.GameData.firstPrisoner != null)
        {
            firstPrisoner = DataManager.Instance.GameData.GetPrisonerFeatures(1);
            prisoners[0] = firstPrisoner;
        }
        if (DataManager.Instance.GameData.secondPrisoner != null)
        {
            secondPrisoner = DataManager.Instance.GameData.GetPrisonerFeatures(2);
            prisoners[1] = secondPrisoner;
        }
        if (DataManager.Instance.GameData.thirdPrisoner != null)
        {
            thirdPrisoner = DataManager.Instance.GameData.GetPrisonerFeatures(3);
            prisoners[2] = thirdPrisoner;
        }
        if (DataManager.Instance.GameData.fourthPrisoner != null)
        {
            fourthPrisoner = DataManager.Instance.GameData.GetPrisonerFeatures(4);
            prisoners[3] = fourthPrisoner;
        }

        prisonerSpawner.SpawnPrisonersFromData(prisoners);
        UpdatePrisonerCountLabel();
    }

    public void AddNewPrisoner(Dictionary<string, int> prisonerFeatures)
    {
        Data prisonerNumber;
        Dictionary<string, int> features;
        switch (DataManager.Instance.GetPrisonersCount())
        {
            case 0:
                firstPrisoner = prisonerFeatures;
                prisoners[0] = firstPrisoner;
                prisonerNumber = Data.FirstPrisoner;
                features = firstPrisoner;
                break;
            case 1:
                secondPrisoner = prisonerFeatures;
                prisoners[1] = secondPrisoner;
                prisonerNumber = Data.SecondPrisoner;
                features = secondPrisoner;
                break;
            case 2:
                thirdPrisoner = prisonerFeatures;
                prisoners[2] = thirdPrisoner;
                prisonerNumber = Data.ThirdPrisoner;
                features = thirdPrisoner;
                break;
            case 3:
                fourthPrisoner = prisonerFeatures;
                prisoners[3] = fourthPrisoner;
                prisonerNumber = Data.FourthPrisoner;
                features = fourthPrisoner;
                break;
            default:
                Debug.Log("No more prisoners can be added.");
                return;
        }

        DataManager.Instance.UpdateValue(prisonerNumber, value: features);
        UpdatePrisonerCountLabel();
    }

    private void UpdatePrisonerCountLabel()
    {
        int prisonerCount = DataManager.Instance.GetPrisonersCount();
        if (prisonerCount == 0)
        {
            StartCoroutine(AnimateSendButton("Hide"));
        }
        else if (prisonerCount == 4)
        {
            StartCoroutine(AnimateSendButton("Show"));
        }
        prisonerCounterText.text = prisonerCount.ToString() + "/4";
    }

    public void OnSendButtonPressed()
    {
        despawnVFX.Play();
        SendPrisoners();
    }

    private void SendPrisoners()
    {
        DataManager.Instance.UpdateValue<Dictionary<string, int>>(Data.FirstPrisoner, null);
        DataManager.Instance.UpdateValue<Dictionary<string, int>>(Data.SecondPrisoner, null);
        DataManager.Instance.UpdateValue<Dictionary<string, int>>(Data.ThirdPrisoner, null);
        DataManager.Instance.UpdateValue<Dictionary<string, int>>(Data.FourthPrisoner, null);
        foreach (Transform prisoner in prison.transform)
        {
            Destroy(prisoner.gameObject);
        }
        UpdatePrisonerCountLabel();
        ActivateCoinRewardAnimator();
    }

    private void ActivateCoinRewardAnimator()
    {
        Vector3 touchPosition = Input.mousePosition;
        coinRewardAnimator.CollectCoinsAnimation(touchPosition);
    }

    private void OnDestroy()
    {
        sendButton.onClick.RemoveListener(OnSendButtonPressed);
    }

    private IEnumerator AnimateSendButton(string trigger)
    {
        while (DepartmentCameraAnimator.Instance.isCameraMoving)
        {
            yield return null;
        }
        prisonersCanvasAnimator.SetTrigger(trigger);
    }
}
