using System.Runtime.CompilerServices;

namespace SWF.Core.Job
{
    public static class TaskExtensions
    {
        public static ConfiguredTaskAwaitable False(this Task task)
        {
            ArgumentNullException.ThrowIfNull(task, nameof(task));

            return task.ConfigureAwait(false);
        }

        public static ConfiguredTaskAwaitable<T> False<T>(this Task<T> task)
        {
            ArgumentNullException.ThrowIfNull(task, nameof(task));

            return task.ConfigureAwait(false);
        }
    }

    public static class ValueTaskExtensions
    {
        public static ConfiguredValueTaskAwaitable False(this ValueTask valueTask)
        {
            return valueTask.ConfigureAwait(false);
        }

        public static ConfiguredValueTaskAwaitable<T> False<T>(this ValueTask<T> valueTask)
        {
            return valueTask.ConfigureAwait(false);
        }
    }

    public static class IAsyncEnumerableExtensions
    {
        public static ConfiguredCancelableAsyncEnumerable<T> False<T>(this IAsyncEnumerable<T> task)
        {
            ArgumentNullException.ThrowIfNull(task, nameof(task));

            return task.ConfigureAwait(false);
        }
    }
}
