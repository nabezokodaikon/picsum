using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using System.IO.Pipes;
using System.Runtime.Versioning;

namespace PicSum.Job.Jobs
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed class PipeServerJob
        : AbstractTwoWayJob<ValueResult<string>>
    {
        protected override async Task Execute()
        {
            while (true)
            {
                this.CheckCancel();

                try
                {
                    using (var pipeServer = new NamedPipeServerStream(
                        AppConstants.PIPE_NAME, PipeDirection.In,
                        1,
                        PipeTransmissionMode.Byte,
                        PipeOptions.Asynchronous))
                    using (var cts = new CancellationTokenSource(1000))
                    {
                        await pipeServer.WaitForConnectionAsync(cts.Token);

                        this.CheckCancel();

                        using (var reader = new StreamReader(pipeServer))
                        {
                            var receivedArgs = await reader.ReadLineAsync();
                            if (!string.IsNullOrEmpty(receivedArgs)
                                && FileUtil.CanAccess(receivedArgs)
                                && ImageUtil.IsImageFile(receivedArgs))
                            {
                                this.Callback(new ValueResult<string>
                                {
                                    Value = receivedArgs,
                                });
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    continue;
                }

                await Task.Delay(250);
            }
        }
    }
}
