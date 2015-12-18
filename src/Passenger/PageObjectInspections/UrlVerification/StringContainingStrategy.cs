using Passenger.PageObjectInspections.UrlDiscovery;

namespace Passenger.PageObjectInspections.UrlVerification
{
    public class StringContainingStrategy : IVerifyUrls
    {
        public bool Supports(DiscoveredUrl expectation)
        {
            return true;
        }

        public bool UrlMatches(string actualUrl, DiscoveredUrl expectation)
        {
            return actualUrl.Contains(expectation.Url.PathAndQuery);
        }
    }
}
