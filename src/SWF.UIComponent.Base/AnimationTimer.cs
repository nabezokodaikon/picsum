using SWF.Core.Base;

namespace SWF.UIComponent.Base
{
    public sealed class AnimationTimer
        : IDisposable
    {
        private bool _disposed = false;

        private System.Threading.Timer? _animationTimer = null;

        public bool Enabled
        {
            get
            {
                return this._animationTimer != null;
            }
        }

        public AnimationTimer()
        {

        }

        private void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                this._animationTimer?.Dispose();
                this._animationTimer = null;
            }

            this._disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Start(Control control, Action animationTick)
        {
            ArgumentNullException.ThrowIfNull(control, nameof(control));
            ArgumentNullException.ThrowIfNull(animationTick, nameof(animationTick));

            if (this._disposed)
            {
                return;
            }

            this._animationTimer?.Dispose();
            this._animationTimer = new System.Threading.Timer(
                _ =>
                {
                    if (!control.IsHandleCreated && !control.IsDisposed)
                    {
                        return;
                    }

                    control.BeginInvoke(() =>
                    {
                        if (!control.IsHandleCreated && !control.IsDisposed)
                        {
                            return;
                        }

                        animationTick();
                    });
                },
                null,
                0,
                DisplayUitl.GetAnimationInterval(control));
        }

        public void Stop()
        {
            this._animationTimer?.Dispose();
            this._animationTimer = null;
        }
    }
}
