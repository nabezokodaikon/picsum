namespace SWF.Core.Job
{
    public interface ISender
    {
        bool IsHandleCreated { get; }
        bool IsLoaded { get; }
        bool IsDisposed { get; }
    }
}
