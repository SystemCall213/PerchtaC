using Combat.Interfaces;
using DefaultNamespace;
using UnityEngine;
using Zenject;

namespace Combat.Misc
{
    public class Worm : MonoBehaviour
    {
        [SerializeField] private float turnSpeed = 90f;
        [SerializeField] private float forwardForce = 10f;
        [SerializeField] private Rigidbody2D rb;
        
        [Inject] private readonly IPlayerMovement playerMovement;
        private void FixedUpdate()
        {
            Vector2 direction = playerMovement.Position - transform.position.ConvertToVector2();
            if (direction == Vector2.zero)
            {
                return;
            }

            float angle = Vector2.SignedAngle(direction.normalized, transform.up);
            float torque = Mathf.Clamp(angle, -turnSpeed, turnSpeed);

            rb.AddTorque(torque, ForceMode2D.Force);
            rb.AddForce(-transform.up * forwardForce, ForceMode2D.Force);
        }
    }
}