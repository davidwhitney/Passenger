using System;
using System.Collections.Generic;
using Ariane.Attributes;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;

namespace Ariane.Test.Unit
{
    [TestFixture]
    public class PageObjectFactoryTests
    {
        private ArianeConfiguration _testConfig;

        [SetUp]
        public void Setup()
        {
            _testConfig = new ArianeConfiguration
            {
                Driver = () => new PhantomJSDriver(),
                WebRoot = () => "http://www.davidwhitney.co.uk"
            };
        }

        [Test]
        public void CanGenerateAProxy()
        {
            using (var ctx = _testConfig.StartTestAt<Homepage>())
            {
                ctx.Page<Homepage>().MiddleWrapper.Click();
                ctx.Page<Homepage>().middleWrapper.Click();
                ctx.Page<Homepage>().Blog.Click();

                ctx.VerifyRedirectionTo<Blog>();

                foreach (var post in ctx.Page<Blog>().Posts)
                {
                    Console.WriteLine(post.Text);
                }
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
    }

    [Uri("/Blog")]
    public class Blog
    {
        [CssSelector(".blog-post-title-on-index")]
        public virtual IEnumerable<IWebElement> Posts { get; set; }
    }

}
