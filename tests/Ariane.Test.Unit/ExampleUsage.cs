using System;
using System.Collections.Generic;
using Ariane.Attributes;
using Ariane.Drivers;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;

namespace Ariane.Test.Unit
{
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
                Driver = () => new WebDriverBindings(new PhantomJSDriver()),
                WebRoot = () => "http://www.davidwhitney.co.uk"
            };
        }

        [TearDown]
        public void Teardown()
        {
            _ctx.Dispose();
        }

        [Test]
        public void CanGenerateAProxy()
        {
            _ctx = _testConfig.StartTestAt<Homepage>();

            _ctx.Page<Homepage>().MiddleWrapper.Click();
            _ctx.Page<Homepage>().middleWrapper.Click();
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
        [Id("middleWrapper")]
        public virtual IWebElement MiddleWrapper { get; set; }

        [Id]
        public virtual IWebElement middleWrapper { get; set; }

        [Text]
        public virtual IWebElement Blog { get; set; }

        public void FillInForm(string user)
        {
        }
    }

    [Uri("/Blog")]
    public class Blog
    {
        [CssSelector(".blog-post-title-on-index")]
        public virtual IEnumerable<IWebElement> Posts { get; set; }
    }

}
