using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestingMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private Button toggeButtonsButton;
    [SerializeField] private Button addEnergyButton;
    [SerializeField] private Button addGoldButton;
    [SerializeField] private Button addPrisonerButton;
    [SerializeField] private PrisonerSpawner prisonerSpawner;

    private void Awake()
    {
        toggeButtonsButton?.onClick.AddListener(ToggleButtonsVisibility);
        addEnergyButton?.onClick.AddListener(AddEnergy);
        addGoldButton?.onClick.AddListener(AddGold);
        addPrisonerButton?.onClick.AddListener(AddPrisoner);
    }

    private void ToggleButtonsVisibility()
    {
        foreach (var button in buttons)
        {
            button.SetActive(!button.activeSelf);
        }
    }

    private void AddEnergy()
    {
        DataManager.Instance.Add(Data.Energy, 5);
    }

    private void AddGold()
    {
        DataManager.Instance.Add(Data.Gold, 15);
    }

    private void AddPrisoner()
    {
        int prisonersCount = DataManager.Instance.GetPrisonersCount();
        if (prisonersCount == 4) return;
        Dictionary<string, int> newPrisoner = new Dictionary<string, int>
        {
            { "hairLength", 1 },
            { "hairColor", 1 },
            { "extra", 1 }
        };
        prisonerSpawner.SpawnNewPrisoner(newPrisoner);
    }

    private void OnDestroy()
    {
        toggeButtonsButton?.onClick.RemoveListener(ToggleButtonsVisibility);
        addEnergyButton?.onClick.RemoveListener(AddEnergy);
        addGoldButton?.onClick.RemoveListener(AddGold);
        addPrisonerButton?.onClick.RemoveListener(AddPrisoner);
    }
}
