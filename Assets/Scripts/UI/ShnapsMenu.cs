using System;
using DefaultNamespace.Shnaps.Interfaces;
using DG.Tweening;
using UI.Interfaces;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace UI
{
    public class ShnapsMenu : ConfigurableCanvas
    {
        [Inject] private readonly IUIFacade uiFacade;
        [Inject] private readonly IShnapsFacade shnapsFacade;
        
        private CanvasGroup canvasGroup;

        private void Awake()
        { 
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            canvasGroup.DOFade(1f, 0.5f).OnComplete(() => canvasGroup.blocksRaycasts = true);
        }
        
        private void OnShnapsFound()
        {
            uiFacade.Open(this);
        }
        
        public void CloseMenu()
        {
            canvasGroup.DOFade(0f, 0.5f).OnComplete(() =>
            {
                canvasGroup.blocksRaycasts = false;
                uiFacade.CloseTopmost();
            });
        }
    } 
}