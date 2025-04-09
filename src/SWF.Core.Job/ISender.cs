namespace SWF.Core.Job
{
    public interface ISender
    {
        bool IsHandleCreated { get; }
        bool IsDisposed { get; }
    }
}
