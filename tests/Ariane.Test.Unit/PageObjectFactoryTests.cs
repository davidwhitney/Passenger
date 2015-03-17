using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;

namespace Ariane.Test.Unit
{
    [TestFixture]
    public class PageObjectFactoryTests
    {
        private PageObjectFactory _factory;

        [SetUp]
        public void Setup()
        {
            _factory = new PageObjectFactory
            {
                WithDriver = () => new PhantomJSDriver()
            };
        }

        [Test]
        public void CanGenerateAProxy()
        {
            var instance = _factory.Load<DavidHomepage>();

            instance.MiddleWrapper.Click();
        }
    }

    [Uri("http://www.davidwhitney.co.uk")]
    public class DavidHomepage
    {
        [Id("middleWrapper")]
        public virtual IWebElement MiddleWrapper { get; set; }
    }

}
