using UnityEditor.ShortcutManagement;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using System.Linq;

[Overlay(typeof(SceneView), "Scene Jumper", true)]
public class SceneSwitcherOverlay : Overlay
{
    private VisualElement root;

    public override VisualElement CreatePanelContent()
    {
        root = new VisualElement();
        root.style.paddingTop = 5;
        root.style.paddingBottom = 5;

        var scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => Path.GetFileNameWithoutExtension(s.path))
            .ToList();

        foreach (var sceneName in scenes)
        {
            var button = new Button(() => LoadScene(sceneName))
            {
                text = sceneName,
                style = {
                    marginBottom = 4,
                    unityFontStyleAndWeight = FontStyle.Bold
                }
            };
            root.Add(button);
        }

        return root;
    }

    private void LoadScene(string sceneName)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            string path = EditorBuildSettings.scenes
                .FirstOrDefault(s => Path.GetFileNameWithoutExtension(s.path) == sceneName)?.path;

            if (!string.IsNullOrEmpty(path))
            {
                EditorSceneManager.OpenScene(path);
            }
        }
    }
}

