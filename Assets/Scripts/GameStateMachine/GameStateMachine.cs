using System;
using CoreLoop.Interfaces;
using CoreLoop.States;

namespace CoreLoop
{
    public class GameStateMachine : IGameStateMachine
    {
        public State CurrentState => currentState;
        private State currentState;
        public event Action<State> OnStateChanged;
        public GameStateMachine(MainMenuState.Factory mainMenuStateFactory)
        {
            
            ChangeState(mainMenuStateFactory.Create());
        }
        public void ChangeState(State state)
        {
            if (currentState != null && currentState.GetType() == state.GetType()) return;
            currentState?.Exit();
            currentState = state;
            currentState.Enter();
            OnStateChanged?.Invoke(state);
        }

        public void ChangeState<TPayload>(State<TPayload> state, TPayload payload)
        {
            state.Payload = payload;
            if (currentState != null && currentState.GetType() == state.GetType())
            {
                var currentWithPayload = currentState as State<TPayload>;
                if (currentWithPayload != null && Equals(currentWithPayload.Payload, payload))
                {
                    return;
                }
            }
            
            currentState?.Exit();
            currentState = state;
            currentState.Enter();
            OnStateChanged?.Invoke(state);
        }
    }
}