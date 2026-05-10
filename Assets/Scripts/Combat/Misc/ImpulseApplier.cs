using System;
using UnityEngine;

namespace Combat.Misc
{
    public class ImpulseApplier : MonoBehaviour
    {
        [SerializeField] protected float impulse = 10f;
        [SerializeField] protected Vector2 direction = Vector2.up;
        
        private Rigidbody2D rb;

        private void Awake()
        {
            Configure();
        }

        private void Start()
        {
            ApplyImpulse();
        }
        
        protected virtual void Configure()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        protected virtual void ApplyImpulse()
        {
            rb.AddForce(direction * impulse, ForceMode2D.Impulse);
        }
        
    }
}