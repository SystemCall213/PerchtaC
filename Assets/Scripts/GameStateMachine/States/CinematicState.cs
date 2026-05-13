using Audio;
using CoreLoop.Interfaces;
using CoreLoop.StatePayload;
using Zenject;

namespace CoreLoop.States
{
    [SceneState(SceneStateType.Cinematic)]
    public class CinematicState : State<CinematicStatePayload>
    {
        private readonly MusicId cinematicMusic;
        private readonly ISceneLoader sceneLoader;
        private readonly DefaultActions defaultActions;

        [Inject]
        public CinematicState(CinematicStatePayload payload, ISceneLoader sceneLoader, DefaultActions defaultActions)
        {
            Payload = payload;
            cinematicMusic = payload.MusicId;
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