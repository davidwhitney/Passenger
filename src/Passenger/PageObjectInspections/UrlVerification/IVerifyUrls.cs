using Passenger.PageObjectInspections.UrlDiscovery;

namespace Passenger.PageObjectInspections.UrlVerification
{
    public interface IVerifyUrls
    {
        bool UrlMatches(string url, DiscoveredUrl expectation);
    }
}