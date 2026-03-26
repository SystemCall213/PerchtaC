namespace CoreLoop.Interfaces
{ 
    public abstract class State
    {
        public abstract void Enter();
        public abstract void Exit();
    }

    public abstract class State<TPayload> : State
    {
        public TPayload Payload { get; set; }
    }
    
    
}