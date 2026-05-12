using System;
using Combat.Interfaces;
using UnityEngine;

namespace Glyph.Glyph_HoldPoint
{
    public class GlyphFollower : MonoBehaviour
    {
        [SerializeField] private float radius;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float rotationSpeedZ = 45f;
        [SerializeField] private int maskResolution = 512;
        [SerializeField] private float paintRadius = 0.2f;
        
        private Vector3[] points;
        private int currentPointIndex = 0;
        private IPlayerMovement player;
        private GlyphProgressTracker glyphProgressTracker;
        private Action onReachedEnd;
        private LineRenderer lineRenderer;
        private SpriteMask spriteMask;
        private Texture2D maskTexture;
        private Color32[] maskPixels;
        private Sprite maskSprite;
        private Rect localMaskRect;

        public void Initialize(LineRenderer line, IPlayerMovement playerMovement, GlyphProgressTracker progressTracker, Action onComplete)
        {
            lineRenderer = line;
            Vector3[] positions = new Vector3[line.positionCount];
            line.GetPositions(positions);
            
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = line.transform.TransformPoint(positions[i]);
            }
            points = positions;
            player = playerMovement;
            glyphProgressTracker = progressTracker;
            onReachedEnd = onComplete;
            InitializeMask(line);

            if (points != null && points.Length > 0)
            {
                transform.position = points[0];
                currentPointIndex = 1;
                EraseAtFollowerPosition();
            }
        }

        private void InitializeMask(LineRenderer line)
        {
            spriteMask = line.GetComponent<SpriteMask>();
            if (spriteMask == null)
            {
                Debug.LogWarning($"Glyph line {line.name} has no SpriteMask component.", line);
                return;
            }

            int resolution = Mathf.Max(1, maskResolution);
            maskTexture = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
            maskTexture.filterMode = FilterMode.Point;
            maskTexture.wrapMode = TextureWrapMode.Clamp;

            maskPixels = new Color32[resolution * resolution];
            for (int i = 0; i < maskPixels.Length; i++)
            {
                maskPixels[i] = new Color32(255, 255, 255, 255);
            }
            maskTexture.SetPixels32(maskPixels);
            maskTexture.Apply(false);

            localMaskRect = CalculateLocalMaskRect(line);
            float pixelsPerUnit = resolution / localMaskRect.width;
            Vector2 pivot = new Vector2(
                Mathf.InverseLerp(localMaskRect.xMin, localMaskRect.xMax, 0f),
                Mathf.InverseLerp(localMaskRect.yMin, localMaskRect.yMax, 0f));

            maskSprite = Sprite.Create(maskTexture, new Rect(0, 0, resolution, resolution), pivot, pixelsPerUnit);
            spriteMask.sprite = maskSprite;
        }

        private Rect CalculateLocalMaskRect(LineRenderer line)
        {
            Vector3[] positions = new Vector3[line.positionCount];
            line.GetPositions(positions);

            if (positions.Length == 0)
            {
                return new Rect(-0.5f, -0.5f, 1f, 1f);
            }

            float minX = positions[0].x;
            float maxX = positions[0].x;
            float minY = positions[0].y;
            float maxY = positions[0].y;

            for (int i = 1; i < positions.Length; i++)
            {
                minX = Mathf.Min(minX, positions[i].x);
                maxX = Mathf.Max(maxX, positions[i].x);
                minY = Mathf.Min(minY, positions[i].y);
                maxY = Mathf.Max(maxY, positions[i].y);
            }

            float padding = Mathf.Max(line.widthMultiplier, paintRadius);
            float width = Mathf.Max(maxX - minX + padding * 2f, 0.01f);
            float height = Mathf.Max(maxY - minY + padding * 2f, 0.01f);
            float size = Mathf.Max(width, height);
            Vector2 center = new Vector2((minX + maxX) * 0.5f, (minY + maxY) * 0.5f);

            return new Rect(center.x - size * 0.5f, center.y - size * 0.5f, size, size);
        }

