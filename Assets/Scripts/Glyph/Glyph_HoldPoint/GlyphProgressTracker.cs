using System;
using System.Collections.Generic;
using Combat.Interfaces;
using Glyph.Interfaces;
using UnityEngine;
using Zenject;

namespace Glyph.Glyph_HoldPoint
{
    public class GlyphProgressTracker : MonoBehaviour
    { 
        [Inject] private GlyphFollower glyphFollowerPrefab;
        [Inject] private IPlayerMovement player;
        [Inject] private IGlyphFacade glyphFacade;
        [Inject] private IInstantiator instantiator;
        [Inject(Id = "GlyphLines")] private List<LineRenderer> glyphLineRenderers;
        
        [SerializeField] private float glyphResetRadius = 3;
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
            if (lineRenderer != null)
            {
                Destroy(lineRenderer.gameObject);
                lineRenderer = null;
            }
            lineRenderer = Instantiate(lineRend, transform);
        }

        private void SpawnFollower()
        {
            if (lineRenderer == null || lineRenderer.positionCount == 0 || glyphFollowerPrefab == null) return;

            GlyphFollower follower = instantiator.InstantiatePrefabForComponent<GlyphFollower>(glyphFollowerPrefab);
            

            follower.Initialize(lineRenderer, player,this, () => {
                glyphFacade.TriggerGlyphPainted();});
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, glyphResetRadius);
        }
    }
}