using Passenger.Drivers.RemoteWebDriver;
using NUnit.Framework;
using OpenQA.Selenium.PhantomJS;

namespace Passenger.Test.Unit.Drivers.RemoteWebDriver
{
    [TestFixture]
    public class PassengerConfigurationExtensionsForWebDriverRegistrationTests
    {
        [Test]
        public void WithDriver__GivenDriver_AssignsDriver()
        {
            var driver = (PhantomJSDriver) System.Runtime.Serialization.FormatterServices.GetSafeUninitializedObject(typeof (PhantomJSDriver)); // Sorry mom
            
            var cfg = new PassengerConfiguration().WithDriver(driver);

            Assert.That(cfg.Driver, Is.InstanceOf<WebDriverBindings>());
        }
    }
}
