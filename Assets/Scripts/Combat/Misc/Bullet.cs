using Combat.Arena;
using UnityEngine;
using Zenject;

namespace Combat.Misc
{
    [RequireComponent(typeof(DamageOnCollision))]
    public class Bullet : MonoBehaviour
    {
        [SerializeField] protected float speed = 5f;
        [SerializeField] protected float lifeTime = 5f;
        
        [Inject] protected CombatArena _arena;

        public void SetSpeed(float speed)
        {
            this.speed = speed;
        }

        private void Start()
        {
            Destroy(gameObject, lifeTime);
            GetComponent<DamageOnCollision>().OnDamage += _ => Destroy(gameObject);
            
            CalculateBulletDirection();
        }

        protected virtual void CalculateBulletDirection()
        {
        }

        private void Update()
        {
            TranslateBullet();
        }

        protected virtual void TranslateBullet()
        {
            transform.Translate(Vector3.up * (speed * Time.deltaTime));
        }
    }
}