using SWF.Core.Base;

namespace SWF.UIComponent.Base
{
    public sealed class AnimationTimer
        : IDisposable
    {
        private volatile bool _disposed = false;  // volatile 追加
        private System.Threading.Timer? _animationTimer = null;
        private int _callbackId = 0; // ← コールバック世代管理

        public bool Enabled
        {
            get
            {
                return this._animationTimer != null && !this._disposed;
            }
        }

        public AnimationTimer()
        {
            AppConstants.ThrowIfNotUIThread();
        }

        private void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                // UIスレッドチェックは public Dispose() 側のみで行う
                this._animationTimer?.Dispose();
                this._animationTimer = null;
            }

            this._disposed = true;
        }

        public void Dispose()
        {
            AppConstants.ThrowIfNotUIThread();

            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Start(Control control, Action animationTick)
        {
            ArgumentNullException.ThrowIfNull(control, nameof(control));
            ArgumentNullException.ThrowIfNull(animationTick, nameof(animationTick));

            AppConstants.ThrowIfNotUIThread();

            if (this._disposed)
            {
                return;
            }

            var uiContext = SynchronizationContext.Current
                ?? throw new InvalidOperationException("UIコンテキストが取得できませんでした。");

            this._animationTimer?.Dispose();
            this._animationTimer = null;

            // ← 世代IDを更新し、古いコールバックを無視する
            var capturedId = Interlocked.Increment(ref this._callbackId);

            this._animationTimer = new System.Threading.Timer(
                _ =>
                {
                    // 世代が変わっていたら（Stop/Start/Dispose済み）無視
                    if (capturedId != this._callbackId)
                    {
                        return;
                    }

                    uiContext.Post(_ =>
                    {
                        if (capturedId != this._callbackId)
                        {
                            return;
                        }

                        if (!control.IsHandleCreated || control.IsDisposed)
                        {
                            return;
                        }

                        animationTick();
                    }, null);
                },
                null,
                0,
                DisplayUtil.GetAnimationInterval(control));
        }

        public void Stop()
        {
            AppConstants.ThrowIfNotUIThread();

            Interlocked.Increment(ref this._callbackId); // 古いコールバックを無効化

            this._animationTimer?.Dispose();
            this._animationTimer = null;
        }
    }
}