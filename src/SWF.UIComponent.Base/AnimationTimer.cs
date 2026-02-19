using SWF.Core.Base;
using WinApi;

namespace SWF.UIComponent.Base
{
    public class AnimationTimer
        : IDisposable
    {
        private const int WM_TIMER_TICK = WinApiMembers.WM_APP + 1;

        public Action? Tick = null;

        private WinApiMembers.TimerCallback? _callback = null;
        private int _timerId;
        private int _intervalMs;
        private bool _enabled;
        private bool _disposed;
        private int _tickPending = 0;
        private TimerNativeWindow? _nativeWindow = null;

        public bool Enabled => this._enabled;

        public void Dispose()
        {
            if (this._disposed)
            {
                return;
            }

            this._disposed = true;
            this.StopTimer();

            GC.SuppressFinalize(this);
        }

        ~AnimationTimer()
        {
            if (this._timerId != 0)
            {
                _ = WinApiMembers.timeKillEvent(this._timerId);
                _ = WinApiMembers.timeEndPeriod(1);
            }
        }

        public void Start(int intervalMs)
        {
            AppConstants.ThrowIfNotUIThread();

            if (intervalMs <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(intervalMs), "1以上の値を指定してください。");
            }

            this._intervalMs = intervalMs;

            if (this._enabled)
            {
                this.StopTimer();
            }

            this.StartTimer();
        }

        public void Stop()
        {
            AppConstants.ThrowIfNotUIThread();

            if (this._enabled)
            {
                this.StopTimer();
            }
        }

        private void StartTimer()
        {
            ObjectDisposedException.ThrowIf(this._disposed, this);

            this._nativeWindow = new TimerNativeWindow(this);
            this._nativeWindow.CreateHandle(new CreateParams());

            _ = WinApiMembers.timeBeginPeriod(1);
            this._callback = this.OnTimerTick;
            this._timerId = WinApiMembers.timeSetEvent(this._intervalMs, 0, this._callback, IntPtr.Zero, 1);
            if (this._timerId == 0)
            {
                _ = WinApiMembers.timeEndPeriod(1);
                this._nativeWindow.DestroyHandle();
                this._nativeWindow = null;
                throw new InvalidOperationException("Multimedia Timerの開始に失敗しました。");
            }

            this._enabled = true;
        }

        private void StopTimer()
        {
            if (this._timerId != 0)
            {
                _ = WinApiMembers.timeKillEvent(this._timerId);
                _ = WinApiMembers.timeEndPeriod(1);
                this._timerId = 0;
                this._callback = null;
            }

            if (this._nativeWindow != null)
            {
                this._nativeWindow.DestroyHandle();
                this._nativeWindow = null;
            }

            this._enabled = false;
            Interlocked.Exchange(ref this._tickPending, 0); // ← ここだけ変更
        }

        private void OnTimerTick(int id, int msg, IntPtr user, IntPtr dw1, IntPtr dw2)
        {
            if (!this._enabled || this._disposed)
            {
                return;
            }

            if (Interlocked.CompareExchange(ref this._tickPending, 1, 0) == 0)
            {
                if (this._nativeWindow != null)
                {
                    _ = WinApiMembers.PostMessage(this._nativeWindow.Handle, WM_TIMER_TICK, IntPtr.Zero, IntPtr.Zero);
                }
            }
        }

        private void OnWmTimerTick()
        {
            Interlocked.Exchange(ref this._tickPending, 0);

            if (!this._enabled || this._disposed)
            {
                return;
            }

            this.Tick?.Invoke();
        }

        private sealed class TimerNativeWindow
            : NativeWindow
        {
            private readonly AnimationTimer _owner;

            public TimerNativeWindow(AnimationTimer owner)
            {
                this._owner = owner;
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_TIMER_TICK)
                {
                    this._owner.OnWmTimerTick();
                    return;
                }
                base.WndProc(ref m);
            }
        }
    }
}