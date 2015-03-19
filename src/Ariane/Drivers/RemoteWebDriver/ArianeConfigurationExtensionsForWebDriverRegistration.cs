namespace Ariane.Drivers.RemoteWebDriver
{
    public static class ArianeConfigurationExtensionsForWebDriverRegistration
    {
        public static ArianeConfiguration WithDriver(this ArianeConfiguration cfg, OpenQA.Selenium.Remote.RemoteWebDriver driver)
        {
            cfg.Driver = new WebDriverBindings(driver);
            return cfg;
        }
    }
}