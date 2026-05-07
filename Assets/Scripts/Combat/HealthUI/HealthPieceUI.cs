using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Combat.HealthUI
{
    public class HealthPieceUI : MonoBehaviour
    {
        public void Destroy()
        {
            CancellationToken ct = this.GetCancellationTokenOnDestroy();
            DestroyAsync(ct).Forget();
        }

        private async UniTaskVoid DestroyAsync(CancellationToken ct)
        {
            if (transform is RectTransform rectTransform)
            {
                rectTransform.DOScale(Vector3.zero, 2f);
            }
            else
            {
                transform.DOScale(Vector3.zero, 2f);
            }
    
            if (!ct.IsCancellationRequested)
            {
                await UniTask.Delay(2000, cancellationToken: ct);
            }
        
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }
    }
}