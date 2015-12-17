using Passenger.PageObjectInspections.UrlDiscovery;

namespace Passenger.PageObjectInspections.UrlVerification
{
    public interface IVerifyUrls
    {
        bool Supports(DiscoveredUrl expectation);
        bool UrlMatches(string url, DiscoveredUrl expectation);
    }
}