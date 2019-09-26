using Passenger.Drivers.RemoteWebDriver;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;

namespace Passenger.Test.Unit.Drivers.RemoteWebDriver
{
    [TestFixture]
    public class PassengerConfigurationExtensionsForWebDriverRegistrationTests
    {
        [Test]
        public void WithDriver__GivenDriver_AssignsDriver()
        {
            var driver = (ChromeDriver) System.Runtime.Serialization.FormatterServices.GetSafeUninitializedObject(typeof (ChromeDriver)); // Sorry mom
            
            var cfg = new PassengerConfiguration().WithDriver(driver);

            Assert.That(cfg.Driver, Is.InstanceOf<WebDriverBindings>());
        }
    }
}
