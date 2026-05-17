using System;

namespace DefaultNamespace.Shnaps.Interfaces
{
    public interface IShnapsFacade
    {
        event Action OnShnapsClicked;
        void GiveShnaps();
        void TakeShnaps();
        void ClearShnaps();
    }
}