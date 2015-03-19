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

        private static readonly List<IHandle> AttributeHandlingMap = new List<IHandle>
        {
            new Handle<IdAttribute>(a => ((IdAttribute) a).Id, (key, d) => d.FindElementsById(key)),
            new Handle<NameAttribute>(a => ((NameAttribute) a).Name, (key, d) => d.FindElementsByName(key)),
            new Handle<TextAttribute>(a => ((TextAttribute) a).String, (key, d) => d.FindElementsByLinkText(key)),
            new Handle<CssSelectorAttribute>(a => ((CssSelectorAttribute) a).Selector, (key, d) => d.FindElementsByCssSelector(key)),
        };

        public NavigationAttributeHandler(Attribute attr, RemoteWebDriver driver)
        {
            _driver = driver;
            _attr = attr;
        }

        public static bool Supports(Type handlerType)
        {
            return AttributeHandlingMap.Any(map => handlerType == map.AttributeType);
        }

        public object InvokeSeleniumSelection(PropertyInfo property)
        {
            var attributeHandler = AttributeHandlingMap.SingleOrDefault(map => _attr.GetType() == map.AttributeType);
            
            var textValue = attributeHandler.GetLookupValue(_attr);
            var key = string.IsNullOrWhiteSpace(textValue) ? property.Name : textValue;
            var matches = attributeHandler.FindAllMatches(key, _driver);

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