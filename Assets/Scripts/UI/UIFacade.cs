using System.Collections.Generic;
using System.Linq;
using UI.Interfaces;
using UnityEngine;

namespace UI
{
    public class UIFacade : IUIFacade
    {
        private IPauseMenu pauseMenu;
        private readonly DefaultActions defaultActions;
        private Stack<IConfigurableCanvas> canvasStack = new Stack<IConfigurableCanvas>();

        public UIFacade(DefaultActions defaultActions)
        {
            this.defaultActions = defaultActions;
            
            defaultActions.UI.CloseMenu.performed += ctx => HandleEscape();
        }

        public void Setup(IPauseMenu pauseMenu)
        {
            this.pauseMenu = pauseMenu;
            Debug.Log("UI Facade setup");
        }
        
        public void CloseTopmost()
        {
            if (canvasStack.Count == 0) return;
            IConfigurableCanvas canvas = canvasStack.Pop();
            canvas.Close();
            UpdateTimeScale();
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
            UpdateTimeScale();
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
                Open(pauseMenu);
            }
            else if (canvasStack.Peek().ClosableWithEscape())
            {
                CloseTopmost();
            }
        }
    }
}