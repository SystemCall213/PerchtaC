using System;
using UnityEngine;

namespace Combat.Misc
{
    public class Bottle : MonoBehaviour
    {
        [SerializeField] private float minImpulse = 5f;
        [SerializeField] private float maxImpulse = 10f;
        private float impulse => UnityEngine.Random.Range(minImpulse, maxImpulse);
        [SerializeField] private float lifetime = 10f;

        private void Start()
        {
            Destroy(gameObject, lifetime);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag($"BottomWall"))
            {
                Vector2 direction = Vector2.up;
                direction.x = UnityEngine.Random.Range(-1f, 1f);
                direction.Normalize();
                gameObject.GetComponent<Rigidbody2D>().AddForce(direction * impulse, ForceMode2D.Impulse);
                gameObject.GetComponent<Collider2D>().excludeLayers += LayerMask.GetMask("BottomWall");
            }
        }
    }
}