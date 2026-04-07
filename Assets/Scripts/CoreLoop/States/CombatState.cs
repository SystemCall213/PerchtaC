using CoreLoop.Interfaces;
using Zenject;

namespace CoreLoop.States
{
    public class CombatState : State<string>
    {
        [Inject] private readonly IGameStateMachine gameStateMachine;
        [Inject] private readonly ISceneLoader sceneLoader;
        [Inject] private readonly DefaultActions defaultActions;
        
        [Inject]
        public CombatState(string combatSceneName)
        {
            Payload = combatSceneName;
        }
        
        public override void Enter()
        {
            sceneLoader.LoadCombatScene(Payload);
            defaultActions.Combat.Enable();
        }

        public override void Exit()
        {
            defaultActions.Combat.Disable();
            sceneLoader.UnloadCombatScene();
        }

        public class Factory : PlaceholderFactory<string, CombatState> { }
    }
}