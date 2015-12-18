using Passenger.PageObjectInspections.UrlDiscovery;

namespace Passenger.PageObjectInspections.UrlVerification
{
    public class RegexStrategy : IVerifyUrls
    {
        public bool Supports(DiscoveredUrl expectation)
        {
            return !string.IsNullOrWhiteSpace(expectation.SourceAttribute.VerificationPattern);
        }

        public bool UrlMatches(string actualUrl, DiscoveredUrl expectation)
        {
            return expectation.SourceAttribute.VerificationRegex.IsMatch(actualUrl);
        }
    }
}