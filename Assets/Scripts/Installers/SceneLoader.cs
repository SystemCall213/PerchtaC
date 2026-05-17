
using System;
using Cysharp.Threading.Tasks;
using CoreLoop.Interfaces;
using DefaultNamespace.Shnaps.Interfaces;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CoreLoop
{
    public class SceneLoader : ISceneLoader
    {
        private const string MainMenuScene = "MainMenuScene";
        private const string CreditsScene = "CreditsScene";
        
        private string[] levels;
        private int currentLevel = 0;
        private string currentCombatScene;
        private readonly LoadingScreen loadingScreen;
        private readonly TutorialMenu tutorialMenu;
        private IShnapsFacade shnapsFacade;
        private bool isLoading;

        public SceneLoader(string[] levels, LoadingScreen loadingScreen, IShnapsFacade shnapsFacade, TutorialMenu tutorialMenu)
        {
            this.levels = levels;
            this.loadingScreen = loadingScreen;
            this.shnapsFacade = shnapsFacade;
            this.tutorialMenu = tutorialMenu;
        }

        public event Action BattleEnded;

        public void LoadNextLevel()
        {
            if (isLoading) return;

            if (currentLevel%levels.Length == 0 && currentLevel != 0)
            {
                LoadMainMenu();
            }
            else
            {
                LoadSceneWithScreen(levels[currentLevel%levels.Length]).Forget();
                currentLevel++;
            }
        }

        public void LoadGivenLevel(string levelName)
        {
            if (isLoading) return;

            LoadSceneWithScreen(levelName).Forget();
        }

        public void LoadMainMenu()
        {
            if (isLoading || SceneManager.GetActiveScene().name == MainMenuScene) return;

            ResetPlaythrough();
            tutorialMenu.ResetFirstTime();
            currentCombatScene = null;
            LoadSceneWithScreen(MainMenuScene).Forget();
        }

        public void LoadCombatScene(string levelName)
        {
            if (isLoading) return;

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == levelName)
                {
                    currentCombatScene = levelName;
                    return;
                }
            }

            LoadSceneWithScreen(levelName, LoadSceneMode.Additive).Forget();
            currentCombatScene = levelName;
        }
        public void UnloadCombatScene()
        {
            SceneManager.UnloadSceneAsync(currentCombatScene);
            BattleEnded.Invoke();
        }

        public void ReloadCurrentCombatScene()
        {
            LoadSceneWithScreen(currentCombatScene, LoadSceneMode.Additive).Forget();
        }

        public void LoadCreditsScene()
        {
            if (isLoading) return;

            LoadSceneWithScreen(CreditsScene).Forget();
        }

        public void LoadCinematicScene(string scene)
        {
            if (isLoading || SceneManager.GetActiveScene().name == scene) return;

            LoadSceneWithScreen(scene).Forget();
        }

        public void ResetPlaythrough()
        {
            currentLevel = 0;
            shnapsFacade.ClearShnaps();
        }

        private async UniTaskVoid LoadSceneWithScreen(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if (isLoading) return;

            isLoading = true;
            try
            {
                await loadingScreen.FadeIn();

                AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);

                if (loadingOperation != null)
                {
                    while (!loadingOperation.isDone)
                    {
                        await UniTask.Yield();
                    }
                }
            }
            finally
            {
                await loadingScreen.FadeOut();
                isLoading = false;
            }
        }
    }
}