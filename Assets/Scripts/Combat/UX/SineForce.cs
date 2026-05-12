using UnityEngine;

namespace Combat.UX
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class SineForce : MonoBehaviour
    {
        [SerializeField] private float force;
        [SerializeField] private float frequency;
        
        private Rigidbody2D rb;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        private void FixedUpdate()
        {
            rb.AddForce(new Vector2(Mathf.Sin(Time.time * frequency) * force, Mathf.Cos(Time.time * frequency) * force), ForceMode2D.Impulse);
        }
    }
}