using NLog;
using SWF.Core.ConsoleAccessor;

namespace SWF.Core.Job
{
    public abstract class AbstractSyncJob
    {
        protected static readonly Logger Logger = Log.Logger;

        public AbstractSyncJob()
        {

        }
    }
}
