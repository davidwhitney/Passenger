namespace Passenger.PageObjectInspections.UrlVerification
{
    public interface IVerifyUrls
    {
        bool UrlMatches(string url, string expectation);
    }
}