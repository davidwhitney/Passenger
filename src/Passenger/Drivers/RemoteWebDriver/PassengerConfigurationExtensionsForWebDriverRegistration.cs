namespace Passenger.Drivers.RemoteWebDriver
{
    public static class PassengerConfigurationExtensionsForWebDriverRegistration
    {
        public static PassengerConfiguration WithDriver(this PassengerConfiguration cfg, OpenQA.Selenium.Remote.RemoteWebDriver driver)
        {
            cfg.Driver = new WebDriverBindings(driver);
            return cfg;
        }
    }
}