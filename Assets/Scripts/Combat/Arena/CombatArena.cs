using UnityEngine;

namespace Combat.Arena
{
    public class CombatArena : MonoBehaviour
    {
        [SerializeField] private float width = 10f;
        [SerializeField] private float height = 10f;
        [SerializeField] private float outerRadius = 5f;

        public float Width => width;
        public float Height => height;
        public float OuterRadius => outerRadius;

        public Vector2 GetRandomPositionOutside(float? customRadius = null)
        {
            float radius =  customRadius.HasValue ? customRadius.Value : outerRadius;
            float angle = Random.Range(0f, Mathf.PI * 2f);
            
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            return (Vector2)transform.position + new Vector2(x, y);
        }

        public Vector2 GetPositionOutside(float radialFactor, float? customRadius = null)
        {
            float angle = Mathf.PI * 2f * radialFactor;
            float radius = customRadius.HasValue ? customRadius.Value : outerRadius;
            return (Vector2)transform.position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        }

        public Vector2 GetSidedPosition(ArenaPositionSideFactor factor, float offset = 0f)
        {
            switch (factor.Side)
            {
                case ArenaPositionSideFactor.SideFactor.Top:
                {
                    return (Vector2)transform.position + new Vector2(width * factor.Factor -  width * 0.5f, height + offset);
                }
                case ArenaPositionSideFactor.SideFactor.Bottom:
                {
                    return (Vector2)transform.position + new Vector2(width * factor.Factor - width * 0.5f, -height - offset);
                }
                case ArenaPositionSideFactor.SideFactor.Left:
                {
                    return (Vector2)transform.position + new Vector2(-width - offset, height * factor.Factor - height * 0.5f);
                }
                case ArenaPositionSideFactor.SideFactor.Right:
                {
                    return (Vector2)transform.position + new Vector2(width + offset, height * factor.Factor - height * 0.5f);
                }
                default:
                {
                    return Vector2.zero;
                }
            }
        }

        public Vector2 GetRandomPointInsideCenter()
        {
            float w = width / 2f;
            float h = height / 2f;
            return (Vector2)transform.position + new Vector2(
                Random.Range(-w / 2f, w / 2f),
                Random.Range(-h / 2f, h / 2f)
            );
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, new Vector3(width, height, 0));
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, new Vector3(width / 2f, height / 2f, 0));
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, outerRadius);
        }
    }
}
