using Passenger.PageObjectInspections.UrlDiscovery;
using Passenger.PageObjectInspections.UrlVerification;

namespace Passenger.Test.Unit.Fakes
{
    public class FakeUrlVerifier : IVerifyUrls
    {
        public bool Called { get; set; }
        public string Url { get; set; }
        public DiscoveredUrl Expectation { get; set; }

        public bool Supports(DiscoveredUrl expectation)
        {
            return true;
        }

        public bool UrlMatches(string actualUrl, DiscoveredUrl expectation)
        {
            Called = true;
            Url = actualUrl;
            Expectation = expectation;
            return true;
        }
    }
}