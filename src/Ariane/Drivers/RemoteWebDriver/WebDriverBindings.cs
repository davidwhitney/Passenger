using System;
using System.Collections.Generic;
using Ariane.Attributes;

namespace Ariane.Drivers.RemoteWebDriver
{
    public class WebDriverBindings : DriverBindings
    {
        public OpenQA.Selenium.Remote.RemoteWebDriver Driver { get; private set; }
        public override string Url { get { return Driver.Url; } }

        public WebDriverBindings(OpenQA.Selenium.Remote.RemoteWebDriver driver)
        {
            Driver = driver;
        }

        public override void NavigateTo(Uri url)
        {
            Driver.Navigate().GoToUrl(url);
        }

        public override void Dispose()
        {
            Driver.Close();
            Driver.Dispose();
        }

        public override IList<TypeSubstitution> Substitutes
        {
            get
            {
                return new List<TypeSubstitution>
                {
                    new TypeSubstitution(typeof(OpenQA.Selenium.Remote.RemoteWebDriver), ()=> Driver),
                    new TypeSubstitution(typeof(OpenQA.Selenium.IWebDriver), ()=> Driver),
                };
            }
        }

        public override IList<IHandle> NavigationHandlers
        {
            get
            {
                return new List<IHandle>
                {
                    new Handle<IdAttribute>((key, d) => WebDriver(d).FindElementsById(key)),
                    new Handle<NameAttribute>((key, d) => WebDriver(d).FindElementsByName(key)),
                    new Handle<TextAttribute>((key, d) => WebDriver(d).FindElementsByLinkText(key)),
                    new Handle<CssSelectorAttribute>((key, d) => WebDriver(d).FindElementsByCssSelector(key)),
                    new Handle<XPathAttribute>((key, d) => WebDriver(d).FindElementsByXPath(key)),
                    new Handle<TagNameAttribute>((key, d) => WebDriver(d).FindElementsByTagName(key)),
                };
            }
        }

        private static OpenQA.Selenium.Remote.RemoteWebDriver WebDriver(IDriverBindings d)
        {
            return ((WebDriverBindings) d).Driver;
        }
    }
}