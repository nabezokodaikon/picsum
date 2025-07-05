namespace SWF.Core.ConsoleAccessor
{
    public class AppException
        : Exception
    {
        public AppException(string message, Exception exception)
            : base(message, exception)
        {

        }

        public AppException(string message)
            : base(message)
        {

        }
    }
}
