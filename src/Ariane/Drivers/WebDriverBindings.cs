using System;
using System.Collections.Generic;
using Ariane.Attributes;
using OpenQA.Selenium.Remote;

namespace Ariane.Drivers
{
    public class WebDriverBindings : DriverBindings
    {
        public RemoteWebDriver Driver { get; private set; }
        public override string Url { get { return Driver.Url; } }

        public WebDriverBindings(RemoteWebDriver driver)
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

        public override IEnumerable<IHandle> NavigationHandlers
        {
            get
            {
                return new List<IHandle>
                {
                    new Handle<IdAttribute>(a => ((IdAttribute) a).Id,
                        (key, d) => ((WebDriverBindings) d).Driver.FindElementsById(key)),

                    new Handle<NameAttribute>(a => ((NameAttribute) a).Name,
                        (key, d) => ((WebDriverBindings) d).Driver.FindElementsByName(key)),

                    new Handle<TextAttribute>(a => ((TextAttribute) a).String,
                        (key, d) => ((WebDriverBindings) d).Driver.FindElementsByLinkText(key)),

                    new Handle<CssSelectorAttribute>(a => ((CssSelectorAttribute) a).Selector,
                        (key, d) => ((WebDriverBindings) d).Driver.FindElementsByCssSelector(key)),
                };
            }
        }

        public static implicit operator WebDriverBindings(RemoteWebDriver driver)
        {
            return new WebDriverBindings(driver);
        }
    }
}