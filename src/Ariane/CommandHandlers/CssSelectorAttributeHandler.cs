using System.Reflection;
using Ariane.Attributes;
using OpenQA.Selenium.Remote;

namespace Ariane.CommandHandlers
{
    public class CssSelectorAttributeHandler : IHandleNavigation
    {
        private readonly CssSelectorAttribute _attribute;
        private readonly RemoteWebDriver _driver;

        public CssSelectorAttributeHandler(CssSelectorAttribute attribute, RemoteWebDriver driver)
        {
            _attribute = attribute;
            _driver = driver;
        }

        public object InvokeSeleniumSelection(PropertyInfo property)
        {
            return property.WhenEnumerable(
                () => _driver.FindElementsByCssSelector(_attribute.Selector),
                () => _driver.FindElementByCssSelector(_attribute.Selector));
        }
    }
}