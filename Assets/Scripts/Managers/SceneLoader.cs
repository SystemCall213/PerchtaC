using System.Collections.Generic;
using Installers.Interfaces;
using UnityEngine.SceneManagement;

namespace Installers
{
    public class SceneLoader : ISceneLoader
    {
        private List<SceneObject> _scenesToLoad;
        private int _currentSceneIndex;

        public SceneLoader(List<SceneObject> scenesToLoad)
        {
            _scenesToLoad = scenesToLoad;
        }

        public void LoadNextScene()
        {
            SceneManager.LoadScene(_scenesToLoad[_currentSceneIndex].SceneName);
            _currentSceneIndex++;
        }
    }
}