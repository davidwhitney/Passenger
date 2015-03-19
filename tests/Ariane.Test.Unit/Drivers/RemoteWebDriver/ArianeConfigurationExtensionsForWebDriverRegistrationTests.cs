using Ariane.Drivers.RemoteWebDriver;
using NUnit.Framework;
using OpenQA.Selenium.PhantomJS;

namespace Ariane.Test.Unit.Drivers.RemoteWebDriver
{
    [TestFixture]
    public class ArianeConfigurationExtensionsForWebDriverRegistrationTests
    {
        [Test]
        public void WithDriver__GivenDriver_AssignsDriver()
        {
            using (var driver = new PhantomJSDriver())
            {
                var cfg = new ArianeConfiguration().WithDriver(driver);

                Assert.That(cfg.Driver, Is.InstanceOf<WebDriverBindings>());
            }
        }
    }
}
