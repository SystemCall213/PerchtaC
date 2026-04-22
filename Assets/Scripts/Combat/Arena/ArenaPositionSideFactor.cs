namespace Combat.Arena
{
    public struct ArenaPositionSideFactor
    {
        public enum SideFactor
        {
            Top,
            Bottom,
            Left,
            Right
        }
        
        public float Factor { get; set; }
        public SideFactor Side { get; set; }
    }
}