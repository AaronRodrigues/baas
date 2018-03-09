using System;

namespace Energy.EHLCommsLib.Exceptions
{
    public class InvalidSwitchException : Exception
    {
        public InvalidSwitchException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}