using CoreLoop.Interfaces;
using CoreLoop.States;

namespace CoreLoop
{
    public class GameStateMachine : IGameStateMachine
    {
        private State currentState;
        public void ChangeState(State state)
        {
            currentState?.Exit();
            currentState = state;
            currentState?.Enter();
        }

        public void ChangeState<TPayload>(State<TPayload> state, TPayload payload)
        {
            currentState?.Exit();
            state.Payload = payload;
            currentState = state;
            currentState.Enter();
        }
    }
}