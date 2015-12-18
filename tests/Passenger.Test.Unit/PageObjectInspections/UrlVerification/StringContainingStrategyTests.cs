using System;
using NUnit.Framework;
using Passenger.Attributes;
using Passenger.PageObjectInspections.UrlDiscovery;
using Passenger.PageObjectInspections.UrlVerification;

namespace Passenger.Test.Unit.PageObjectInspections.UrlVerification
{
    [TestFixture]
    public class StringContainingStrategyTests
    {
        private StringContainingStrategy _strat;

        [SetUp]
        public void SetUp()
        {
            _strat = new StringContainingStrategy();
        }

        [Test]
        public void Supports_NoVerificationPatternProvided_ReturnsTrue()
        {
            var discoveredUrl = new DiscoveredUrl(new Uri("http://tempuri.org"), new UriAttribute("http://tempuri.org", null));

            Assert.That(_strat.Supports(discoveredUrl), Is.True);
        }

        [Test]
        public void Supports_VerificationPatternProvided_ReturnsTrue()
        {
            var discoveredUrl = new DiscoveredUrl(new Uri("http://tempuri.org"), new UriAttribute("http://tempuri.org", "http://tempuri.org/[a-z]+"));

            Assert.That(_strat.Supports(discoveredUrl), Is.True);
        }

        [Test]
        public void UrlMatches_AbsoluteUrlStringContainsDiscoveredUrl_ReturnsTrue()
        {
            var discoveredUrl = new DiscoveredUrl(new Uri("http://tempuri.org"), new UriAttribute("http://tempuri.org"));

            Assert.That(_strat.UrlMatches("http://tempuri.org/some/path", discoveredUrl), Is.True);
        }

        [Test]
        public void UrlMatches_AbsoluteUrlStringDoesNotContainsDiscoveredUrl_ReturnsFalse()
        {
            var discoveredUrl = new DiscoveredUrl(new Uri("http://tempuri.org"), new UriAttribute("http://tempuri.org"));

            Assert.That(_strat.UrlMatches("http://nopenopenope.org/some/path", discoveredUrl), Is.False);
        }

        [Test]
        public void UrlMatches_RelativeUrlStringContainsDiscoveredUrl_ReturnsTrue()
        {
            var discoveredUrl = new DiscoveredUrl(new Uri("/test", UriKind.Relative), new UriAttribute("/test"));

            Assert.That(_strat.UrlMatches("http://tempuri.org/test", discoveredUrl), Is.True);
        }

        [Test]
        public void UrlMatches_RelativeUrlStringDoesNotContainsDiscoveredUrl_ReturnsFalse()
        {
            var discoveredUrl = new DiscoveredUrl(new Uri("/test", UriKind.Relative), new UriAttribute("/test"));

            Assert.That(_strat.UrlMatches("http://nopenopenope.org/not-here", discoveredUrl), Is.False);
        }
    }
}
