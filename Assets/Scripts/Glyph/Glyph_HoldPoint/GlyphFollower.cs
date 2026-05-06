using System;
using Combat.Interfaces;
using UnityEngine;

namespace Glyph.Glyph_HoldPoint
{
    public class GlyphFollower : MonoBehaviour
    {
        [SerializeField] private float radius;
        [SerializeField] private float moveSpeed;
        
        private Vector3[] points;
        private int currentPointIndex = 0;
        private IPlayerMovement player;
        private GlyphProgressTracker glyphProgressTracker;
        private Action onReachedEnd;

        public void Initialize(Vector3[] linePoints, IPlayerMovement playerMovement, GlyphProgressTracker progressTracker, Action onComplete)
        {
            points = linePoints;
            player = playerMovement;
            glyphProgressTracker = progressTracker;
            onReachedEnd = onComplete;

            if (points != null && points.Length > 0)
            {
                transform.position = points[0];
                currentPointIndex = 1;
            }
        }

        private void Update()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.Position);

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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
