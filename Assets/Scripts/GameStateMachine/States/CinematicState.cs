using CoreLoop.Interfaces;
using Zenject;

namespace CoreLoop.States
{
    [SceneState(SceneStateType.Cinematic)]
    public class CinematicState : State
    {
        private readonly ISceneLoader sceneLoader;
        private readonly DefaultActions defaultActions;

        [Inject]
        public CinematicState(ISceneLoader sceneLoader, DefaultActions defaultActions)
        {
            this.sceneLoader = sceneLoader;
            this.defaultActions = defaultActions;
        }

        public override void Enter()
        {
            defaultActions.UI.CloseMenu.Disable();
            defaultActions.UI.SkipCinematic.Enable();
        }

        public override void Exit()
        {
            defaultActions.UI.CloseMenu.Enable();
            defaultActions.UI.SkipCinematic.Disable();
        }

        public class Factory : PlaceholderFactory<CinematicState> { }
    }
}