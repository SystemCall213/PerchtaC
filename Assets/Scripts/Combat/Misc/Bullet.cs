using UnityEngine;

namespace Combat.Misc
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float lifeTime = 5f;
        [SerializeField] private int damage = 1;

        public void SetSpeed(float speed)
        {
            this.speed = speed;
        }

        private void Start()
        {
            Destroy(gameObject, lifeTime);
        }

        private void Update()
        {
            transform.Translate(Vector3.up * (speed * Time.deltaTime));
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<PlayerHealth>(out var playerHealth))
            {
                if (playerHealth.IsDead() || playerHealth.IsImmune) return;
                playerHealth.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}