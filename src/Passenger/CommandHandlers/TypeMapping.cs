using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Castle.Components.DictionaryAdapter.Xml;
using OpenQA.Selenium;
using Passenger.Attributes;
using Passenger.ModelInterception;

namespace Passenger.CommandHandlers
{
    public static class TypeMapping
    {
        public static object ReturnOrMap(object sourceElement, Type targetType, PassengerConfiguration cfg)
        {
            if (sourceElement == null)
            {
                return null;
            }

            if (sourceElement.GetType().IsAWebElement()
                && targetType.IsAPassengerElement())
            {
                return WrapIntoPassengerElement(sourceElement, targetType);
            }

            if (sourceElement.GetType().IsCollection()
                && targetType.IsCollection())
            {
                return BuildCollectionOfWrappers(sourceElement, targetType, cfg);
            }

            return sourceElement;
        }

        public static IEnumerable BuildCollectionOfWrappers(object sourceElement, Type targetType, PassengerConfiguration configuration)
        {
            var elementType = targetType.GetElementItemType();

            var wrappedItems = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
            foreach (var item in (IEnumerable) sourceElement)
            {
                var itemToAdd = CreateWrappedOrProxiedItem(elementType, item, configuration);
                wrappedItems.Add(itemToAdd);
            }

            if (targetType.IsArray)
            {
                return wrappedItems.ToArray(elementType);
            }

            var enumerableOfElementType = typeof(IEnumerable<>).MakeGenericType(elementType);
            if (targetType.Implements(enumerableOfElementType)
                || targetType == enumerableOfElementType)
            {
                return wrappedItems;
            }

            return (IEnumerable)Activator.CreateInstance(targetType, wrappedItems);
        }

        private static object CreateWrappedOrProxiedItem(Type ofType, object sourceItem, PassengerConfiguration configuration)
        {
            var itemToAdd = sourceItem;

            if (ofType.IsAPassengerElement())
            {
                itemToAdd = WrapIntoPassengerElement(sourceItem, ofType);
            }

            if (ofType.IsPageComponent())
            {
                itemToAdd = ProxyGenerator.Generate(ofType, configuration);
            }

            return itemToAdd;
        }

        public static IPassengerElement WrapIntoPassengerElement(object singleElement, Type targetType)
        {
            var wrapper = (IPassengerElement)Activator.CreateInstance(targetType);
            wrapper.Inner = (IWebElement)singleElement;
            return wrapper;
        }
    }
}