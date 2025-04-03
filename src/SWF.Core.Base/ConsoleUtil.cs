namespace SWF.Core.Base
{
    public static class ConsoleUtil
    {
        public static void Write(bool enable, string message)
        {
#if DEBUG
            if (enable)
            {
                ArgumentNullException.ThrowIfNull(message, nameof(message));
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.ffff}] [{Thread.CurrentThread.Name}] {message}");
            }
#endif
        }
    }
}
