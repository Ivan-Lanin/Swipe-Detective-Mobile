using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public enum Data
{
    CompletedLevels,
    CollectedEmeralds,
    Energy,
    Gold,
    TutorialState,
    FirstPrisoner,
    SecondPrisoner,
    ThirdPrisoner,
    FourthPrisoner,
    MusicVolume,
    SFXVolume,
    TutorialCompleted,
    EULAAccepted,
    IsFirstLaunch,
    SortingCost,
    PrisonersPrice,
    EvidenceRoomLevel,
    EvidenceRoomPrice,
    MaxEvidenceRoomLevel,
    ContactCenterLevel,
    ContactCenterPrice,
    MaxContactCenterLevel,
    LastDailyRewardDate
}

[Serializable]
public class GameData
{
    // Player progress data
    public int completedLevels = 0;
    public int collectedEmeralds = 0;
    public int energy = 20;
    public int gold = 0;
    public TutorialState tutorialState = TutorialState.Welcome;
    public int evidenceRoomLevel = 0;
    public int contactCenterLevel = 0;

    // example of prisoner string: "hairLength=1; hairColor=1; extra=4;"
    public string firstPrisoner = null;
    public string secondPrisoner = null;
    public string thirdPrisoner = null;
    public string fourthPrisoner = null;

    // Game settings
    public float musicVolume = 1.0f;
    public float sfxVolume = 1.0f;
    public bool tutorialCompleted = false;
    public bool eulaAccepted = false;
    public bool isFirstLaunch = true; //false after first sorting level transition
    public int sortingCost = 5;
    public int prisonersBounty = 15;
    public int evidenceRoomPrice = 30;
    public int maxEvidenceRoomLevel = 1;
    public int contactCenterPrice = 60;
    public int maxContactCenterLevel = 1;
    //public string lastDailyRewardDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
    public string lastDailyRewardDate = "2023-10-01";

    // Level specific data
    public Dictionary<string, object> levelData = new Dictionary<string, object>();

    public Dictionary<string, int> GetPrisonerFeatures(int prisonerN)
    {

        switch (prisonerN)
        {
            case 1:
                return PrisonerStringToDict(firstPrisoner);
            case 2:
                return PrisonerStringToDict(secondPrisoner);
            case 3:
                return PrisonerStringToDict(thirdPrisoner);
            case 4:
                return PrisonerStringToDict(fourthPrisoner);
            default:
                return null;
        }
    }

    private Dictionary<string, int> PrisonerStringToDict(string prisonerString)
    {
        if (string.IsNullOrEmpty(prisonerString)) return null;

        Dictionary<string, int> prisonerDict = new Dictionary<string, int>();
        string[] keysAndValues = prisonerString.Split("; ");

        foreach (string keyValue in keysAndValues)
        {
            string key = keyValue.Split("=")[0];
            string stringValue = keyValue.Split("=")[1];
            stringValue = stringValue.Replace(";", "");
            if (stringValue == "null") continue;
            int value;
            value = Int32.Parse(stringValue);
            prisonerDict.Add(key, value);
        }

        return prisonerDict;
    }

    public void SetPrisonerFeatures(int prisonerN, Dictionary<string, int> prisonerDict)
    {
        switch (prisonerN)
        {
            case 1:
                firstPrisoner = PrisonerDictToString(prisonerDict);
                break;
            case 2:
                secondPrisoner = PrisonerDictToString(prisonerDict); PrisonerDictToString(prisonerDict);
                break;
            case 3:
                thirdPrisoner = PrisonerDictToString(prisonerDict); PrisonerDictToString(prisonerDict);
                break;
            case 4:
                fourthPrisoner = PrisonerDictToString(prisonerDict); PrisonerDictToString(prisonerDict);
                break;
            default:
                return;
        }
    }

    private string PrisonerDictToString(Dictionary<string, int> prisonerDict)
    {
        if (prisonerDict == null) return null;
        if (prisonerDict.Count == 0) return null;

        string prisonerString = "";

        foreach (KeyValuePair<string, int> keyValuePair in prisonerDict)
        {
            prisonerString += keyValuePair.Key + "=";
            prisonerString += keyValuePair.Value + ";";
            if (keyValuePair.Key != "extra") prisonerString += " ";
        }

        return prisonerString;
    }
}

public class DataManager : MonoBehaviour
{
    [SerializeField] private TutorialText tutorialDialogueTextENG;
    private string language = "ENG"; // TODO: Implement language selection
    public TutorialText TutorialDialogueText
    {
        get
        {
            switch (language)
            {
                case "ENG":
                    return tutorialDialogueTextENG;
                default:
                    return tutorialDialogueTextENG;
            }
        }
    }

