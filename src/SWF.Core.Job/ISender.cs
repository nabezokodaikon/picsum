namespace SWF.Core.Job
{
    public interface ISender
    {
        bool IsLoaded { get; }
        bool IsDisposed { get; }
    }
}
