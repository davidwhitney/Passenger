using System;
using NUnit.Framework;
using Passenger.Attributes;
using Passenger.PageObjectInspections.UrlDiscovery;
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

            Assert.That(config.UrlVerificationStrategies[0], Is.TypeOf<RegexStrategy>());
            Assert.That(config.UrlVerificationStrategies[1], Is.TypeOf<StringContainingStrategy>());
        }

        [Test]
        public void UrlVerificationStrategiesStrategyFor_GivenPlainTextUrlAttribute_ReturnsStringContainingStrategy()
        {
            var discoveredUrl = new DiscoveredUrl(new Uri("http://tempuri.org"), new UriAttribute("http://tempuri.org", null));

            var strat = new PassengerConfiguration().UrlVerificationStrategies.StrategyFor(discoveredUrl);

            Assert.That(strat, Is.TypeOf<StringContainingStrategy>());
        }

        [Test]
        public void UrlVerificationStrategiesStrategyFor_GivenUrlAttributeWithRegexPattern_ReturnsRegexStrategy()
        {
            var discoveredUrl = new DiscoveredUrl(new Uri("http://tempuri.org"), new UriAttribute("http://tempuri.org", "http://tempuri.org/[a-z]+"));

            var strat = new PassengerConfiguration().UrlVerificationStrategies.StrategyFor(discoveredUrl);

            Assert.That(strat, Is.TypeOf<RegexStrategy>());
        }
    }
}
