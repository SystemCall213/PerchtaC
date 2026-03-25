using System.Collections.Generic;
using Audio;
using CoreLoop.Interfaces;
using TMPro;
using UI.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class SettingMenu : ConfigurableCanvas
    {
        [Inject] private readonly AudioManager audioManager;
        [Inject] private readonly IUIFacade uiFacade;
        
        [Header( "Audio" )]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        
        [Header( "Graphics" )]
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        
        [SerializeField] private Button backButton;
        
        private List<Resolution> filteredResolutions;
        private float currentRefreshRate;
        private Canvas canvas;

        private void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
        
        private void OnEnable()
        {
            InitializeResolutionDropdown();
            
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggle);
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        }

        private void OnDisable()
        {
            masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
            sfxVolumeSlider.onValueChanged.RemoveListener(OnSfxVolumeChanged);
            musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
            
            fullscreenToggle.onValueChanged.RemoveListener(OnFullscreenToggle);
            resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);
        }
        
        
        
        private void InitializeResolutionDropdown()
        {
            Resolution[] resolutions = Screen.resolutions;
            filteredResolutions = new List<Resolution>();
        
            resolutionDropdown.ClearOptions();
            currentRefreshRate = Screen.currentResolution.refreshRate;
            foreach (var resolution in resolutions)
            {
                if (resolution.refreshRate == currentRefreshRate)
                {
                    filteredResolutions.Add(resolution);
                }
            }
            resolutionDropdown.AddOptions(filteredResolutions.ConvertAll(resolution => resolution.ToString()));
            resolutionDropdown.value = filteredResolutions.FindIndex(resolution => resolution.Equals(Screen.currentResolution));
        }
        
        
        #region UI Events

        private void OnMasterVolumeChanged(float value)
        {
            audioManager.SetMasterVolume(value);
        }
        
        private void OnSfxVolumeChanged(float value)
        {
            audioManager.SetSfxVolume(value);
        }
        
        private void OnMusicVolumeChanged(float value)
        {
            audioManager.SetMusicVolume(value);
        }

        private void OnFullscreenToggle(bool value)
        {
            Screen.fullScreen = value;
        }

        private void OnResolutionChanged(int value)
        {
            Screen.SetResolution(filteredResolutions[value].width, filteredResolutions[value].height, Screen.fullScreen);
        }
        #endregion
    }
}