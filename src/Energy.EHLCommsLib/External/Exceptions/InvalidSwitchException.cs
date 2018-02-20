using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
