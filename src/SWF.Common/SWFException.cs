using System;

namespace SWF.Common
{
    public class SWFException
        : Exception
    {
        internal SWFException(string message, Exception exception)
           : base(message, exception)
        {

        }

        internal SWFException(string message)
            : base(message)
        {

        }
    }
}
