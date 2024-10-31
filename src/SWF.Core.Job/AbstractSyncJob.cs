using NLog;

namespace SWF.Core.Job
{
    public abstract class AbstractSyncJob
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public AbstractSyncJob()
        {

        }
    }
}
