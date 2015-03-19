using System;
using Ariane.Drivers;
using OpenQA.Selenium.Remote;

namespace Ariane
{
    public class ArianeConfiguration
    {
        public DriverBindings Driver { get; private set; }
        public string WebRoot { get; set; }

        public ArianeConfiguration(RemoteWebDriver driver): this(new WebDriverBindings(driver))
        {
        }

        public ArianeConfiguration(DriverBindings driver)
        {
            Driver = driver;
        }

        public PageObjectTestContext<TPageObjectType> StartTestAt<TPageObjectType>() where TPageObjectType : class
        {
            return new PageObjectTestContext<TPageObjectType>(Driver, WebRoot);
        }
    }
}
