using UnityEngine;

namespace Combat.Misc
{
    [RequireComponent(typeof(DamageOnCollision))]
    public class Feather : MonoBehaviour
    {
        [SerializeField] protected float speed = 5f;
        [SerializeField] protected float lifeTime = 10f;
        [SerializeField] protected float pendulumLength = 2f;
        [SerializeField] protected float swingFrequency = 2f;
        [SerializeField] protected float rotationAmplitude = 30f;

        private float _startTime;
        private Vector3 _pivotPosition;
        
        private void Start()
        {
            _startTime = Time.time;
            // The pivot starts above the feather
            _pivotPosition = transform.position + Vector3.up * pendulumLength;
            Destroy(gameObject, lifeTime);
            GetComponent<DamageOnCollision>().OnDamage += _ => Destroy(gameObject);
        }

        private void Update()
        {
            TranslateFeather();
        }

        protected virtual void TranslateFeather()
        {
            float elapsed = Time.time - _startTime;
            
            // Pivot movement downwards
            _pivotPosition += Vector3.down * (speed * Time.deltaTime);
            
            // Calculate swing angle
            float angle = Mathf.Sin(elapsed * swingFrequency) * rotationAmplitude;
            
            // Calculate position relative to pivot
            // 0 degrees is straight down (Vector3.down)
            Vector3 offset = Quaternion.Euler(0, 0, angle) * (Vector3.down * pendulumLength);
            
            // Update position
            transform.position = _pivotPosition + offset;
            
            // Rotation swing matches the angle
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}