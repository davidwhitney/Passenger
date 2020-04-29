using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using Passenger.Attributes;
using Passenger.Drivers.RemoteWebDriver;

namespace Passenger.Examples.Amazon
{
    [TestFixture]
    public class AmazonExample
    {
        private ChromeDriver driver;

        [SetUp]
        public void Setup()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--headless");
            chromeOptions.AddArgument("--no-sandbox");
            chromeOptions.AddArgument("window-size=1400,2100");
            driver = new ChromeDriver(Environment.CurrentDirectory, chromeOptions);
        }

        [Test]
        public void WithoutPassenger()
        {
            using (driver)
            {
                driver.Navigate().GoToUrl("http://www.amazon.co.uk");
                var myElement = driver.FindElementById("twotabsearchtextbox");

                myElement.Click();
                myElement.SendKeys("Game of thrones");

                var goButton = driver.FindElementByClassName("nav-searchbar");
                goButton.Submit();

                var allH2s = driver.FindElementsByTagName("h2");

                var oneWithGameOfThrones = allH2s.Where(x => x.Text == "Game of Thrones - Season 4");

                Assert.That(oneWithGameOfThrones, Is.Not.Null);
            }
        }

        [Test]
        public void WithPassenger()
        {
            var testConfig = new PassengerConfiguration
            {
                WebRoot = "http://www.amazon.co.uk"
            }.WithDriver(driver);

            using (var context = testConfig.StartTestAt<Homepage>())
            {
                context.Page<Homepage>().SearchFor("Game of thrones");
                var ex = context.Page<SearchResultsPage>().AllH2s.Where(x => x.Text == "Game of Thrones - Season 4");

                Assert.That(ex, Is.Not.Null);
            }
        }

        [Test]
        public void SomethingElse()
        {
            var testConfig = new PassengerConfiguration
            {
                WebRoot = "http://www.amazon.co.uk"
            }.WithDriver(driver);

            using (var context = testConfig.StartTestAt<Homepage>())
            {
                context
                    .Page<Homepage>()
                    .SearchFor("Game of thrones")
                    .SearchResultSomething();
            }
        }

        [Uri("/")]
        public class Homepage
        {
            public virtual RemoteWebDriver Driver { get; set; }

            [Id("twotabsearchtextbox")]
            public virtual IWebElement SearchBox { get; set; }

            [ClassName("nav-searchbar")]
            public virtual IWebElement SearchForm { get; set; }

            public virtual SearchResultsPage SearchFor(string thing)
            {
                SearchBox.Click();
                SearchBox.SendKeys(thing);
                SearchForm.Submit();

                return Arrives.At<SearchResultsPage>();
            }
        }

        [Uri("/s")]
        public class SearchResultsPage
        {
            [TagName("h2")]
            public virtual IEnumerable<IWebElement> AllH2s { get; set; }

            public virtual Menu MyMenu { get; set; }

            public Homepage SearchResultSomething()
            {
                return Arrives.At<Homepage>();
            }
        }

        [PageComponent]
        public class Menu
        {

        }
    }
}