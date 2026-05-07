using UnityEngine;

namespace Combat.Misc
{
    public class CenterPointingBullet : Bullet
    {
        [SerializeField] private float randomDeviationX = 0f;
        [SerializeField] private float randomDeviationY = 0f;

        protected override void CalculateBulletDirection()
        {
            Vector2 targetPos = _arena.transform.position;
            
            // Add random deviation to target position
            targetPos.x += Random.Range(-randomDeviationX, randomDeviationX);
            targetPos.y += Random.Range(-randomDeviationY, randomDeviationY);
            
            Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
            transform.up = direction;
        }
    } 
}