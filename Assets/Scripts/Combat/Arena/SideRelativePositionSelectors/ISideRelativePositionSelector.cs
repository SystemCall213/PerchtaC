namespace Combat.Arena.SideRelativePositionSelectors
{
    public interface ISideRelativePositionSelector
    {
        ArenaPositionSideFactor GetNextPositionFactor();
    }
}