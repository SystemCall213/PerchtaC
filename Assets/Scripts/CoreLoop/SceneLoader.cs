
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

        public void LoadNextLevel()
        {
            SceneManager.LoadSceneAsync(levels[currentLevel]);
            currentLevel++;
        }

        public void LoadMainMenu()
        {
            SceneManager.LoadSceneAsync(MainMenuScene);
        }

        public void LoadCombatScene(string levelName)
        {
            SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
            currentCombatScene = levelName;
        }
        public void UnloadCombatScene()
        {
            SceneManager.UnloadSceneAsync(currentCombatScene);
            currentCombatScene = null;
        }

        public void LoadCreditsScene()
        {
            SceneManager.LoadSceneAsync(CreditsScene);
        }

        public void LoadCinematicScene(string scene)
        {
            SceneManager.LoadSceneAsync(scene);
        }

        public void ResetPlaythrough()
        {
            currentLevel = 0;
        }
    }
}