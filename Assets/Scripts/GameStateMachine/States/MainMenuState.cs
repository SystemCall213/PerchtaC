using Audio;
using CoreLoop.Interfaces;
using UI.Interfaces;
using Zenject;

namespace CoreLoop.States
{
    [SceneState(SceneStateType.MainMenu)]
    public class MainMenuState : State
    {
        [Inject] private readonly IUIFacade uiFacade;
        [Inject] private readonly IMusicService musicService;
        public override void Enter()
        {
            musicService.Request(MusicId.MainMenu);
            uiFacade.CloseAll();
        }

        public override void Exit()
        {
            
        }

        public class Factory : PlaceholderFactory<MainMenuState> { }
    }
}