using UnityEngine;
using UnityEngine.Serialization;

namespace Glyph
{
    public class PlayerPainting : MonoBehaviour
    {
        [SerializeField] private float brushRadius = 0.5f;
        [SerializeField] private float brushHardness = 0.5f;
        [SerializeField] private float brushStrength = 1.0f;
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.TryGetComponent<GlyphRenderer>(out var glyphRenderer))
            {
                Vector3 localPos = other.transform.InverseTransformPoint(transform.position);
                
                Bounds bounds = other.bounds;
                Vector3 min = other.transform.InverseTransformPoint(bounds.min);
                Vector3 max = other.transform.InverseTransformPoint(bounds.max);

                float u = Mathf.InverseLerp(min.x, max.x, localPos.x);
                float v = Mathf.InverseLerp(min.y, max.y, localPos.y);

                glyphRenderer.Paint(new Vector2(u, v), brushRadius, brushHardness, brushStrength);
            }
        }
    }
}