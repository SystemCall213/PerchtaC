using System;
using Cysharp.Threading.Tasks;
using DefaultNamespace.Shnaps.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace DefaultNamespace.Shnaps
{
    public class ShnapsCollector : MonoBehaviour
    {
        [Inject] private readonly IShnapsFacade shnapsFacade;
        
        private Button button;
        
        private void Awake()
        {
            button = GetComponent<Button>();
        }
        
        public void CollectShnaps()
        {
            shnapsFacade.TakeShnaps();
            button.interactable = false;
            button.onClick.RemoveAllListeners();
        }
    }
}