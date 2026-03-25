using UnityEngine;

namespace UI.Interfaces
{
    public interface IUIFacade
    {
        void CloseTopmost();
        void CloseAll();
        void Open(ConfigurableCanvas canvas);
    }
}