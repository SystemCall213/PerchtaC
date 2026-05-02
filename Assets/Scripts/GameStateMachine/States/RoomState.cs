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

        [Inject]
        public RoomState(RoomStatePayload payload, IMusicService musicService)
        {
            Payload = payload;
            this.musicService = musicService;
        }
        
        public override void Enter()
        {
            if (Payload.MusicId != MusicId.None)
            { 
                musicService.Request(Payload.MusicId);
            }
        }

        public override void Exit()
        {
        }

        public class Factory : PlaceholderFactory<RoomStatePayload, RoomState> { }
    }
}
