using System;
using System.Collections.Generic;
using Ariane.Drivers;
using NUnit.Framework;

namespace Ariane.Test.Unit.ModelInterception
{
    [TestFixture]
    public class TotallyFakeWebDriverTests
    {
        private TotallyFakeWebDriver _fake;

        [SetUp]
        public void Setup()
        {
            _fake = new TotallyFakeWebDriver();
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

    public class TotallyFakeWebDriver : IDriverBindings
    {
        public TotallyFakeWebDriver()
        {
            Substitutes = new List<DriverBindings.TypeSubstitution>();
            NavigationHandlers = new List<DriverBindings.IHandle>();
        }

        public string UrlBacking { get; set; }
        public string Url
        {
            get { return UrlBacking; }
        }

        public IList<DriverBindings.IHandle> NavigationHandlers { get; private set; }
        public IList<DriverBindings.TypeSubstitution> Substitutes { get; private set; }

        public void NavigateTo(Uri url)
        {
            UrlBacking = url.ToString();
        }

        public void Dispose()
        {
        }
    }
}