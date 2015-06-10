using System;
using System.Collections.Generic;
using Passenger.Attributes;
using Passenger.Drivers.RemoteWebDriver;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Remote;

namespace Passenger.Test.Unit
{
    [Ignore("Demo test")]
    [TestFixture]
    public class ExampleUsage
    {
        private PassengerConfiguration _testConfig;
        private PageObject<Homepage> _pageObject;
        
        [SetUp]
        public void Setup()
        {
            _testConfig = new PassengerConfiguration
            {
                WebRoot = "http://www.davidwhitney.co.uk"
            }.WithDriver(new PhantomJSDriver());
        }

        [TearDown]
        public void Teardown()
        {
            _pageObject.Dispose();
        }

        [Test]
        public void ExampleUsageTest()
        {
            _pageObject = _testConfig.StartTestAt<Homepage>();

            _pageObject.Page<Homepage>().MiddleWrapper.Click();
            _pageObject.Page<Homepage>().FillInForm("abc");

            _pageObject.Page<Homepage>().Blog.Click();
            _pageObject.VerifyRedirectionTo<Blog>();
            
            foreach (var post in _pageObject.Page<Blog>().Posts)
            {
                Console.WriteLine(post.Text);
            }
        }
    }

    [Uri("/")]
    public class Homepage
    {
        // Magically wired up.
        protected virtual RemoteWebDriver YayWebDriver { get; set; }

        [Id("middleWrapper")]
        public virtual IWebElement MiddleWrapper { get; set; }

        [Id]
        public virtual IWebElement middleWrapper { get; set; }

        [LinkText]
        public virtual IWebElement Blog { get; set; }

        public void FillInForm(string user)
        {
            var ele = YayWebDriver.FindElementById("middleWrapper"); // Or some other driver operation
        }
    }

    [Uri("/Blog")]
    public class Blog
    {
        [CssSelector(".blog-post-title-on-index")]
        public virtual IEnumerable<IWebElement> Posts { get; set; }
    }

}
