using Passenger.Attributes;

namespace Passenger.Test.Unit.Fakes
{
    [Uri("http://tempuri.org/fake2", VerificationPattern = "(.)*")]
    public class FakePageWithRegex : PageObject
    {
    }
}