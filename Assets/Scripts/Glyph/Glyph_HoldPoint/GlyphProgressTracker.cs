using System;
using System.Collections.Generic;
using Combat;
using Glyph.Interfaces;
using UnityEngine;
using Zenject;

namespace Glyph.Glyph_HoldPoint
{
    public class GlyphProgressTracker : MonoBehaviour
    { 
        [Inject] private GlyphFollower glyphFollowerPrefab;
        [Inject] private PlayerMovement player;
        [Inject] private IGlyphFacade glyphFacade;
        [Inject] private IInstantiator instantiator;
        [Inject(Id = "GlyphLines")] private List<LineRenderer> glyphLineRenderers;
        
        [SerializeField] private float glyphResetRadius = 250f;
        public float GlyphResetRadius => glyphResetRadius;
        
        private LineRenderer lineRenderer;
        private int currentGlyphIndex;
        

        private void Start()
        {
            UpdateLineRenderer(0);
            glyphFacade.OnGlyphPainted += UpdateLineRenderer;
        }

        public void UpdateLineRenderer(int damage)
        {
            
            SpawnLineRenderer();
            SpawnFollower();
        }

        private void SpawnLineRenderer()
        {
            if (glyphLineRenderers.Count == 0) return;
            if (currentGlyphIndex >= glyphLineRenderers.Count)
            {
                currentGlyphIndex = 0;
            }
            LineRenderer lineRend = glyphLineRenderers[currentGlyphIndex++];
            Destroy(lineRenderer);
            lineRenderer = Instantiate(lineRend, transform);
        }

        private void SpawnFollower()
        {
            if (lineRenderer == null || lineRenderer.positionCount == 0 || glyphFollowerPrefab == null) return;

            GlyphFollower follower = instantiator.InstantiatePrefabForComponent<GlyphFollower>(glyphFollowerPrefab);

            Vector3[] positions = new Vector3[lineRenderer.positionCount];
            lineRenderer.GetPositions(positions);
            
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = lineRenderer.transform.TransformPoint(positions[i]);
            }

            follower.Initialize(positions, player,this, () => {
                glyphFacade.TriggerGlyphPainted();});
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, glyphResetRadius);
        }
    }
}