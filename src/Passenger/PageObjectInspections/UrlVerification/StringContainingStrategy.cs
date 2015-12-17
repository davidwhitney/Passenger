namespace Passenger.PageObjectInspections.UrlVerification
{
    public class StringContainingStrategy : IVerifyUrls
    {
        public bool UrlMatches(string url, string expectation)
        {
            return url.Contains(expectation);
        }
    }
}
