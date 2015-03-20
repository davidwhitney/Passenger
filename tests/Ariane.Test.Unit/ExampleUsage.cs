using System;
using System.Collections.Generic;
using Ariane.Attributes;
using Ariane.Drivers.RemoteWebDriver;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Remote;

namespace Ariane.Test.Unit
{
    [Ignore("Demo test")]
    [TestFixture]
    public class ExampleUsage
    {
        private ArianeConfiguration _testConfig;
        private PageObjectTestContext<Homepage> _ctx;
        
        [SetUp]
        public void Setup()
        {
            _testConfig = new ArianeConfiguration
            {
                WebRoot = "http://www.davidwhitney.co.uk"
            }.WithDriver(new PhantomJSDriver());
        }

        [TearDown]
        public void Teardown()
        {
            _ctx.Dispose();
        }

        [Test]
        public void ExampleUsageTest()
        {
            _ctx = _testConfig.StartTestAt<Homepage>();

            _ctx.Page<Homepage>().MiddleWrapper.Click();
            _ctx.Page<Homepage>().FillInForm("abc");

            _ctx.Page<Homepage>().Blog.Click();
            _ctx.VerifyRedirectionTo<Blog>();
            
            foreach (var post in _ctx.Page<Blog>().Posts)
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

        [Text]
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
