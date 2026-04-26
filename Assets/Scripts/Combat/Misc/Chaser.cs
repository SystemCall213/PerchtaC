using UnityEngine;

namespace Combat.Misc
{
    public class Chaser : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float maxDistance = 10f;
        protected Vector2 targetPos;
        protected Rigidbody2D rb;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Accelerate()
        {
            rb.AddForce(Vector2.right * speed);
        }
    }
}