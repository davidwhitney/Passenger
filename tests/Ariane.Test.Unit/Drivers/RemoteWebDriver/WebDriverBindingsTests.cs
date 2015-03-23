using System;
using System.Collections.Generic;
using System.Linq;
using Ariane.Attributes;
using Ariane.Drivers.RemoteWebDriver;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Ariane.Test.Unit.Drivers.RemoteWebDriver
{
    public class WebDriverBindingsTests
    {
        private WebDriverBindings _bindings;
        private TotallyFakeRemoteWebDriver _fakeDriver;

        [SetUp]
        public void SetUp()
        {
            _fakeDriver = new TotallyFakeRemoteWebDriver(new DesiredCapabilities("abc", "1", new Platform(PlatformType.Any)));
            _bindings = new WebDriverBindings(_fakeDriver);
        }

        [Test]
        public void NavigateTo_InvokesDriversNavigation()
        {
            _bindings.NavigateTo(new Uri("http://www.tempuri.org/"));

            Assert.That(_fakeDriver.LastExecutedCommand, Is.EqualTo("get"));
            Assert.That(_fakeDriver.LastProvidedParameters.First().Key, Is.EqualTo("url"));
            Assert.That(_fakeDriver.LastProvidedParameters.First().Value, Is.EqualTo("http://www.tempuri.org/"));
        }

        [Test]
        public void Substitues_ContainsMappingForWebDriver()
        {
            Assert.That(_bindings.Substitutes.Count, Is.EqualTo(2));
            Assert.That(_bindings.Substitutes.Any(x=>x.Type == typeof(IWebDriver)));
            Assert.That(_bindings.Substitutes.Any(x=>x.Type == typeof(OpenQA.Selenium.Remote.RemoteWebDriver)));
        }

        [Test]
        public void NavigationHandlers_NotNull()
        {
            Assert.That(_bindings.NavigationHandlers, Is.Not.Null);
        }

        [Test]
        public void NavigationHandlers_HandlerPresentForEachNavigationAttribute()
        {
            var navAttributeTypes =
                typeof (NavigationAttributeBase).Assembly.GetTypes()
                    .Where(x => x.BaseType == typeof (NavigationAttributeBase))
                    .ToList();

            foreach (var attr in navAttributeTypes)
            {
                Assert.That(_bindings.NavigationHandlers.Any(x=>x.AttributeType == attr));
            }
        }

        [Test]
        public void NavigationHandlers_HandlerInvoked_BindsToWebDriver()
        {
            foreach (var handler in _bindings.NavigationHandlers)
            {
                var handlerStub = handler.AttributeType.Name.ToLower().Replace("attribute", "");

                try
                {
                    handler.FindAllMatches("a", _bindings);
                }
                catch (Exception)
                {
                    // Lets pretend selenium exists here.
                }

                Assert.That(_fakeDriver.LastExecutedCommand, Is.EqualTo("findElements"));
                Assert.That(_fakeDriver.LastProvidedParameters["using"].ToString().Replace(" ", ""), Is.EqualTo(handlerStub));
                Assert.That(_fakeDriver.LastProvidedParameters["value"], Is.EqualTo("a"));
            }
        }

        [Test]
        public void Subsitutions_ProvideSubstituesForWebDriver()
        {
            foreach (var typeSubstitution in _bindings.Substitutes)
            {
                var instance = typeSubstitution.GetInstance();

                Assert.That(instance, Is.TypeOf<TotallyFakeRemoteWebDriver>());
            }
        }

        [Test]
        public void Dispose_DisposesOfWebDriver()
        {
            _bindings.Dispose();

            Assert.That(((TotallyFakeRemoteWebDriver)_bindings.Driver).Disposed, Is.EqualTo(true));
        }
    }

    public class TotallyFakeRemoteWebDriver : OpenQA.Selenium.Remote.RemoteWebDriver 
    {
        public TotallyFakeRemoteWebDriver(ICapabilities desiredCapabilities)
            : base(desiredCapabilities)
        {
        }

        protected override Response Execute(string driverCommandToExecute, Dictionary<string, object> parameters)
        {
            LastExecutedCommand = driverCommandToExecute;
            LastProvidedParameters = parameters;
            return new Response();
        }

        protected override void Dispose(bool disposing)
        {
            Disposed = true;
        }

        public bool Disposed { get; set; }
        public bool Closed { get; set; }
        public Dictionary<string, object> LastProvidedParameters { get; set; }
        public string LastExecutedCommand { get; set; }
    }
}
