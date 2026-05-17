using System;
using CoreLoop.Interfaces;
using DefaultNamespace.Shnaps.Interfaces;
using UI;
using UI.Interfaces;
using UnityEngine;
using Zenject;

namespace DefaultNamespace.Shnaps
{
    public class ShnapsFacade : IShnapsFacade
    {
        [Inject] private IShnapsController shnapsController;
        [Inject] private IUIFacade uiFacade;
        [Inject] private ShnapsUI shnapsUI;
        [Inject] private ShnapsMenu shnapsMenu;
        
        private bool isFirstShnapsFound = true;
        
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
            if (isFirstShnapsFound)
            {
                Debug.Log("Shnaps found!");
                uiFacade.Open(shnapsMenu);
                isFirstShnapsFound = false;
            }
        }

        public void ClearShnaps()
        {
            shnapsController.ClearShnaps();
        }
    }
}