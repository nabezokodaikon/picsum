namespace SWF.Core.Job
{
    public interface ISender
    {
        bool InvokeRequired { get; }
        bool IsHandleCreated { get; }
        bool IsDisposed { get; }
        IAsyncResult BeginInvoke(Delegate method);
        void Invoke(Action method);
    }
}
