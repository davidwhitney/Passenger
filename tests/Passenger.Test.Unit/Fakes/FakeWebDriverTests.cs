using System;
using NUnit.Framework;

namespace Passenger.Test.Unit.Fakes
{
    [TestFixture]
    public class FakeWebDriverTests
    {
        private FakeWebDriver _fake;

        [SetUp]
        public void Setup()
        {
            _fake = new FakeWebDriver();
        }

        [Test]
        public void Url_ReturnsUrlBacking()
        {
            var guid = "/" + Guid.NewGuid();
            _fake.UrlBacking = guid;

            Assert.That(_fake.Url, Is.EqualTo(guid));
        }

        [Test]
        public void NavigateTo_Called_UpdatesUri()
        {
            var destination = new Uri("http://www.tempuri.org");

            _fake.NavigateTo(destination);

            Assert.That(_fake.Url, Is.EqualTo(destination.ToString()));
        }

        [Test]
        public void Dispose_DoesNotThrow()
        {
            _fake.Dispose();
        }
    }
}