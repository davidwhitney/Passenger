using System;
using System.Collections;
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
                throw new NavigationTypeNotSupportedException(attr, property.Name);
            }

            var textValue = attr.ToString();
            var key = string.IsNullOrWhiteSpace(textValue) ? property.Name : textValue;
            var allMatches = attributeHandler.FindAllMatches(key, _driver);

            if (IsCollection(property))
            {
                return allMatches;
            }
            
            var enumerator = ((IEnumerable) allMatches).GetEnumerator();
            enumerator.MoveNext();
            return enumerator.Current;
        }

        private static bool IsCollection(PropertyInfo property)
        {
            var isCollection = property.PropertyType.GetInterfaces().Any(x => x.Name.ToLower().Contains("enumerable"));
            return isCollection;
        }
    }
}