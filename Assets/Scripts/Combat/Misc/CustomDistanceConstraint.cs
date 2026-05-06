using UnityEngine;

namespace Combat.Misc
{
    public class CustomDistanceConstraint : MonoBehaviour
    {
        [SerializeField] private GameObject target;
        [SerializeField] private float maxDistance = 10f;
        [SerializeField] private float damping = 0.3f;
        private void FixedUpdate()
        {
            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance < maxDistance) return;
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, (distance - maxDistance) * damping);
            transform.rotation = Quaternion.Slerp(transform.rotation, target.transform.rotation, Time.fixedDeltaTime * 3);
        }
        
        private void OnDrawGizmos()
        {
            if (target == null) return;
            Gizmos.DrawLine(transform.position, target.transform.position);
            Gizmos.DrawWireSphere(target.transform.position, maxDistance);
        }
    }
}