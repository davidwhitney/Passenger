using System;
using NUnit.Framework;
using Passenger.Attributes;
using Passenger.PageObjectInspections.UrlDiscovery;
using Passenger.PageObjectInspections.UrlVerification;

namespace Passenger.Test.Unit.PageObjectInspections.UrlVerification
{
    [TestFixture]
    public class RegexStrategyTests
    {
        private RegexStrategy _strat;

        [SetUp]
        public void SetUp()
        {
            _strat = new RegexStrategy();
        }

        [Test]
        public void Supports_NoVerificationPatternProvided_ReturnsFalse()
        {
            var discoveredUrl = new DiscoveredUrl(new Uri("http://tempuri.org"), new UriAttribute("http://tempuri.org", null));

            Assert.That(_strat.Supports(discoveredUrl), Is.False);
        }

        [Test]
        public void Supports_VerificationPatternProvided_ReturnsTrue()
        {
            var discoveredUrl = new DiscoveredUrl(new Uri("http://tempuri.org"), new UriAttribute("http://tempuri.org", "http://tempuri.org/[a-z]+"));

            Assert.That(_strat.Supports(discoveredUrl), Is.True);
        }
    }
}
