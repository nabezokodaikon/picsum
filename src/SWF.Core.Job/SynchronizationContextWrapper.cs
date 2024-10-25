using SWF.Core.Base;
using System.Diagnostics;

namespace SWF.Core.Job
{
    internal sealed class SynchronizationContextWrapper
    {
        private readonly SynchronizationContext context;

        public SynchronizationContextWrapper()
        {
            if (SynchronizationContext.Current == null)
            {
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            }

            if (SynchronizationContext.Current != null)
            {
                this.context = SynchronizationContext.Current;
            }
            else
            {
                throw new NullReferenceException("コンテキストがNullです。");
            }
        }

        public void Post(SendOrPostCallback d, object? state)
        {
            ArgumentNullException.ThrowIfNull(d, nameof(d));

            var sw = Stopwatch.StartNew();

            this.context.Post(_ =>
            {
                d(_);

                sw.Stop();
                ConsoleUtil.Write($"SynchronizationContextWrapper.Post: {sw.ElapsedMilliseconds} ms");
            }, state);
        }
    }
}
