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
            if (expectation.Url.IsAbsoluteUri)
            {
                return actualUrl.Contains(expectation.Url.OriginalString);
            }

            return actualUrl.Contains(expectation.Url.OriginalString);
        }
    }
}
