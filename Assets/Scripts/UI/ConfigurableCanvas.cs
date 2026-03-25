
using UI.Interfaces;
using UnityEngine;
using Zenject;

namespace UI
{
    [RequireComponent(typeof(Canvas))]
    public class ConfigurableCanvas : MonoBehaviour
    {
        [Inject] private readonly IUIFacade uiFacade;
        
        [HideInInspector] public Canvas canvas;
        public bool ClosableWithEscape = true;
        public bool PausesTime = true;

        public void Awake()
        {
            canvas = GetComponent<Canvas>();
        }

        public void Open()
        {
            uiFacade.Open(this);
        }
    }
}