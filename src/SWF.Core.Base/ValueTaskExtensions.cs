using System.Runtime.CompilerServices;

namespace SWF.Core.Base
{
    public static class ValueTaskExtensions
    {
        public static ConfiguredValueTaskAwaitable WithConfig(this ValueTask valueTask)
        {
            return valueTask.ConfigureAwait(false);
        }

        public static ConfiguredValueTaskAwaitable<T> WithConfig<T>(this ValueTask<T> valueTask)
        {
            return valueTask.ConfigureAwait(false);
        }
    }
}
