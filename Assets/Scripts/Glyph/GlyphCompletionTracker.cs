using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Glyph
{
    public class GlyphCompletionTracker : IInitializable, IDisposable
    {
        private readonly GlyphRenderer _glyphRenderer;
        private readonly GlyphFacade _glyphFacade;
        private readonly float _completionThreshold;

        private Texture2D _readableMask;
        private bool _isCompleted;
        private int _totalGlyphPixels = -1;
        
        private bool[] _glyphPixelMap;
        private int _maskWidth;
        private int _maskHeight;
        private float _lastCheckTime;
        private const float CheckInterval = 0.1f;
        private bool _isChecking;
        private CancellationTokenSource _cts;

        public event Action OnGlyphPainted;

        public GlyphCompletionTracker(GlyphRenderer glyphRenderer, GlyphFacade glyphFacade, [InjectOptional] float completionThreshold = 0.5f)
        {
            _glyphRenderer = glyphRenderer;
            _glyphFacade = glyphFacade;
            _completionThreshold = completionThreshold;
        }

        public void Initialize()
        {
            _cts = new CancellationTokenSource();
            StartCheckingLoop(_cts.Token).Forget();
        }

        private async UniTaskVoid StartCheckingLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!_isCompleted && !_isChecking)
                {
                    await CheckCompletionAsync(cancellationToken);
                }
                
                await UniTask.Delay(TimeSpan.FromSeconds(CheckInterval), cancellationToken: cancellationToken);
            }
        }

        private async UniTask CheckCompletionAsync(CancellationToken cancellationToken)
        {
            _isChecking = true;
            try
            {
                float fillPercent = await CalculateFillPercentageAsync(cancellationToken);
                UnityEngine.Debug.Log($"Glyph completion check: {fillPercent}%");
                if (fillPercent >= _completionThreshold)
                {
                    _isCompleted = true;
                    _glyphFacade.TriggerGlyphPainted();
                    Reset();
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                _isChecking = false;
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

        private void Reset()
        {
            _isCompleted = false;
            _totalGlyphPixels = -1;
            _glyphPixelMap = null;
        }

        public async UniTask<float> CalculateFillPercentageAsync(CancellationToken cancellationToken)
        {
            Sprite sprite = _glyphRenderer.Sprite;
            if (sprite == null) return 0;

            if (_glyphPixelMap == null)
            {
                PrepareGlyphPixelMap(sprite);
            }

            if (_totalGlyphPixels <= 0) return 1f;
            
            UpdateReadableMask();

            // ReadPixels happened in UpdateReadableMask (Main Thread)
            // Now we can move the heavy pixel loop to a background thread if needed, 
            // but for simplicity and since we are already async, we can just yield or run on thread pool.
            
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

            // Use GetPixels to avoid multiple GetPixel calls
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
            if (_readableMask == null || _readableMask.width != maskRT.width || _readableMask.height != maskRT.height)
            {
                if (_readableMask != null) UnityEngine.Object.Destroy(_readableMask);
                _readableMask = new Texture2D(maskRT.width, maskRT.height, TextureFormat.R8, false);
            }

            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = maskRT;
            _readableMask.ReadPixels(new Rect(0, 0, maskRT.width, maskRT.height), 0, 0);
            _readableMask.Apply(false); // No mipmaps
            RenderTexture.active = previous;
        }
    }
}
