namespace CoreLoop.Interfaces
{
    public interface IGameStateMachine
    {
        void ChangeState(State state);
        void ChangeState<TPayload>(State<TPayload> state, TPayload payload);
    }
}