using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ariane.Drivers;

namespace Ariane.CommandHandlers
{
    public class NavigationAttributeHandler : IHandleNavigation
    {
        private readonly IDriverBindings _driver;
        private readonly Attribute _attr;

        public NavigationAttributeHandler(Attribute attr, IDriverBindings driver)
        {
            _driver = driver;
            _attr = attr;
        }

        public object InvokeDriver(PropertyInfo property)
        {
            var attributeHandler = _driver.NavigationHandlers.SingleOrDefault(map => _attr.GetType() == map.AttributeType);
            
            var textValue = attributeHandler.GetLookupValue(_attr);
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