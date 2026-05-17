using UI.Interfaces;
using Zenject;

namespace UI
{
    public class TutorialMenu : ConfigurableCanvas
    { 
        [Inject] private IUIFacade uiFacade;
        private bool isFirstTime = true;

        public void TryOpenTutorial()
        {
            if (isFirstTime)
            {
                uiFacade.Open(this);
                isFirstTime = false;
            }
        }

        public void CloseTutorial()
        {
            uiFacade.CloseTopmost();
        }

        public void ResetFirstTime()
        {
            isFirstTime = true;
        }
    }
}