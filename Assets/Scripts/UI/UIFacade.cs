using System.Collections.Generic;
using System.Linq;
using UI.Interfaces;
using UnityEngine;

namespace UI
{
    public class UIFacade : IUIFacade
    {
        private readonly PauseMenu pauseMenu;
        private readonly DefaultActions defaultActions;
        private Stack<IConfigurableCanvas> canvasStack = new Stack<IConfigurableCanvas>();

        public UIFacade(PauseMenu pauseMenu, DefaultActions defaultActions)
        {
            this.pauseMenu = pauseMenu;
            this.defaultActions = defaultActions;
            
            defaultActions.UI.CloseMenu.performed += ctx => HandleEscape();
        }
        
        public void CloseTopmost()
        {
            if (canvasStack.Count == 0) return;
            IConfigurableCanvas canvas = canvasStack.Pop();
        }

        public void CloseAll()
        {
            while (canvasStack.Count > 0)
            {
                CloseTopmost();
            }
        }

        public void Open(IConfigurableCanvas canvas)
        {
            canvas.Open();
            canvasStack.Push(canvas);
        }
        
        private void UpdateTimeScale()
        {
            bool shouldPause = canvasStack.Any(x => x.PausesTime());
            Time.timeScale = shouldPause ? 0 : 1;
        }

        private void HandleEscape()
        {
            if (canvasStack.Count == 0)
            {
                pauseMenu.Open();
            }
            else if (canvasStack.Peek().ClosableWithEscape())
            {
                CloseTopmost();
            }
        }
    }
}