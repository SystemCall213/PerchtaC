using System;
using UnityEngine;
using Zenject;

namespace Glyph
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class GlyphRenderer : MonoBehaviour
    {
        [SerializeField] private Shader brushShader;
        
        [Inject] private GlyphCompletionTracker glyphCompletionTracker;
        [Inject] private GlyphFacade _glyphFacade;
        
        private RenderTexture _maskTexture;
        private Material _glyphMaterial;
        private SpriteRenderer _spriteRenderer;
        private Material _brushMaterial;
        public RenderTexture MaskTexture => _maskTexture;
        public Sprite Sprite => _spriteRenderer.sprite;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            
            if (brushShader != null)
            {
                _brushMaterial = new Material(brushShader);
            }
            
            _glyphFacade.OnGlyphPainted += InitializeNextGlyph;
            
            InitializeNextGlyph();
        }

        private void InitializeNextGlyph(int _damage = 0)
        {
            var glyphSO = _glyphFacade.GetNextGlyph();
            if (glyphSO != null)
            {
                _spriteRenderer.sprite = glyphSO.glyphSprite;
            }

            int maskResolution = 256;
            if (_spriteRenderer.sprite != null && _spriteRenderer.sprite.texture != null)
            {
                maskResolution = Mathf.Max(_spriteRenderer.sprite.texture.width, _spriteRenderer.sprite.texture.height);
            }
            
            if (_maskTexture == null || _maskTexture.width != maskResolution || _maskTexture.height != maskResolution)
            {
                if (_maskTexture != null)
                {
                    _maskTexture.Release();
                    Destroy(_maskTexture);
                }
                _maskTexture = new RenderTexture(maskResolution, maskResolution, 0, RenderTextureFormat.R8);
                _maskTexture.Create();
            }
            
            RenderTexture.active = _maskTexture;
            GL.Clear(true, true, Color.black);
            RenderTexture.active = null;

            _glyphMaterial = _spriteRenderer.material;
            _glyphMaterial.SetTexture("_MaskTex", _maskTexture);
        }

        public void Paint(Vector2 uv, float radius, float hardness, float strength)
        {
            if (_brushMaterial == null) return;

            _brushMaterial.SetVector("_PaintUV", new Vector4(uv.x, uv.y, 0, 0));
            _brushMaterial.SetFloat("_Radius", radius);
            _brushMaterial.SetFloat("_Hardness", hardness);
            _brushMaterial.SetFloat("_Strength", strength);

            RenderTexture temp = RenderTexture.GetTemporary(_maskTexture.width, _maskTexture.height, 0, _maskTexture.format);
            Graphics.Blit(_maskTexture, temp);
            Graphics.Blit(temp, _maskTexture, _brushMaterial);
            RenderTexture.ReleaseTemporary(temp);
        }

        private void OnDestroy()
        {
            if (_glyphFacade != null)
            {
                _glyphFacade.OnGlyphPainted -= InitializeNextGlyph;
            }

            if (_maskTexture != null)
            {
                _maskTexture.Release();
                Destroy(_maskTexture);
            }
        }
    }
}