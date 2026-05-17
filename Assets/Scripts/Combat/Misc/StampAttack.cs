using UnityEngine;

namespace Combat.Misc
{
    public class StampAttack : Pillow
    {
        [SerializeField] private float glassSpeed;
        protected override void SpawnPrefab()
        {
            float radius = transform.localScale.x * 1.5f;
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, radius);
            
            foreach (var col in hitPlayers)
            {
                if (col.TryGetComponent<PlayerHealth>(out var playerHealth))
                {
                    playerHealth.TakeDamage(damage);
                }
            }

            float angleStep = 360f / spawnCount;
            for (int i = 0; i < spawnCount; i++)
            {
                float angle = i * angleStep;
                Quaternion rotation = Quaternion.Euler(0, 0, angle);
                GameObject bulletObj = _instantiator.InstantiatePrefab(prefabsToSpawn[i % prefabsToSpawn.Count], transform.position, rotation, null);
                
                if (bulletObj.TryGetComponent<Bullet>(out var bullet))
                {
                    bullet.SetSpeed(glassSpeed);
                }
            }
        }
    }
}