    public static DataManager Instance;

    private GameData _gameData;

    private readonly string _saveFileName = "gamedata.json";

    public string SaveFilePath
    {
        get => Path.Combine(Application.persistentDataPath, _saveFileName);
        private set => SaveFilePath = value;
    }

    public event Action OnDataLoaded;
    public event Action OnDataSaved;
    public event Action OnDataUpdated;
    public event Action OnResourceDataUpdated;

    public GameData GameData => _gameData;

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

        LoadData();
        DontDestroyOnLoad(gameObject);
    }

    public bool LoadData()
    {
        try
        {
            if (File.Exists(SaveFilePath))
            {
                string jsonData = File.ReadAllText(SaveFilePath);
                _gameData = JsonUtility.FromJson<GameData>(jsonData);
                OnDataLoaded?.Invoke();
                return true;
            }
            else
            {
                Debug.Log("No saved game data found. Creating new data.");
                _gameData = new GameData();
                SaveData();
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load game data: {e.Message}");
            _gameData = new GameData();
            return false;
        }
    }

    public bool SaveData()
    {
        try
        {
            string jsonData = JsonUtility.ToJson(_gameData, true);
            File.WriteAllText(SaveFilePath, jsonData);
            OnDataSaved?.Invoke();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save game data: {e.Message}");
            return false;
        }
    }

    public void DeleteSaveData()
    {
        if (File.Exists(SaveFilePath))
        {
            File.Delete(SaveFilePath);
            _gameData = new GameData();
            Debug.Log("Game data deleted");
        }
    }

    // Update a specific value and save
    public void UpdateValue<T>(Data key, T value)
    {
        switch (key)
        {
            case Data.CompletedLevels:
                _gameData.completedLevels = Convert.ToInt32(value);
                break;
            case Data.CollectedEmeralds:
                _gameData.collectedEmeralds = Convert.ToInt32(value);
                break;
            case Data.Energy:
                _gameData.energy = Convert.ToInt32(value);
                break;
            case Data.Gold:
                _gameData.gold = Convert.ToInt32(value);
                break;
            case Data.TutorialState:
                if (Enum.TryParse(value.ToString(), out TutorialState tutorialState))
                {
                    _gameData.tutorialState = tutorialState;
                }
                else
                {
                    Debug.LogError($"Invalid TutorialState value: {value}");
                }
                break;
            case Data.FirstPrisoner:
                _gameData.SetPrisonerFeatures(1, value as Dictionary<string, int>);
                break;
            case Data.SecondPrisoner:
                _gameData.SetPrisonerFeatures(2, value as Dictionary<string, int>);
                break;
            case Data.ThirdPrisoner:
                _gameData.SetPrisonerFeatures(3, value as Dictionary<string, int>);
                break;
            case Data.FourthPrisoner:
                _gameData.SetPrisonerFeatures(4, value as Dictionary<string, int>);
                break;
            case Data.MusicVolume:
                _gameData.musicVolume = Convert.ToSingle(value);
                break;
            case Data.SFXVolume:
                _gameData.sfxVolume = Convert.ToSingle(value);
                break;
            case Data.TutorialCompleted:
                _gameData.tutorialCompleted = Convert.ToBoolean(value);
                break;
            case Data.EULAAccepted:
                _gameData.eulaAccepted = Convert.ToBoolean(value);
                break;
            case Data.IsFirstLaunch:
                _gameData.isFirstLaunch = Convert.ToBoolean(value);
                break;
            case Data.SortingCost:
                _gameData.sortingCost = Convert.ToInt32(value);
                break;
            case Data.PrisonersPrice:
                _gameData.prisonersBounty = Convert.ToInt32(value);
                break;
            case Data.EvidenceRoomLevel:
                _gameData.evidenceRoomLevel = Convert.ToInt32(value);
                break;
            case Data.EvidenceRoomPrice:
                _gameData.evidenceRoomPrice = Convert.ToInt32(value);
                break;
            case Data.MaxEvidenceRoomLevel:
                _gameData.maxEvidenceRoomLevel = Convert.ToInt32(value);
                break;
            case Data.ContactCenterLevel:
                _gameData.contactCenterLevel = Convert.ToInt32(value);
                break;
            case Data.ContactCenterPrice:
                _gameData.contactCenterPrice = Convert.ToInt32(value);
                break;
            case Data.MaxContactCenterLevel:
                _gameData.maxContactCenterLevel = Convert.ToInt32(value);
                break;
            case Data.LastDailyRewardDate:
                _gameData.lastDailyRewardDate = value.ToString();
                break;
            default:
                // Store in level data dictionary
                _gameData.levelData[key.ToString()] = value;
                break;
        }

        SaveData();
        OnDataUpdated?.Invoke();
        if (key == Data.Energy || key == Data.Gold || key == Data.CollectedEmeralds)
        {
            OnResourceDataUpdated?.Invoke();
        }
    }

    public void Add(Data key, int value)
    {
        switch (key)
        {
            case Data.CompletedLevels:
                int currentCompletedLevels = _gameData.completedLevels;
                UpdateValue(Data.CompletedLevels, currentCompletedLevels + value);
                break;
            case Data.CollectedEmeralds:
                int currentCollectedEmeralds = _gameData.collectedEmeralds;
                UpdateValue(Data.CollectedEmeralds, currentCollectedEmeralds + value);
                break;
            case Data.Energy:
                int currentEnergy = _gameData.energy;
                UpdateValue(Data.Energy, currentEnergy + value);
                break;
            case Data.Gold:
                int currentGold = _gameData.gold;
                UpdateValue(Data.Gold, currentGold + value);
                break;
            case Data.EvidenceRoomLevel:
                int currentEvidenceRoomLevel = _gameData.evidenceRoomLevel;
                int nextLevel = currentEvidenceRoomLevel + value;
                if (nextLevel <= _gameData.maxEvidenceRoomLevel) UpdateValue(Data.EvidenceRoomLevel, nextLevel);
                break;
            case Data.ContactCenterLevel:
                int currentContactCenterLevel = _gameData.contactCenterLevel;
                int nextContactCenterLevel = currentContactCenterLevel + value;
                if (nextContactCenterLevel <= _gameData.maxContactCenterLevel) UpdateValue(Data.ContactCenterLevel, nextContactCenterLevel);
                break;
            default:
                Debug.LogWarning($"Data key {key} is not supported for addition.");
                break;
        }
    }

    public int GetPrisonersCount()
    {
        int count = 0;
        if (_gameData.GetPrisonerFeatures(1) != null) count++;
        if (_gameData.GetPrisonerFeatures(2) != null) count++;
        if (_gameData.GetPrisonerFeatures(3) != null) count++;
        if (_gameData.GetPrisonerFeatures(4) != null) count++;
        return count;
    }

    // PlayerPrefs backup for critical settings
    public void SaveToPlayerPrefs()
    {
        PlayerPrefs.SetFloat("MusicVolume", _gameData.musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", _gameData.sfxVolume);
        PlayerPrefs.SetInt("CompletedLevels", _gameData.completedLevels);
        PlayerPrefs.SetInt("TutorialCompleted", _gameData.tutorialCompleted ? 1 : 0);
        PlayerPrefs.SetInt("EULAAccepted", _gameData.eulaAccepted ? 1 : 0);
        PlayerPrefs.SetInt("IsFirstLaunch", _gameData.isFirstLaunch ? 1 : 0);
        PlayerPrefs.SetInt("SortingCost", _gameData.sortingCost);
        PlayerPrefs.SetInt("PrisonersPrice", _gameData.prisonersBounty);
        PlayerPrefs.SetInt("EvidenceRoomPrice", _gameData.evidenceRoomPrice);
        PlayerPrefs.SetInt("MaxEvidenceRoomLevel", _gameData.maxEvidenceRoomLevel);
        PlayerPrefs.SetInt("ContactCenterPrice", _gameData.contactCenterPrice);
        PlayerPrefs.SetInt("MaxContactCenterLevel", _gameData.maxContactCenterLevel);
        PlayerPrefs.Save();
    }

    // Load from PlayerPrefs as fallback
    public void LoadFromPlayerPrefs()
    {
        _gameData.musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        _gameData.sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        _gameData.completedLevels = PlayerPrefs.GetInt("CompletedLevels", 0);
        _gameData.tutorialCompleted = PlayerPrefs.GetInt("TutorialCompleted", 0) == 1;
        _gameData.eulaAccepted = PlayerPrefs.GetInt("EULAAccepted", 0) == 1;
        _gameData.isFirstLaunch = PlayerPrefs.GetInt("IsFirstLaunch", 1) == 1;
        _gameData.sortingCost = PlayerPrefs.GetInt("SortingCost", 5);
        _gameData.prisonersBounty = PlayerPrefs.GetInt("PrisonersPrice", 15);
        _gameData.evidenceRoomPrice = PlayerPrefs.GetInt("EvidenceRoomPrice", 30);
        _gameData.maxEvidenceRoomLevel = PlayerPrefs.GetInt("MaxEvidenceRoomLevel", 1);
        _gameData.contactCenterPrice = PlayerPrefs.GetInt("ContactCenterPrice", 60);
        _gameData.maxContactCenterLevel = PlayerPrefs.GetInt("MaxContactCenterLevel", 1);
    }
}
