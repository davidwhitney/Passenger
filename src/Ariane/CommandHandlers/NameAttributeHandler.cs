using System;
using System.Reflection;
using Ariane.Attributes;
using OpenQA.Selenium.Remote;

namespace Ariane.CommandHandlers
{
    public class NameAttributeHandler : IHandleNavigation
    {
        private readonly NameAttribute _attribute;
        private readonly RemoteWebDriver _driver;

        public NameAttributeHandler(NameAttribute attribute, RemoteWebDriver driver)
        {
            _attribute = attribute;
            _driver = driver;
        }

        public object InvokeSeleniumSelection(PropertyInfo property)
        {
            return property.WhenEnumerable(
                () => _driver.FindElementsByName(_attribute.Name),
                () => _driver.FindElementByName(_attribute.Name));
        }
    }
}