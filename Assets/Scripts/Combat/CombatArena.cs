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
            float w = width * 2;
            float h = height * 2;

            int side = Random.Range(0, 4);
            float x = 0, y = 0;

            switch (side)
            {
                case 0:
                    x = Random.Range(-w / 2f, w / 2f);
                    y = h / 2f;
                    break;
                case 1:
                    x = Random.Range(-w / 2f, w / 2f);
                    y = -h / 2f;
                    break;
                case 2:
                    x = -w / 2f;
                    y = Random.Range(-h / 2f, h / 2f);
                    break;
                case 3:
                    x = w / 2f;
                    y = Random.Range(-h / 2f, h / 2f);
                    break;
            }

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
            Gizmos.DrawWireCube(transform.position, new Vector3(width * 2, height  * 2, 0));
        }
    }
}
