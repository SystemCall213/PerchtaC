using System;
using CoreLoop.Interfaces;
using DefaultNamespace.Shnaps.Interfaces;
using Zenject;

namespace DefaultNamespace.Shnaps
{
    public class ShnapsAdapter : IInitializable, IDisposable
    {
        [Inject] private IShnapsController shnapsController;
        [Inject] private ShnapsUI shnapsUI;
        [Inject] private ISceneLoader sceneLoader;
        
        public void Initialize()
        {
            shnapsUI.OnNoShnapsGiven += OnNoShnapsGiven;
            shnapsUI.OnShnapsGiven += OnShnapsGiven;
        }

        public void Dispose()
        {
            shnapsUI.OnNoShnapsGiven -= OnNoShnapsGiven;
            shnapsUI.OnShnapsGiven -= OnShnapsGiven;
        }
        
        public void OnShnapsGiven()
        {
            shnapsController.RemoveShnaps();
            sceneLoader.ReloadCurrentCombatScene();
            
        }
        
        private void OnNoShnapsGiven()
        {
            sceneLoader.LoadMainMenu();
            sceneLoader.ResetPlaythrough();
        }
        
    }
}