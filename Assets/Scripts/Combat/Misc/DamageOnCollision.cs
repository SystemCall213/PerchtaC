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
            ApplyDamageOnCollision(collision.gameObject);
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            ApplyDamageOnCollision(other.gameObject);
        }

        public void ApplyDamageOnCollision(GameObject other)
        {
            if (other == null) return;
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null && !playerHealth.IsDead() && !playerHealth.IsImmune)
            {
                playerHealth.TakeDamage(damage);
                OnDamage?.Invoke(damage);
            }
        }
    }
}