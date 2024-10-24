using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.Job;
using System.IO.Pipes;

namespace PicSum.Job.Jobs
{
    public sealed class PipeServerJob
        : AbstractTwoWayJob<ValueResult<string>>
    {
        protected override void Execute()
        {
            var thread = this.DoWork();

            while (true)
            {
                try
                {
                    this.CheckCancel();
                }
                catch (JobCancelException)
                {
                    thread.Wait();
                    throw;
                }

                Thread.Sleep(100);
            }
        }

        private async Task DoWork()
        {
            NamedPipeServerStream? pipeServer = null;

            try
            {
                while (true)
                {
                    this.CheckCancel();

                    using (var cts = new CancellationTokenSource())
                    {
                        if (pipeServer == null)
                        {
                            pipeServer = new NamedPipeServerStream(
                                ApplicationConstants.PIPE_NAME, PipeDirection.In);
                        }

                        cts.CancelAfter(100);

                        try
                        {
                            await pipeServer.WaitForConnectionAsync(cts.Token);

                            using (var reader = new StreamReader(pipeServer))
                            {
                                var buffer = new char[1024];
                                try
                                {
                                    var bytesRead = reader.Read(buffer, 0, buffer.Length);
                                    var receivedArgs = (new string(buffer, 0, bytesRead)).Trim();
                                    if (FileUtil.CanAccess(receivedArgs)
                                        && FileUtil.IsImageFile(receivedArgs))
                                    {
                                        this.Callback(new ValueResult<string>
                                        {
                                            Value = receivedArgs,
                                        });
                                    }
                                }
                                catch (IOException ex)
                                {
                                    this.WriteErrorLog(new JobException(this.ID, ex));
                                }
                                finally
                                {
                                    pipeServer.Dispose();
                                    pipeServer = null;
                                }
                            }
                        }
                        catch (OperationCanceledException) { }
                    }
                }
            }
            catch (JobCancelException)
            {
                return;
            }
            finally
            {
                if (pipeServer != null)
                {
                    pipeServer.Dispose();
                    pipeServer = null;
                }
            }
        }
    }
}
