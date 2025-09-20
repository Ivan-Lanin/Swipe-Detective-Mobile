using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DataManager))]
public class DataManagerEditor : Editor
{
    private int tutorialStateToSet = 0;
    private static bool isSubscribed = false;
    
    // Use EditorPrefs key to store and retrieve the checkbox state
    private const string EDITOR_PREF_SAVE_ON_PLAY = "DataManager_CreateSaveWithTutorialOnPlay";
    
    // Property that reads/writes to EditorPrefs
    private static bool createSaveFileWithCompleteTutorialOnPlay
    {
        get { return EditorPrefs.GetBool(EDITOR_PREF_SAVE_ON_PLAY, false); }
        set { EditorPrefs.SetBool(EDITOR_PREF_SAVE_ON_PLAY, value); }
    }

    private void OnEnable()
    {
        // Subscribe to the event if the checkbox is checked
        if (createSaveFileWithCompleteTutorialOnPlay && !isSubscribed)
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            isSubscribed = true;
            Debug.Log("OnEnable: Registered play mode state change handler");
        }
    }

    private void OnDisable()
    {
        // Always unsubscribe when the editor is disabled
        if (isSubscribed)
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            isSubscribed = false;
            Debug.Log("OnDisable: Removed play mode state change handler");
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();

        DataManager dataManager = (DataManager)target;

        GUI.enabled = !Application.isPlaying;

        if (GUILayout.Button("Delete Save File"))
        {
            DeleteSaveFile(dataManager);
        }

        if (GUILayout.Button("Create New Save File Without EULA"))
        {
            string newSaveFilePath = Path.Combine(Application.dataPath, "Scripts/Editor/NewSaveFile/gamedata - Copy.json");
            if (File.Exists(newSaveFilePath))
            {
                if (!string.IsNullOrEmpty(dataManager.SaveFilePath))
                {
                    if (File.Exists(dataManager.SaveFilePath))
                    {
                        DeleteSaveFile(dataManager);
                    }

                    string newSaveFileDirectory = Path.GetDirectoryName(dataManager.SaveFilePath);
                    if (!Directory.Exists(newSaveFileDirectory))
                    {
                        Directory.CreateDirectory(newSaveFileDirectory);
                    }
                    File.Copy(newSaveFilePath, dataManager.SaveFilePath, true);
                    Debug.Log("New save file created at: " + dataManager.SaveFilePath);
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogWarning("Save file path is empty.");
                }
            }
            else
            {
                Debug.LogWarning("Template file not found at: " + newSaveFilePath);
            }
        }

        if (GUILayout.Button("Create New Save File With Complete Tutorial"))
        {
            string newSaveFilePath = Path.Combine(Application.dataPath, "Scripts/Editor/NewSaveFile/gamedata - Copy 1.json");
            if (File.Exists(newSaveFilePath))
            {
                if (!string.IsNullOrEmpty(dataManager.SaveFilePath))
                {
                    if (File.Exists(dataManager.SaveFilePath))
                    {
                        DeleteSaveFile(dataManager);
                    }
                    string newSaveFileDirectory = Path.GetDirectoryName(dataManager.SaveFilePath);
                    if (!Directory.Exists(newSaveFileDirectory))
                    {
                        Directory.CreateDirectory(newSaveFileDirectory);
                    }
                    File.Copy(newSaveFilePath, dataManager.SaveFilePath, true);
                    Debug.Log("New save file created at: " + dataManager.SaveFilePath);
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogWarning("Save file path is empty.");
                }
            }
            else
            {
                Debug.LogWarning("Template file not found at: " + newSaveFilePath);
            }
        }

        // Checkbox "Create New Save File With Complete Tutorial On Play"
        EditorGUI.BeginChangeCheck();
        bool newValue = EditorGUILayout.Toggle(
            "Complete Tutorial On Play", 
            createSaveFileWithCompleteTutorialOnPlay);
        
        if (EditorGUI.EndChangeCheck())
        {
            createSaveFileWithCompleteTutorialOnPlay = newValue;
            Debug.Log($"Checkbox changed to: {createSaveFileWithCompleteTutorialOnPlay}");
            
            // Only add/remove event when the checkbox value actually changes
            if (createSaveFileWithCompleteTutorialOnPlay && !isSubscribed)
            {
                EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
                isSubscribed = true;
                Debug.Log("Registered play mode state change handler");
            }
            else if (!createSaveFileWithCompleteTutorialOnPlay && isSubscribed)
            {
                EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                isSubscribed = false;
                Debug.Log("Removed play mode state change handler");
            }
        }

        if (GUILayout.Button("Open Save File in Explorer"))
        {
            if (!string.IsNullOrEmpty(dataManager.SaveFilePath) && File.Exists(dataManager.SaveFilePath))
            {
                EditorUtility.RevealInFinder(dataManager.SaveFilePath);
            }
            else
            {
                Debug.LogWarning("File does not exist.");
            }
        }

        GUIContent tutorialStateContent = new GUIContent(
            "State to Set",
            "Available states: \n0: Welcome\n6: PrisonersCount"
        );
        tutorialStateToSet = EditorGUILayout.IntField(tutorialStateContent, tutorialStateToSet);

        tutorialStateToSet = (int)(TutorialState)EditorGUILayout.EnumPopup("Tutorial State (Direct)", (TutorialState)tutorialStateToSet);


        if (GUILayout.Button("Set tutorialState in JSON"))
        {
            if (!string.IsNullOrEmpty(dataManager.SaveFilePath) && File.Exists(dataManager.SaveFilePath))
            {
                string json = File.ReadAllText(dataManager.SaveFilePath);

                GameData gameData = JsonUtility.FromJson<GameData>(json);

                gameData.tutorialState = (TutorialState)tutorialStateToSet;


                string newJson = JsonUtility.ToJson(gameData);
                File.WriteAllText(dataManager.SaveFilePath, newJson);
                Debug.Log($"tutorialState value set to {tutorialStateToSet}");
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogWarning("JSON file not found.");
            }
        }

        GUI.enabled = true;
    }

    // This is a static method because the editor scripts get reloaded
    // and we need to ensure this works across domain reloads
    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        Debug.Log($"PlayModeStateChanged: {state} - Checkbox value: {createSaveFileWithCompleteTutorialOnPlay}");
        
        // Use this instead of accessing target which might be null in play mode
        if (state == PlayModeStateChange.ExitingEditMode && createSaveFileWithCompleteTutorialOnPlay)
        {
            Debug.Log("Create save file with complete tutorial file before entering play mode...");

            DataManager dataManager = Object.FindFirstObjectByType<DataManager>();

            if (dataManager == null)
            {
                Debug.LogWarning("Could not find DataManager instance in scene.");
                return;
            }
            
            string newSaveFilePath = Path.Combine(Application.dataPath, "Scripts/Editor/NewSaveFile/gamedata - Copy 1.json");
            
            if (File.Exists(newSaveFilePath))
            {
                if (!string.IsNullOrEmpty(dataManager.SaveFilePath))
                {
                    if (File.Exists(dataManager.SaveFilePath))
                    {
                        File.Delete(dataManager.SaveFilePath);
                        Debug.Log("Deleted existing save file for play mode.");
                    }
                    
                    string newSaveFileDirectory = Path.GetDirectoryName(dataManager.SaveFilePath);
                    if (!Directory.Exists(newSaveFileDirectory))
                    {
                        Directory.CreateDirectory(newSaveFileDirectory);
                    }
                    
                    File.Copy(newSaveFilePath, dataManager.SaveFilePath, true);
                    Debug.Log("Created new save file with complete tutorial for play mode.");
                }
                else
                {
                    Debug.LogWarning("Save file path is empty.");
                }
            }
            else
            {
                Debug.LogWarning("Template file not found at: " + newSaveFilePath);
            }
        }
    }

    private void DeleteSaveFile(DataManager dataManager)
    {
        if (!string.IsNullOrEmpty(dataManager.SaveFilePath))
        {
            if (File.Exists(dataManager.SaveFilePath))
            {
                File.Delete(dataManager.SaveFilePath);
                Debug.Log("File deleted: " + dataManager.SaveFilePath);
            }
            else
            {
                Debug.LogWarning("File not found: " + dataManager.SaveFilePath);
            }

            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogWarning("File path is empty.");
        }
    }
}
