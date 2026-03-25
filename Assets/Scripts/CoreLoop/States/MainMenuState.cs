using CoreLoop.Interfaces;
using UI;
using UI.Interfaces;
using Zenject;

namespace CoreLoop.States
{
    public class MainMenuState : State
    {
        [Inject] private readonly ISceneLoader sceneLoader;
        [Inject] private readonly IUIFacade uiFacade;
        public override void Enter()
        {
            sceneLoader.LoadMainMenu();
            uiFacade.CloseAll();
        }

        public override void Exit()
        {
            
        }
    }
}