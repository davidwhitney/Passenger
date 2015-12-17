using System;

namespace Passenger.PageObjectInspections.UrlDiscovery
{
    public interface IDiscoverUrls
    {
        DiscoveredUrl UrlFor(object potentialPageObject, PassengerConfiguration configuration);
    }
}
