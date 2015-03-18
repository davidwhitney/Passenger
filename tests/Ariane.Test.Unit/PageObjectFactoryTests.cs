using System;
using Ariane.Attributes;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;

namespace Ariane.Test.Unit
{
    [TestFixture]
    public class PageObjectFactoryTests
    {
        private PageObjectTest _test;

        [SetUp]
        public void Setup()
        {
            _test = new PageObjectTest
            {
                WithDriver = () => new PhantomJSDriver(),
                WebRoot = () => "http://www.davidwhitney.co.uk"
            };
        }

        [Test]
        public void CanGenerateAProxy()
        {
            using (var ctx = _test.StartAt<DavidHomepage>())
            {
                ctx.Page.MiddleWrapper.Click();

                Assert.That(ctx.Page.MiddleWrapper.Text.Contains("coder"));
            }
        }
    }

    [Uri("/")]
    public class DavidHomepage
    {
        [Id("middleWrapper")]
        public virtual IWebElement MiddleWrapper { get; set; }
    }

}
