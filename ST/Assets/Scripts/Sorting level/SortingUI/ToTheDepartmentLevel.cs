using UnityEngine;
using UnityEngine.SceneManagement;

public class ToTheDepartmentLevel : ToTheLevelBase
{
    protected override void Awake()
    {
        sceneName = "DepartmentLevel";
        base.Awake();
    }

    public void StartDepartmentLevel()
    {
        StartLevel();
    }
}
