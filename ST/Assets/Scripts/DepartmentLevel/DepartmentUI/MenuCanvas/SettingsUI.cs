using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private Slider soundFXSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button setingsButton;
    [SerializeField] private Image musicIcon;
    [SerializeField] private Sprite musicSprite;
    [SerializeField] private Sprite musicSpriteOff;
    [SerializeField] private Image soundFXIcon;
    [SerializeField] private Sprite soundFXSprite;
    [SerializeField] private Sprite soundFXSpriteOff;

    [Header("Controllers")]
    [SerializeField] private MainCameraController mainCameraController;

    [Header("Logic")]
    [SerializeField] private SaveFileEraser saveFileEraser;

    private AudioSource mainTheme;

    private void Awake()
    {
        setingsButton.onClick.AddListener(OnSettingsButtonClicked);
        soundFXSlider.onValueChanged.AddListener(OnSoundFXSliderChanged);
        musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    private void Start()
    {
        soundFXSlider.value = DataManager.Instance.GameData.sfxVolume;
        musicSlider.value = DataManager.Instance.GameData.musicVolume;
    }

    private void OnSoundFXSliderChanged(float value)
    {
        if (SFXManager.Instance != null)
        {
            if (value > 0)
            {
                soundFXIcon.sprite = soundFXSprite;
            }
            else
            {
                soundFXIcon.sprite = soundFXSpriteOff;
            }
        }
    }

    private void OnMusicSliderChanged(float value)
    {
        float mappedValue = value * 0.15f;

        mainTheme = MainThemeAudio.Instance.audioSource;

        if (mainTheme != null)
        {
            mainTheme.volume = mappedValue;

            if (mainTheme.volume > 0)
            {
                musicIcon.sprite = musicSprite;
            }
            else
            {
                musicIcon.sprite = musicSpriteOff;
            }
        }
    }

    private void OnSettingsButtonClicked()
    {
        settingsCanvas.SetActive(true);
        mainCameraController.enabled = false;
    }

    private void OnExitButtonClicked()
    {
        DataManager.Instance.UpdateValue(Data.MusicVolume, musicSlider.value);
        DataManager.Instance.UpdateValue(Data.SFXVolume, soundFXSlider.value);
        saveFileEraser.OnSettingsExit();
        settingsCanvas.SetActive(false);
        mainCameraController.enabled = true;
    }
}
