
using UI.Interfaces;
using UnityEngine;
using Zenject;

namespace UI
{
    [RequireComponent(typeof(Canvas))]
    public class ConfigurableCanvas : MonoBehaviour, IConfigurableCanvas
    {
        [Inject] private readonly IUIFacade uiFacade;
        
        [HideInInspector] public Canvas canvas;
        [SerializeField] private bool closableWithEscape = true;
        [SerializeField] private bool pausesTime = true;

        public void Awake()
        {
            canvas = GetComponent<Canvas>();
        }

        public bool ClosableWithEscape()
        {
            return closableWithEscape;
        }

        public bool PausesTime()
        {
            return pausesTime;
        }
        
        public void Open()
        {
            canvas.gameObject.SetActive(true);
        }
        
        public void Close()
        {
            canvas.gameObject.SetActive(false);
        }
    }
}