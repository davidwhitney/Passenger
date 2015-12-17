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

        public bool UrlMatches(string url, DiscoveredUrl expectation)
        {
            Called = true;
            Url = url;
            Expectation = expectation;
            return true;
        }
    }
}