namespace SWF.Core.Base
{
    public class SWFException
        : Exception
    {
        public SWFException(string message, Exception exception)
           : base(message, exception)
        {

        }

        public SWFException(string message)
            : base(message)
        {

        }
    }
}
