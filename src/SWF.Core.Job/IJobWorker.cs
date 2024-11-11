namespace SWF.Core.Job
{
    public interface ITwoWayJob<TJob, TJobParameter, TJobResult>
        : IDisposable
        where TJob : AbstractTwoWayJob<TJobParameter, TJobResult>, new()
        where TJobParameter : IJobParameter
        where TJobResult : IJobResult
    {
        public AbstractBackgroundProcess<TJob, TJobParameter, TJobResult> Reset();
        public AbstractBackgroundProcess<TJob, TJobParameter, TJobResult> Callback(Action<TJobResult> action);
        public AbstractBackgroundProcess<TJob, TJobParameter, TJobResult> Cancel(Action action);
        public AbstractBackgroundProcess<TJob, TJobParameter, TJobResult> Catch(Action<JobException> action);
        public AbstractBackgroundProcess<TJob, TJobParameter, TJobResult> Complete(Action action);
        public void StartJob(ISender sender, TJobParameter parameter);
        public void StartJob(ISender sender);
        public AbstractBackgroundProcess<TJob, TJobParameter, TJobResult> BeginCancel();
        public AbstractBackgroundProcess<TJob, TJobParameter, TJobResult> WaitJobComplete();
    }

    public interface ITwoWayJob<TJob, TJobResult>
        : ITwoWayJob<TJob, EmptyParameter, TJobResult>
        where TJob : AbstractTwoWayJob<TJobResult>, new()
        where TJobResult : IJobResult
    {

    }

    public interface IOneWayJob<TJob>
        : ITwoWayJob<TJob, EmptyParameter, EmptyResult>
        where TJob : AbstractOneWayJob, new()
    {

    }

    public interface IOneWayJob<TJob, TJobParameter>
        : ITwoWayJob<TJob, TJobParameter, EmptyResult>
        where TJob : AbstractOneWayJob<TJobParameter>, new()
        where TJobParameter : IJobParameter
    {

    }
}
