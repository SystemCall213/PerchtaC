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

        protected abstract void Enter(TPayload payload);
        protected abstract void Exit(TPayload payload);
        
        public override void Enter()
        {
            Enter(Payload);
        }

        public override void Exit()
        {
            Exit(Payload);
        }
    }
}