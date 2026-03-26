using System;
using System.Runtime.InteropServices;
using CoreLoop.Interfaces;
using CoreLoop.States;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    [RequireComponent(typeof(Canvas))]
    public class MainMenuController : MonoBehaviour
    {
        [Inject] private readonly IGameStateMachine gameStateMachine;
        [Inject] private readonly SettingMenu settingMenu;
        [Inject] private readonly LoadLevel.Factory loadLevelFactory;
        [Inject] private readonly CinematicState.Factory cinematicStateFactory;
        
        [SerializeField] private Button startButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button quitButton;

        private void OnEnable()
        {
            startButton.onClick.AddListener(Play);
            settingsButton.onClick.AddListener(Settings);
            creditsButton.onClick.AddListener(Credits);
            quitButton.onClick.AddListener(Quit);
        }

        private void OnDisable()
        {
            startButton.onClick.RemoveListener(Play);
            settingsButton.onClick.RemoveListener(Settings);
            creditsButton.onClick.RemoveListener(Credits);
            quitButton.onClick.RemoveListener(Quit);
        }

        public void Play()
        { 
            gameStateMachine.ChangeState(loadLevelFactory.Create());
        }
        
        public void Settings()
        {
            settingMenu.Open();
        }
        public void Credits()
        {
            gameStateMachine.ChangeState(cinematicStateFactory.Create("CreditsLevel"));
        }
        
        public void Quit()
        {
            Application.Quit();
        }
    }
}