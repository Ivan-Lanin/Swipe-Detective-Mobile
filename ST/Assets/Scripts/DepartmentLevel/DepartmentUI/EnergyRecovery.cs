using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class EnergyRecovery : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject timerBar;
    [SerializeField] private int energyRecoveryRateInSeconds;
    [SerializeField] private int maxEnergy;

    private DateTime lastDateTime;
    private bool isTimerRunning = false;

    private void Awake()
    {
        DataManager.Instance.OnDataUpdated += OnDataUpdatedHandler;
    }

    private void Start()
    {
        StartCoroutine(UpdateTimer());
    }

    private void OnDestroy()
    {
        DataManager.Instance.OnDataUpdated -= OnDataUpdatedHandler;
    }

    private void OnDataUpdatedHandler()
    {
        StartCoroutine(UpdateTimer());
    }

    private IEnumerator UpdateTimer()
    {
        if (isTimerRunning) yield break;
        if (DataManager.Instance.GameData.energy >= maxEnergy)
        {
            timerText.text = "00:00";
            timerBar.SetActive(false);
            yield break;
        }

        isTimerRunning = true;
        timerBar.SetActive(true);
        lastDateTime = DateTime.UtcNow;

        while (DataManager.Instance.GameData.energy < maxEnergy)
        {
            TimeSpan timeSinceLastUpdate = DateTime.UtcNow - lastDateTime;
            int secondsToAdd = (int)timeSinceLastUpdate.TotalSeconds;

            if (secondsToAdd >= energyRecoveryRateInSeconds)
            {
                lastDateTime = DateTime.UtcNow;
                DataManager.Instance.UpdateValue(Data.Energy, DataManager.Instance.GameData.energy + 1);
            }

            int remainingSeconds = Mathf.Max(0, energyRecoveryRateInSeconds - secondsToAdd);
            int minutes = remainingSeconds / 60;
            int seconds = remainingSeconds % 60;
            timerText.text = $"{minutes:D2}:{seconds:D2}";

            yield return new WaitForSeconds(1f);
        }
        timerBar.SetActive(false);
        isTimerRunning = false;
    }
}
