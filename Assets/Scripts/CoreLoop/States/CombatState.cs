using CoreLoop.Interfaces;
using Zenject;

namespace CoreLoop.States
{
    public class CombatState : State<string>
    {
        [Inject] private readonly IGameStateMachine gameStateMachine;
        [Inject] private readonly ISceneLoader sceneLoader;
        [Inject] private readonly DefaultActions defaultActions;
        public CombatState(string payload)
        {
            Payload = payload;
        }
        public override void Enter()
        {
            sceneLoader.LoadCombatScene(Payload);
            defaultActions.Combat.Enable();
            
        }

        public override void Exit()
        {
            defaultActions.Combat.Disable();
        }
    }
}