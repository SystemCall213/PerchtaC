using System;
using UnityEngine;

namespace Combat.Misc
{
    public class DamageOnCollision : MonoBehaviour
    {
        public int damage = 1;
        
        public event Action<int> OnDamage;
        protected void OnCollisionEnter2D(Collision2D collision)
        {
            ApplyDamageOnCollision(collision);
        }

        private void ApplyDamageOnCollision(Collision2D collision)
        {
            if (collision == null) return;
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null && !playerHealth.IsDead() && !playerHealth.IsImmune)
            {
                playerHealth.TakeDamage(damage);
                OnDamage?.Invoke(damage);
            }
        }
    }
}