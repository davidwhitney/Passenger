using System.Collections.Generic;
using NUnit.Framework;
using Passenger.PageObjectInspections.UrlVerification;
using Passenger.Test.Unit.Fakes;

namespace Passenger.Test.Unit
{
    [TestFixture]
    public class PageObjectTests
    {
        private PassengerConfiguration _config;
        private PageObject<FakePage> _po;

        [SetUp]
        public void SetUp()
        {
            _config = new PassengerConfiguration { Driver = new FakeWebDriver() };
            _po = new PageObject<FakePage>(_config);
        }

        [Test]
        public void VerifyRedirection_DelegatesUrlVerificationToConfiguredStartegy()
        {
            _config.UrlVerificationStrategies = new UrlVerificationStrategyCollection {new FakeUrlVerifier()};
            
            _po.VerifyRedirectionTo<FakePage>();

            Assert.That(((FakeUrlVerifier)_config.UrlVerificationStrategies[0]).Called, Is.True);
        }
    }
}
