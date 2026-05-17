using Audio;
using CoreLoop.Interfaces;
using CoreLoop.StatePayload;
using UnityEngine;
using Zenject;

namespace CoreLoop.States
{
    [SceneState(SceneStateType.Combat)]
    public class CombatState : State<CombatStatePayload>
    {
        private readonly MusicId combatMusic;
        private readonly IMusicService musicService;
        private readonly DefaultActions defaultActions;
        
        [Inject]
        public CombatState(CombatStatePayload payload,IMusicService musicService, DefaultActions defaultActions)
        {
            Payload = payload;
            combatMusic = payload.MusicId;
            this.musicService = musicService;
            this.defaultActions = defaultActions;
        }
        
        public override void Enter()
        {
            defaultActions.UI.CloseMenu.Enable();
            defaultActions.Combat.Enable();
            musicService.Request(combatMusic);
            Cursor.visible = false;
        }

        public override void Exit()
        {
            defaultActions.Combat.Disable();
            defaultActions.UI.CloseMenu.Disable();
            Cursor.visible = true;
        }

        public class Factory : PlaceholderFactory<CombatStatePayload, CombatState> { }
    }
}