using System;

namespace SWF.Common
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
