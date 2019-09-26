﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Passenger.Attributes;
using Passenger.Drivers.RemoteWebDriver;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace Passenger.Test.Unit
{
    //[Ignore("Demo test")]
    [TestFixture]
    public class ExampleUsage
    {
        private PassengerConfiguration _testConfig;
        private PageObject<Homepage> _pageObject;
        
        [SetUp]
        public void Setup()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--headless");
            chromeOptions.AddArgument("--no-sandbox");
            chromeOptions.AddArgument("window-size=1400,2100");
            var driver = new ChromeDriver(Environment.CurrentDirectory, chromeOptions);

            _testConfig = new PassengerConfiguration
            {
                WebRoot = "http://www.davidwhitney.co.uk"
            }.WithDriver(driver);
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

        [Id("heroWrapper")]
        public virtual IWebElement MiddleWrapper { get; set; }

        [Id]
        public virtual IWebElement heroWrapper { get; set; }

        [LinkText]
        public virtual IWebElement Blog { get; set; }

        public void FillInForm(string user)
        {
            var ele = YayWebDriver.FindElementById("heroWrapper"); // Or some other driver operation
        }
    }

    [Uri("/Blog")]
    public class Blog
    {
        [CssSelector(".blog-post-title-on-index")]
        public virtual IEnumerable<IWebElement> Posts { get; set; }
    }

}
