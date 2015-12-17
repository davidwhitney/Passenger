using Passenger.PageObjectInspections.UrlVerification;

namespace Passenger.Test.Unit.Fakes
{
    public class FakeUrlVerifier : IVerifyUrls
    {
        public bool Called { get; set; }
        public string Url { get; set; }
        public string Expectation { get; set; }

        public bool UrlMatches(string url, string expectation)
        {
            Called = true;
            Url = url;
            Expectation = expectation;
            return true;
        }
    }
}