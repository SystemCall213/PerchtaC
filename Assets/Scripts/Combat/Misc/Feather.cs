using UnityEngine;

namespace Combat.Misc
{
    [RequireComponent(typeof(DamageOnCollision))]
    public class Feather : MonoBehaviour
    {
        [SerializeField] protected float speed = 5f;
        [SerializeField] protected float lifeTime = 10f;
        [SerializeField] protected float swingAmplitude = 1f;
        [SerializeField] protected float swingFrequency = 2f;
        [SerializeField] protected float rotationAmplitude = 30f;

        private float _startTime;
        private Vector3 _startPosition;
        
        private void Start()
        {
            _startTime = Time.time;
            _startPosition = transform.position;
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
            
            // Vertical movement
            _startPosition += Vector3.down * (speed * Time.deltaTime);
            
            // Horizontal swing
            float horizontalOffset = Mathf.Sin(elapsed * swingFrequency) * swingAmplitude;
            
            // Update position
            transform.position = _startPosition + Vector3.right * horizontalOffset;
            
            // Rotation swing
            float rotationOffset = Mathf.Cos(elapsed * swingFrequency) * rotationAmplitude;
            transform.rotation = Quaternion.Euler(0, 0, rotationOffset);
        }
    }
}