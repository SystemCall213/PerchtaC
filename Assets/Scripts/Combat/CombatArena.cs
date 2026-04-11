using UnityEngine;

namespace Combat
{
    public class CombatArena : MonoBehaviour
    {
        [SerializeField] private float width = 10f;
        [SerializeField] private float height = 10f;

        public float Width => width;
        public float Height => height;

        public Vector2 GetRandomPositionOutside()
        {
            float radius = width * 1.5f;
            float angle = Random.Range(0f, Mathf.PI * 2f);
            
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            return (Vector2)transform.position + new Vector2(x, y);
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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, new Vector3(width, height, 0));
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, new Vector3(width / 2f, height / 2f, 0));
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, width * 1.5f);
        }
    }
}
