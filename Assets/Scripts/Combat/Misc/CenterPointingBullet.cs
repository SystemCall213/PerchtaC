using UnityEngine;

namespace Combat.Misc
{
    public class CenterPointingBullet : Bullet
    {
        protected override void CalculateBulletDirection()
        {
            Vector2 targetPos = _arena.transform.position;
            Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
            transform.up = direction;
        }
    } 
}