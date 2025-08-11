using System.Runtime.CompilerServices;

namespace SWF.Core.Base
{
    public static class TaskExtensions
    {
        public static ConfiguredTaskAwaitable WithConfig(this Task task)
        {
            return task.ConfigureAwait(false);
        }

        public static ConfiguredTaskAwaitable<T> WithConfig<T>(this Task<T> task)
        {
            return task.ConfigureAwait(false);
        }
    }
}