        private void Update()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.Position);
            
            transform.Rotate(0, 0, rotationSpeedZ * Time.deltaTime);
            
            if (distanceToPlayer <= radius)
            {
                MoveTowardsNextPoint();
            }
            else
            {
                float distanceFromOutsideRadius = Vector3.Distance(glyphProgressTracker.transform.position, player.Position);
                if(distanceFromOutsideRadius >= glyphProgressTracker.GlyphResetRadius)
                {
                    MoveBackwards();
                }
            }
        }

        private void MoveTowardsNextPoint()
        {
            Vector3 targetPosition = points[currentPointIndex];
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            EraseAtFollowerPosition();

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                currentPointIndex++;

                if (currentPointIndex >= points.Length)
                {
                    onReachedEnd?.Invoke();
                    Destroy(gameObject);
                }
            }
        }

        private void MoveBackwards()
        {
            if (currentPointIndex == 0)
            {
                return;
            }
            Vector3 targetPosition = points[currentPointIndex-1];
            FillAtFollowerPosition();
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                currentPointIndex--;
                if (currentPointIndex < 0)
                {
                    currentPointIndex = 0;
                }
            }
        }

        private void EraseAtFollowerPosition()
        {
            PaintAtFollowerPosition(new Color32(0, 0, 0, 0));
        }

        private void FillAtFollowerPosition()
        {
            PaintAtFollowerPosition(new Color32(255, 255, 255, 255));
        }

        private void PaintAtFollowerPosition(Color32 color)
        {
            if (lineRenderer == null || maskTexture == null || maskPixels == null)
            {
                return;
            }

            Vector3 localPosition = lineRenderer.transform.InverseTransformPoint(transform.position);
            float u = Mathf.InverseLerp(localMaskRect.xMin, localMaskRect.xMax, localPosition.x);
            float v = Mathf.InverseLerp(localMaskRect.yMin, localMaskRect.yMax, localPosition.y);
            int centerX = Mathf.RoundToInt(u * (maskTexture.width - 1));
            int centerY = Mathf.RoundToInt(v * (maskTexture.height - 1));
            int radiusPixels = Mathf.CeilToInt(paintRadius / localMaskRect.width * maskTexture.width);

            PaintCircle(centerX, centerY, radiusPixels, color);
        }

        private void PaintCircle(int centerX, int centerY, int radiusPixels, Color32 color)
        {
            int minX = Mathf.Max(0, centerX - radiusPixels);
            int maxX = Mathf.Min(maskTexture.width - 1, centerX + radiusPixels);
            int minY = Mathf.Max(0, centerY - radiusPixels);
            int maxY = Mathf.Min(maskTexture.height - 1, centerY + radiusPixels);
            int radiusSqr = radiusPixels * radiusPixels;
            bool changed = false;

            for (int y = minY; y <= maxY; y++)
            {
                int offsetY = y - centerY;
                for (int x = minX; x <= maxX; x++)
                {
                    int offsetX = x - centerX;
                    if (offsetX * offsetX + offsetY * offsetY > radiusSqr)
                    {
                        continue;
                    }

                    int pixelIndex = y * maskTexture.width + x;
                    if (maskPixels[pixelIndex].a == color.a)
                    {
                        continue;
                    }

                    maskPixels[pixelIndex] = color;
                    changed = true;
                }
            }

            if (!changed)
            {
                return;
            }

            maskTexture.SetPixels32(maskPixels);
            maskTexture.Apply(false);
        }

        private void OnDestroy()
        {
            if (spriteMask != null && spriteMask.sprite == maskSprite)
            {
                spriteMask.sprite = null;
            }

            if (maskSprite != null)
            {
                Destroy(maskSprite);
                maskSprite = null;
            }

            if (maskTexture != null)
            {
                Destroy(maskTexture);
                maskTexture = null;
            }
            lineRenderer = null;
            maskPixels = null;
            
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
