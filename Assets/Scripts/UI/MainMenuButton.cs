using CoreLoop.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class MainMenuButton : MonoBehaviour
    {
        [Inject] private ISceneLoader sceneLoader;
        private Button button;
        
        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OpenMainMenu);
        }
        
        public void OpenMainMenu()
        {
            sceneLoader.LoadMainMenu();
        }
    }
}