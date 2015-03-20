using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ariane.Drivers;

namespace Ariane.CommandHandlers
{
    public class ElementSelectionHandler
    {
        private readonly IDriverBindings _driver;

        public ElementSelectionHandler(IDriverBindings driver)
        {
            _driver = driver;
        }

        public object SelectElement(Attribute attr, PropertyInfo property)
        {
            if (attr == null || property == null)
            {
                return null;
            }

            var attributeHandler = _driver.NavigationHandlers.SingleOrDefault(map => attr.GetType() == map.AttributeType);
            if (attributeHandler == null)
            {
                return null;
            }

            var textValue = attr.ToString();
            var key = string.IsNullOrWhiteSpace(textValue) ? property.Name : textValue;
            var allMatches = attributeHandler.FindAllMatches(key, _driver);

            return IsCollection(property)
                ? allMatches
                : ((IEnumerable<object>) allMatches).SingleOrDefault();
        }

        private static bool IsCollection(PropertyInfo property)
        {
            return property.PropertyType.GetInterfaces().Any(x => x.Name.ToLower().Contains("enumerable"));
        }
    }
}