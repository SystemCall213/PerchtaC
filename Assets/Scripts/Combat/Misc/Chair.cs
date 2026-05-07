using UnityEngine;

namespace Combat.Misc
{
    [RequireComponent(typeof(DamageOnCollision))]
    public class Chair : MonoBehaviour
    {
        [SerializeField] protected float lifeTime = 5f;

        private void Start()
        {
            float randomRotation = Random.Range(0f, 360f);
            transform.Rotate(0, 0, randomRotation);
            
            Destroy(gameObject, lifeTime);
        }
    }
}