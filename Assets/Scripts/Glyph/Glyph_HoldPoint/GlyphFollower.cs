using System;
using Combat;
using UnityEngine;
using Zenject;

namespace Glyph.Glyph_HoldPoint
{
    public class GlyphFollower : MonoBehaviour
    {
        [SerializeField] private float radius;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float spinSpeed = 100f;
        
        private Vector3[] points;
        private int currentPointIndex = 0;
        private PlayerMovement player;
        private GlyphProgressTracker glyphProgressTracker;
        private Action onReachedEnd;

        public void Initialize(Vector3[] linePoints, PlayerMovement playerMovement, GlyphProgressTracker progressTracker, Action onComplete)
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
            Spin();

            if (player == null || points == null || currentPointIndex >= points.Length)
                return;

            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= radius)
            {
                MoveTowardsNextPoint();
            }
            else
            {
                float distanceFromOutsideRadius = Vector3.Distance(glyphProgressTracker.transform.position, player.transform.position);
                if(distanceFromOutsideRadius >= glyphProgressTracker.GlyphResetRadius)
                {
                    MoveBackwards();
                }
            }
        }

        private void Spin()
        {
            transform.Rotate(Vector3.forward, spinSpeed * Time.deltaTime);
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
