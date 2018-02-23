using System;

namespace Energy.EHLCommsLib.Interfaces
{
    public interface IApplicationContext
    {
        Guid? JourneyId { get; }
    }
}