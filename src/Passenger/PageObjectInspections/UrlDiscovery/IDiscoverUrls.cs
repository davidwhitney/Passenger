using System;

namespace Passenger.PageObjectInspections.UrlDiscovery
{
    public interface IDiscoverUrls
    {
        Uri UrlFor(object potentialPageObject, PassengerConfiguration configuration);
    }
}
