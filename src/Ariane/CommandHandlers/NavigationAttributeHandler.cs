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

        private readonly List<IHandle> _attributeHandlingMap = new List<IHandle>
        {
            new Handle<IdAttribute>(a => ((IdAttribute) a).Id, (key, d) => d.FindElementsById(key)),
            new Handle<NameAttribute>(a => ((NameAttribute) a).Name, (key, d) => d.FindElementsById(key)),
            new Handle<TextAttribute>(a => ((TextAttribute) a).String, (key, d) => d.FindElementsById(key)),
            new Handle<CssSelectorAttribute>(a => ((CssSelectorAttribute) a).Selector, (key, d) => d.FindElementsById(key)),
        };

        public NavigationAttributeHandler(Attribute attr, RemoteWebDriver driver)
        {
            _driver = driver;
            _attr = attr;
        }

        public object InvokeSeleniumSelection(PropertyInfo property)
        {
            var attributeHandler = _attributeHandlingMap.SingleOrDefault(map => _attr.GetType() == map.AttributeType);
            
            var textValue = attributeHandler.GetLookupValue(_attr);
            var matches = attributeHandler.FindAllMatches(textValue ?? property.Name, _driver);

            if (property.PropertyType.GetInterfaces().Any(x => x.Name.ToLower().Contains("enumerable")))
            {
                return matches;
            }
            
            var enumberableObjects = (IEnumerable<object>) matches;
            return enumberableObjects.SingleOrDefault();
        }

        private class Handle<TAttributeType> : IHandle
        {
            public Type AttributeType { get { return typeof(TAttributeType); } }
            public Func<Attribute, string> GetLookupValue { get; private set; }
            public Func<string, RemoteWebDriver, object> FindAllMatches { get; private set; }

            public Handle(Func<Attribute, string> getLookupValue, Func<string, RemoteWebDriver, object> findAllMatches)
            {
                GetLookupValue = getLookupValue;
                FindAllMatches = findAllMatches;
            }
        }

        private interface IHandle
        {
            Type AttributeType { get; }
            Func<Attribute, string> GetLookupValue { get; }
            Func<string, RemoteWebDriver, object> FindAllMatches { get; }
        }
    }
}