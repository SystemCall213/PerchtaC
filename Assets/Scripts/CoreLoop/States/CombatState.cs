using CoreLoop.Interfaces;
using Zenject;

namespace CoreLoop.States
{
    public class CombatState : State
    {
        [Inject] private readonly IGameStateMachine gameStateMachine;
        [Inject] private readonly ISceneLoader sceneLoader;
        [Inject] private readonly DefaultActions defaultActions;
        public override void Enter()
        {
            defaultActions.Combat.Enable();
        }

        public override void Exit()
        {
            defaultActions.Combat.Disable();
        }

        public class Factory : PlaceholderFactory<CombatState> { }
    }
}