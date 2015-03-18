using System.Reflection;
using Ariane.Attributes;
using OpenQA.Selenium.Remote;

namespace Ariane.CommandHandlers
{
    public class IdAttributeHandler : IHandleNavigation
    {
        private readonly IdAttribute _attribute;
        private readonly RemoteWebDriver _driver;

        public IdAttributeHandler(IdAttribute attribute, RemoteWebDriver driver)
        {
            _attribute = attribute;
            _driver = driver;
        }

        public object InvokeSeleniumSelection(PropertyInfo property)
        {
            return property.WhenEnumerable(
                () => _driver.FindElementsById(_attribute.Id),
                () => _driver.FindElementById(_attribute.Id));
        }
    }
}