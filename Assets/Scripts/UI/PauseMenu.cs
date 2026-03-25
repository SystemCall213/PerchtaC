using System;
using CoreLoop.Interfaces;
using UI.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class PauseMenu : ConfigurableCanvas
    {
        [Inject] private readonly IUIFacade uiFacade;
        [Inject] private readonly ISceneLoader sceneLoader;
        [Inject] private readonly SettingMenu settingMenu;
        
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitButton;
        
        private void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
        
        private void OnEnable()
        {
            resumeButton.onClick.AddListener(Resume);
            settingsButton.onClick.AddListener(Settings);
            exitButton.onClick.AddListener(Exit);
        }
        
        private void OnDisable()
        {
            resumeButton.onClick.RemoveListener(Resume);
            settingsButton.onClick.RemoveListener(Settings);
            exitButton.onClick.RemoveListener(Exit);
        }

        private void Resume()
        {
            uiFacade.CloseTopmost();
        }
        private void Settings()
        {
            settingMenu.Open();
        }
        private void Exit()
        {
            sceneLoader.LoadMainMenu();
        }
    }
}