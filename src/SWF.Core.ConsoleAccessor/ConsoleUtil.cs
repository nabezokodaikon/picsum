using NLog;

namespace SWF.Core.ConsoleAccessor
{
    public static class ConsoleUtil
    {
        public static void Write(bool enable, string message)
        {
#if DEBUG
            if (enable)
            {
                ArgumentNullException.ThrowIfNull(message, nameof(message));

                if (ScopeContext.TryGetProperty("task", out var value))
                {
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} | {value} | {message}");
                }
                else
                {
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} | {message}");
                }
            }
#endif
        }
    }
}
