using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Remote;
using Passenger;
using Passenger.Attributes;
using Passenger.Drivers.RemoteWebDriver;

namespace ClassLibrary1
{
    [TestFixture]
    public class AmazonExample
    {
        [Test]
        public void WithoutPassenger()
        {
            using (var webdriver = new PhantomJSDriver())
            {
                webdriver.Navigate().GoToUrl("http://www.amazon.co.uk");
                var myElement = webdriver.FindElementById("twotabsearchtextbox");

                myElement.Click();
                myElement.SendKeys("Game of thrones");

                var goButton = webdriver.FindElementByClassName("nav-searchbar");
                goButton.Submit();

                var allH2s = webdriver.FindElementsByTagName("h2");

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
            }.WithDriver(new PhantomJSDriver());

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
            }.WithDriver(new PhantomJSDriver());

            using (var context = testConfig.StartTestAt<Homepage>())
            {
                context
                    .Page<Homepage>()
                    .SearchFor("Game of thrones")
                    .SearchResultSomething();
            }
        }

        [Uri("/")]
        public class Homepage : IEnableChaining
        {
            public virtual RemoteWebDriver Driver { get; set; }
            public virtual TType ArriveAt<TType>() { return default(TType); }

            [Id("twotabsearchtextbox")]
            public virtual IWebElement SearchBox { get; set; }

            [CssSelector("nav-searchbar")]
            public virtual IWebElement SearchForm { get; set; }

            public virtual SearchResultsPage SearchFor(string thing)
            {
                SearchBox.Click();
                Driver.Keyboard.SendKeys(thing);
                //SearchForm.Submit();

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
