namespace SWF.Core.Base
{
    public static class ConsoleUtil
    {
        public static void Write(string message)
        {
#if DEBUG
            ArgumentNullException.ThrowIfNull(message, nameof(message));
            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss.ffff")}] [{Thread.CurrentThread.Name}] {message}");
#endif
        }

        public static void Write(bool enable, string message)
        {
#if DEBUG
            if (enable)
            {
                ArgumentNullException.ThrowIfNull(message, nameof(message));
                Write(message);
            }
#endif
        }
    }
}
