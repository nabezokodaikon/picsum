using SWF.Core.Base;
using SWF.Core.FileAccessor;
using SWF.Core.ImageAccessor;
using SWF.Core.Job;
using System.IO.Pipes;

namespace PicSum.Job.Jobs
{

    public sealed class PipeServerJob
        : AbstractTwoWayJob<ValueResult<string>>
    {
        protected override async ValueTask Execute()
        {
            while (true)
            {
                this.ThrowIfJobCancellationRequested();

                using (var pipeServer = new NamedPipeServerStream(
                    AppConstants.PIPE_NAME,
                    PipeDirection.In,
                    1,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous))
                {
                    await pipeServer.WaitForConnectionAsync(this.CancellationToken).False();

                    this.ThrowIfJobCancellationRequested();

                    using (var reader = new StreamReader(pipeServer))
                    {
                        var receivedArgs = await reader.ReadLineAsync().False();
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
        }
    }
}
