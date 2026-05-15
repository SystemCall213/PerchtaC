using DefaultNamespace.Shnaps.Interfaces;
using UnityEngine;
using Zenject;

namespace DefaultNamespace.Shnaps
{
    public class ShnapsInteractable : MonoBehaviour
    {
        [Inject] private IShnapsController shnapsController;
        
        public void OnInteract()
        {
            shnapsController.AddShnaps();
        }
    }
}