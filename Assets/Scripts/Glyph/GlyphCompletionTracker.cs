using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Glyph
{
    public class GlyphCompletionTracker : IInitializable, IDisposable
    {
        [Inject] private readonly GlyphRenderer _glyphRenderer;
        private readonly GlyphFacade _glyphFacade;
        private readonly float _completionThreshold;

        private Texture2D _readableMask;
        private bool _isCompleted;
        private int _totalGlyphPixels = -1;
        
        private bool[] _glyphPixelMap;
        private int _maskWidth;
        private int _maskHeight;
        private const float CheckInterval = 0.1f;
        private CancellationTokenSource _cts;

        public GlyphCompletionTracker(GlyphFacade glyphFacade, [InjectOptional] float completionThreshold = 0.9f)
        {
            _glyphFacade = glyphFacade;
            _completionThreshold = completionThreshold;
        }

        public void Initialize()
        {
            _cts = new CancellationTokenSource();
            InitializeNewGlyph();
            Debug.Log("new glyph initialized");
            StartCheckingLoop(_cts.Token).Forget();
        }

        public void InitializeNewGlyph()
        {
            Sprite sprite = _glyphRenderer.Sprite;
            PrepareGlyphPixelMap(sprite);
            PrepareReadableMask();
            _isCompleted = false;
        }

        private void PrepareReadableMask()
        {
            RenderTexture maskRT = _glyphRenderer.MaskTexture;
            if (maskRT == null) return;
            
            if (_readableMask == null || _readableMask.width != maskRT.width || _readableMask.height != maskRT.height)
            {
                if (_readableMask != null) UnityEngine.Object.Destroy(_readableMask);
                _readableMask = new Texture2D(maskRT.width, maskRT.height, TextureFormat.R8, false);
            }
        }

        private async UniTaskVoid StartCheckingLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!_isCompleted)
                {
                    await CheckCompletionAsync(cancellationToken);
                }
                
                await UniTask.Delay(TimeSpan.FromSeconds(CheckInterval), cancellationToken: cancellationToken);
            }
        }

        private async UniTask CheckCompletionAsync(CancellationToken cancellationToken)
        {
            try
            {
                float fillPercent = await CalculateFillPercentageAsync(cancellationToken);
                if (fillPercent >= _completionThreshold)
                {
                    _isCompleted = true;
                    _glyphFacade.TriggerGlyphPainted();
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            if (_readableMask != null)
            {
                UnityEngine.Object.Destroy(_readableMask);
            }
        }


        public async UniTask<float> CalculateFillPercentageAsync(CancellationToken cancellationToken)
        {
            if (_glyphPixelMap == null) return 0f;
            if (_totalGlyphPixels <= 0) return 1f;
            
            UpdateReadableMask();
            
            Color32[] maskPixels = _readableMask.GetPixels32();
            
            await UniTask.SwitchToThreadPool();
            
            int revealedVisiblePixels = 0;
            for (int i = 0; i < maskPixels.Length; i++)
            {
                if (cancellationToken.IsCancellationRequested) return 0;

                if (_glyphPixelMap[i] && maskPixels[i].r > 128)
                {
                    revealedVisiblePixels++;
                }
            }

            float result = (float)revealedVisiblePixels / _totalGlyphPixels;
            
            await UniTask.SwitchToMainThread();
            
            return result;
        }

        private void PrepareGlyphPixelMap(Sprite sprite)
        {
            Texture2D tex = sprite.texture;
            Rect spriteRect = sprite.textureRect;
            RenderTexture maskRT = _glyphRenderer.MaskTexture;
            _maskWidth = maskRT.width;
            _maskHeight = maskRT.height;
            
            _glyphPixelMap = new bool[_maskWidth * _maskHeight];
            _totalGlyphPixels = 0;

            Color[] spritePixels = tex.GetPixels((int)spriteRect.x, (int)spriteRect.y, (int)spriteRect.width, (int)spriteRect.height);
            int spriteW = (int)spriteRect.width;
            int spriteH = (int)spriteRect.height;

            for (int i = 0; i < _glyphPixelMap.Length; i++)
            {
                int x = i % _maskWidth;
                int y = i / _maskWidth;

                float u = (float)x / (_maskWidth - 1);
                float v = (float)y / (_maskHeight - 1);

                int sX = Mathf.Clamp(Mathf.FloorToInt(u * (spriteW - 1)), 0, spriteW - 1);
                int sY = Mathf.Clamp(Mathf.FloorToInt(v * (spriteH - 1)), 0, spriteH - 1);

                if (spritePixels[sY * spriteW + sX].a > 0.1f)
                {
                    _glyphPixelMap[i] = true;
                    _totalGlyphPixels++;
                }
            }
        }

        private void UpdateReadableMask()
        {
            RenderTexture maskRT = _glyphRenderer.MaskTexture;
            if (maskRT == null || _readableMask == null) return;

            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = maskRT;
            _readableMask.ReadPixels(new Rect(0, 0, maskRT.width, maskRT.height), 0, 0);
            _readableMask.Apply(false);
            RenderTexture.active = previous;
        }
    }
}
