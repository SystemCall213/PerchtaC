using Audio;
using CoreLoop.Interfaces;
using CoreLoop.StatePayload;
using Zenject;

namespace CoreLoop.States
{
    [SceneState(SceneStateType.Room)]
    public class RoomState : State<RoomStatePayload>
    {
        private readonly IMusicService musicService;
        private readonly DefaultActions defaultActions;

        [Inject]
        public RoomState(RoomStatePayload payload, IMusicService musicService, DefaultActions defaultActions)
        {
            Payload = payload;
            this.musicService = musicService;
            this.defaultActions = defaultActions;
        }
        
        public override void Enter()
        {
            defaultActions.UI.CloseMenu.Enable();
            
            if (Payload.MusicId != MusicId.None)
            {
                musicService.RequestIfNotPlaying(Payload.MusicId, MusicDestinationMarker.Room);
                musicService.SetIsInRoom(true);
            }
        }

        public override void Exit()
        {
        }

        public class Factory : PlaceholderFactory<RoomStatePayload, RoomState> { }
    }
}
