using CoreLoop.Interfaces;
using Zenject;

namespace CoreLoop.States
{
    public class RoomState : State
    {
        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public class Factory : PlaceholderFactory<RoomState> { }
    }
}
