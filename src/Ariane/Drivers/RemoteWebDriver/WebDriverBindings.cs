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
                    new Handle<IdAttribute>((key, d) => ((WebDriverBindings) d).Driver.FindElementsById(key)),
                    new Handle<NameAttribute>((key, d) => ((WebDriverBindings) d).Driver.FindElementsByName(key)),
                    new Handle<TextAttribute>((key, d) => ((WebDriverBindings) d).Driver.FindElementsByLinkText(key)),
                    new Handle<CssSelectorAttribute>((key, d) => ((WebDriverBindings) d).Driver.FindElementsByCssSelector(key)),
                };
            }
        }
    }
}