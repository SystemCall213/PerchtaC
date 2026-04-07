using CoreLoop.Interfaces;
using Zenject;

namespace CoreLoop.States
{
    public class CombatState : State<string>
    {
        private readonly ISceneLoader sceneLoader;
        private readonly DefaultActions defaultActions;
        
        [Inject]
        public CombatState(string combatSceneName, ISceneLoader sceneLoader, DefaultActions defaultActions)
        {
            Payload = combatSceneName;
            this.sceneLoader = sceneLoader;
            this.defaultActions = defaultActions;
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