using System;

namespace Energy.EHLCommsLib.External.Exceptions
{
    public class InvalidSwitchException : Exception
    {
        public InvalidSwitchException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}