using CoreLoop.Interfaces;
using DefaultNamespace.Shnaps.Interfaces;
using UI.Interfaces;
using Zenject;

namespace DefaultNamespace.Shnaps
{
    public class ShnapsFacade : IShnapsFacade
    {
        [Inject] private IShnapsController shnapsController;
        [Inject] private IUIFacade uiFacade;
        [Inject] private ShnapsUI shnapsUI;
        
        public void GiveShnaps()
        {
            uiFacade.Open(shnapsUI);
            if (shnapsController.IsShnapsAvailable())
            {
                shnapsUI.PlayGivingShnapsAnimation();
                
                shnapsController.RemoveShnaps();
            }
            else
            {
                shnapsUI.PlayNoShnapsAnimation();
            }
        }

        public void TakeShnaps()
        {
            shnapsController.AddShnaps();
        }

        public void ClearShnaps()
        {
            shnapsController.ClearShnaps();
        }
    }
}