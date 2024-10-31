using NLog;

namespace SWF.Core.Job
{
    public abstract class AbstractSyncLogic
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public AbstractSyncLogic()
        {

        }
    }
}
