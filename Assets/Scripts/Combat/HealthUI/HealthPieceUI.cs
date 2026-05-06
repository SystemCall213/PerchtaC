using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Combat.HealthUI
{
    public class HealthPieceUI : MonoBehaviour
    {
        public void Destroy()
        {
            DestroyAsync().Forget();
        }

        private async UniTaskVoid DestroyAsync()
        {
            if (transform is RectTransform rectTransform)
            {
                rectTransform.DOScale(Vector3.zero, 2f);
            }
            else
            {
                transform.DOScale(Vector3.zero, 2f);
            }
        
            await UniTask.Delay(2000);
            
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }
    }
}