namespace Combat.Arena.PositionSelectors
{
    public class RandomRadialPositionSelector : IRadialPositionSelector
    {
        public float GetNextPositionFactor()
        {
            return UnityEngine.Random.Range(0f, 1f);
        }
    }
}