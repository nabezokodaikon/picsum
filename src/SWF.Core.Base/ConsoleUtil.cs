namespace SWF.Core.Base
{
    public static class ConsoleUtil
    {
        public static void Write(string message)
        {
#if DEBUG
            ArgumentNullException.ThrowIfNull(message, nameof(message));
            Console.WriteLine($"[{Thread.CurrentThread.Name}] {message}");
#elif DEVELOP
            ArgumentNullException.ThrowIfNull(message, nameof(message));
            Console.WriteLine($"[{Thread.CurrentThread.Name}] {message}");
#endif
        }
    }
}
