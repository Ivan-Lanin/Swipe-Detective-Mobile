using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ToTheDepartmentLevel))]
public class PlayResultPanel : FadeInPanel
{
    [SerializeField] private TMP_Text killerNameText;
    [SerializeField] private TMP_Text emeraldsCount;
    [SerializeField] private Image[] stars;
    [SerializeField] private ParticleSystem[] starParticles;
    [SerializeField] private Button claimButton;
    
    private ToTheDepartmentLevel toTheDepartmentLevelComponent;

    protected override void Awake()
    {
        base.Awake();
        toTheDepartmentLevelComponent = GetComponent<ToTheDepartmentLevel>();
        claimButton.onClick?.AddListener(OnClaimButtonClicked);
    }

    public void Appear(GameObject killer, int health, int emeralds)
    {
        base.Appear();

        // This is a forced measure due to the implementation of the used asset
        foreach (var particle in starParticles)
        {
            particle.Stop();
        }

        for (int i = 0; i < health; i++)
        {
            stars[i].color = Color.white;
            starParticles[i].Play();
        }
        StartCoroutine(FadeIn(1));
        string killerName = killer.name.Substring(2);
        killerNameText.text = killerName;
        emeraldsCount.text = emeralds.ToString();
    }

    private void OnClaimButtonClicked()
    {
        var prisonerSpawnerTrigger = new GameObject("PrisonerSpawnerTrigger").AddComponent<NewPrisonerTrigger>();
        prisonerSpawnerTrigger.DisposableTransitionData = SuspectFeatureGenerator.Instance.GetPrisonerFeatureIndexes();

        DataManager.Instance.UpdateValue(Data.CompletedLevels, DataManager.Instance.GameData.completedLevels + 1);
        DataManager.Instance.UpdateValue(Data.CollectedEmeralds, DataManager.Instance.GameData.collectedEmeralds + int.Parse(emeraldsCount.text));
        toTheDepartmentLevelComponent.StartDepartmentLevel();
    }

    private void OnDestroy()
    {
        claimButton.onClick?.RemoveListener(OnClaimButtonClicked);
    }
}
