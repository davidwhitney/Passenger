using System.Reflection;
using Ariane.Attributes;
using OpenQA.Selenium.Remote;

namespace Ariane.CommandHandlers
{
    public class TextAttributeHandler : IHandleNavigation
    {
        private readonly TextAttribute _attribute;
        private readonly RemoteWebDriver _driver;

        public TextAttributeHandler(TextAttribute attribute, RemoteWebDriver driver)
        {
            _attribute = attribute;
            _driver = driver;
        }

        public object InvokeSeleniumSelection(PropertyInfo property)
        {
            return property.WhenEnumerable(
                () => _driver.FindElementsByLinkText(_attribute.String),
                () => _driver.FindElementByLinkText(_attribute.String));
        }
    }
}