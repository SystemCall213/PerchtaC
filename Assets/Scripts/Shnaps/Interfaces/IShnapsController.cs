namespace DefaultNamespace.Shnaps.Interfaces
{
    public interface IShnapsController
    {
        void AddShnaps();
        void RemoveShnaps();
        void ClearShnaps();
        bool IsShnapsAvailable();
        int GetShnaps();
    }
}