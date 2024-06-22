namespace SWF.Core.Base
{
    public class SWFException
        : Exception
    {
        protected SWFException(string message, Exception exception)
           : base(message, exception)
        {

        }

        protected SWFException(string message)
            : base(message)
        {

        }
    }
}
