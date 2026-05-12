using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Combat.HealthUI
{
    public class BossHealthPieceUI : MonoBehaviour
    {
        [SerializeField] private float impulse = 0.25f;
        [SerializeField] private GameObject bottomRopePiece;
        [SerializeField] private Rigidbody2D bottomRopeRb;
        [SerializeField] private HingeJoint2D hingeJoint;
        
        public void Destroy()
        {
            CancellationToken ct = this.GetCancellationTokenOnDestroy();
            DestroyAsync(ct).Forget();
        }

        private async UniTaskVoid DestroyAsync(CancellationToken ct)
        {
            hingeJoint.enabled = false;
            
            float torque = Random.value > 0.5f 
                ? Random.Range(0.1f, impulse) 
                : Random.Range(-impulse, -0.1f);
            
            bottomRopeRb.AddTorque(torque, ForceMode2D.Impulse);
    
            if (!ct.IsCancellationRequested)
            {
                await UniTask.Delay(5000, cancellationToken: ct);
            }
        
            if (gameObject != null)
            {
                Destroy(bottomRopePiece);
            }
        }
    }
}