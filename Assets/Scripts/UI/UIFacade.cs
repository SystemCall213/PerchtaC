using System.Collections.Generic;
using System.Linq;
using CoreLoop.Interfaces;
using UI.Interfaces;
using UnityEngine;

namespace UI
{
    public class UIFacade : IUIFacade
    {
        private IGameStateMachine gameStateMachine;
        private IPauseMenu pauseMenu;
        private readonly DefaultActions defaultActions;
        private Stack<IConfigurableCanvas> canvasStack = new Stack<IConfigurableCanvas>();

        public UIFacade(DefaultActions defaultActions, IGameStateMachine gameStateMachine)
        {
            this.defaultActions = defaultActions;
            this.gameStateMachine = gameStateMachine;
            
            defaultActions.UI.CloseMenu.performed += ctx => HandleEscape();
        }

        public void Setup(IPauseMenu pauseMenu)
        {
            this.pauseMenu = pauseMenu;
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
                if (gameStateMachine.IsInMainMenu)
                {
                    return;
                }
                Open(pauseMenu);
            }
            else if (canvasStack.Peek().ClosableWithEscape())
            {
                CloseTopmost();
            }
        }
    }
}