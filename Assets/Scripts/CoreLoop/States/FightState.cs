using CoreLoop.Interfaces;
using Zenject;

namespace CoreLoop.States
{
    public class FightState : State<string>
    {
        [Inject] private readonly IGameStateMachine gameStateMachine;
        [Inject] private readonly ISceneLoader sceneLoader;
        public FightState(string payload)
        {
            Payload = payload;
        }
        public override void Enter()
        {
            sceneLoader.LoadCombatScene(Payload);
            
        }

        public override void Exit()
        {
            throw new System.NotImplementedException();
        }
    }
}