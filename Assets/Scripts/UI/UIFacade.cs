using System.Collections.Generic;
using System.Linq;
using CoreLoop.Interfaces;
using ModestTree;
using UI.Interfaces;
using UnityEngine;
using Zenject;

namespace UI
{
    public class UIFacade : IUIFacade
    {
        [Inject] private readonly PauseMenu pauseMenu;
        [Inject] private readonly DefaultActions defaultActions;
        private Stack<ConfigurableCanvas> canvasStack = new Stack<ConfigurableCanvas>();

        public UIFacade()
        {
            defaultActions.UI.CloseMenu.performed += ctx => HandleEscape();
        }
        
        public void CloseTopmost()
        {
            if (canvasStack.Count == 0) return;
            ConfigurableCanvas canvas = canvasStack.Pop();
            canvas.gameObject.SetActive(false);
        }

        public void CloseAll()
        {
            while (canvasStack.Count > 0)
            {
                CloseTopmost();
            }
        }

        public void Open(ConfigurableCanvas canvas)
        {
            canvas.gameObject.SetActive(true);
            canvasStack.Push(canvas);
        }
        
        private void UpdateTimeScale()
        {
            bool shouldPause = canvasStack.Any(x => x.PausesTime);
            Time.timeScale = shouldPause ? 0 : 1;
        }

        private void HandleEscape()
        {
            if (canvasStack.IsEmpty())
            {
                pauseMenu.Open();
            }
            else if (canvasStack.Peek().ClosableWithEscape)
            {
                CloseTopmost();
            }
        }
    }
}