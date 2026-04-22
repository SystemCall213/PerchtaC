using UnityEngine;

namespace Combat.Misc
{
    [RequireComponent(typeof(DamageOnCollision))]
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
            GetComponent<DamageOnCollision>().OnDamage += _ => Destroy(gameObject);
        }

        private void Update()
        {
            transform.Translate(Vector3.up * (speed * Time.deltaTime));
        }
    }
}