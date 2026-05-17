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
            Debug.Log("Giving shnaps");
            uiFacade.Open(shnapsUI);
            if (shnapsController.IsShnapsAvailable())
            {
                shnapsUI.PlayGivingShnapsAnimation();
            }
            else
            {
                shnapsUI.PlayNoShnapsAnimation();
            }
        }

        public void TakeShnaps()
        {
            Debug.Log("Taking shnaps");
            shnapsController.AddShnaps();
            if (isFirstShnapsFound)
            {
                uiFacade.Open(shnapsMenu);
                isFirstShnapsFound = false;
            }
        }

        public void ClearShnaps()
        {
            Debug.Log("Clearing shnaps");
            shnapsController.ClearShnaps();
            isFirstShnapsFound = true;
        }
    }
}