using System;
using Combat.Interfaces;
using UnityEngine;
using Zenject;

namespace Combat
{
    public class PlayerMovement : MonoBehaviour, IPlayerMovement
    {
        [SerializeField] private float moveSpeed = 5f;
        [Inject] private readonly DefaultActions defaultActions;
        private Rigidbody2D rb;

        public Vector2 Position => transform.position;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 0f;
            }
        }

        private void FixedUpdate()
        {
            if (defaultActions == null) return;
            Vector2 movementInput = defaultActions.Combat.Movement.ReadValue<Vector2>();
            rb.velocity = movementInput * moveSpeed;
        }
    }
}