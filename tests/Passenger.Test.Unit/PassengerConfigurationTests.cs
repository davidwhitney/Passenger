using NUnit.Framework;
using Passenger.PageObjectInspections.UrlVerification;

namespace Passenger.Test.Unit
{
    [TestFixture]
    public class PassengerConfigurationTests
    {
        [Test]
        public void Ctor_DefaultConfiguration_HasUrlVerificationStrategyOfStringContainingStrategyy()
        {
            var config = new PassengerConfiguration();

            Assert.That(config.UrlVerificationStrategy, Is.TypeOf<StringContainingStrategy>());
        }
    }
}
