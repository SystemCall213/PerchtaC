using CoreLoop.Interfaces;
using CoreLoop.States;
using UI.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    [RequireComponent(typeof(Canvas))]
    public class MainMenuController : MonoBehaviour
    {
        [Inject] private readonly ISceneLoader sceneLoader;
        [Inject] private readonly SettingMenu settingMenu;
        [Inject] private readonly IUIFacade uiFacade;
        
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
            sceneLoader.LoadNextLevel();
        }
        
        public void Settings()
        {
            uiFacade.Open(settingMenu);
        }
        public void Credits()
        {
            // do nothing for now, credits scene not ready
            // sceneLoader.LoadCreditsScene();
        }
        
        public void Quit()
        {
            Application.Quit();
        }
    }
}