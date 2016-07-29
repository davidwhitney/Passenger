using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Passenger.Drivers;

namespace Passenger.CommandHandlers
{
    public class ElementSelectionHandler
    {
        private readonly IDriverBindings _driver;

        public ElementSelectionHandler(PassengerConfiguration configuration)
        {
            _driver = configuration.Driver;
        }

        public object SelectElement(Attribute attr, PropertyInfo property)
        {
            if (attr == null || property == null)
            {
                return null;
            }

            var elements = ExecuteAttributeHandler(attr, property);
            if (property.PropertyType == elements.GetType())
            {
                return elements;
            }

            if (elements.GetType().IsCollection() 
                && !property.PropertyType.IsCollection())
            {
                elements = SelectFirstItemFrom(elements, property);
            }

            return elements;
        }

        private DriverBindings.IHandle SelectHandlerForNavigationAttribute(Attribute attr, PropertyInfo property)
        {
            var attributeHandler = _driver.NavigationHandlers.SingleOrDefault(map => attr.GetType() == map.AttributeType);
            if (attributeHandler == null)
            {
                throw new NavigationTypeNotSupportedException(attr, property.Name);
            }
            return attributeHandler;
        }

        private object ExecuteAttributeHandler(Attribute attr, PropertyInfo property)
        {
            var attributeHandler = SelectHandlerForNavigationAttribute(attr, property);
            var key = string.IsNullOrWhiteSpace(attr.ToString()) ? property.Name : attr.ToString();
            return attributeHandler.FindAllMatches(key, _driver);
        }

        private static object SelectFirstItemFrom(object elements, PropertyInfo property)
        {
            try
            {
                var asEnumerable = (IEnumerable) elements;
                var enumerator = asEnumerable.GetEnumerator();
                enumerator.MoveNext();
                return enumerator.Current;
            }
            catch (Exception ex)
            {
                throw new PropertySelectionFailureException(property, ex);
            }
        }
    }
}