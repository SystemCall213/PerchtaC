
using System;
using CoreLoop.Interfaces;
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

        public SceneLoader(string[] levels)
        {
            this.levels = levels;
        }

        public event Action BattleEnded;

        public void LoadNextLevel()
        {
            if (currentLevel%levels.Length == 0 && currentLevel != 0)
            {
                LoadMainMenu();
            }
            else
            {
                SceneManager.LoadSceneAsync(levels[currentLevel%levels.Length]);
                currentLevel++;    
            }
        }

        public void LoadGivenLevel(string levelName)
        {
            SceneManager.LoadSceneAsync(levelName);
        }

        public void LoadMainMenu()
        {
            if (SceneManager.GetActiveScene().name == MainMenuScene) return;
            ResetPlaythrough();
            currentCombatScene = null;
            SceneManager.LoadSceneAsync(MainMenuScene);
        }

        public void LoadCombatScene(string levelName)
        {
            if (currentCombatScene == levelName) return;

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == levelName)
                {
                    currentCombatScene = levelName;
                    return;
                }
            }

            SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
            currentCombatScene = levelName;
        }
        public void UnloadCombatScene()
        {
            SceneManager.UnloadSceneAsync(currentCombatScene);
            BattleEnded.Invoke();
            currentCombatScene = null;
        }

        public void LoadCreditsScene()
        {
            SceneManager.LoadSceneAsync(CreditsScene);
        }

        public void LoadCinematicScene(string scene)
        {
            if (SceneManager.GetActiveScene().name == scene) return;
            SceneManager.LoadSceneAsync(scene);
        }

        public void ResetPlaythrough()
        {
            currentLevel = 0;
        }
    }
}