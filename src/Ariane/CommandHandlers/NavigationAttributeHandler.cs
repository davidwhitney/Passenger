using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ariane.Attributes;
using OpenQA.Selenium.Remote;

namespace Ariane.CommandHandlers
{
    public class NavigationAttributeHandler : IHandleNavigation
    {
        private readonly RemoteWebDriver _driver;
        private readonly Attribute _attr;

        private readonly Dictionary<Type, AttributeMap> _attributeHandlingMap = new Dictionary<Type, AttributeMap>
        {
            {typeof (IdAttribute), new AttributeMap(attr => ((IdAttribute) attr).Id, (key, d) => d.FindElementsById(key))},
            {typeof (CssSelectorAttribute), new AttributeMap(attr => ((CssSelectorAttribute) attr).Selector, (key, d) => d.FindElementsById(key))},
            {typeof (NameAttribute), new AttributeMap(attr => ((NameAttribute) attr).Name, (key, d) => d.FindElementsById(key))},
            {typeof (TextAttribute), new AttributeMap(attr => ((TextAttribute) attr).String, (key, d) => d.FindElementsById(key))},
        };

        public NavigationAttributeHandler(Attribute attr, RemoteWebDriver driver)
        {
            _driver = driver;
            _attr = attr;
        }

        public object InvokeSeleniumSelection(PropertyInfo property)
        {
            var handlingFunc = _attributeHandlingMap.SingleOrDefault(kvp => _attr.GetType() == kvp.Key);

            var textValue = handlingFunc.Value.GetLookupValue(_attr);
            var textKey = textValue ?? property.Name;
            var matches = handlingFunc.Value.FindAllMatches(textKey, _driver);

            if (property.PropertyType.GetInterfaces().Any(x => x.Name.ToLower().Contains("enumerable")))
            {
                return matches;
            }
            
            var enumberableObjects = (IEnumerable<object>) matches;
            return enumberableObjects.SingleOrDefault();
        }
    }
}