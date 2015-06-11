using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using Passenger.Attributes;

namespace Passenger.CommandHandlers
{
    public static class TypeMapping
    {
        public static object ReturnOrMap(object sourceElement, Type targetType)
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
                return BuildCollectionOfWrappers(sourceElement, targetType);
            }

            return sourceElement;
        }

        private static IEnumerable BuildCollectionOfWrappers(object sourceElement, Type targetType)
        {
            var elementType = GetElementType(targetType);
            var passengerItems = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
            
            foreach (var item in ((IEnumerable) sourceElement))
            {
                passengerItems.Add(WrapIntoPassengerElement(item, elementType));
            }

            if (targetType.IsArray)
            {
                return TypedArray(elementType, passengerItems);
            }

            return (IEnumerable)Activator.CreateInstance(targetType, passengerItems);
        }

        private static IEnumerable TypedArray(Type elementType, IList passengerItems)
        {
            var array = Array.CreateInstance(elementType, passengerItems.Count);
            for (var index = 0; index < passengerItems.Count; index++)
            {
                array.SetValue(passengerItems[index], index);
            }
            return array;
        }

        private static Type GetElementType(Type targetType)
        {
            if (targetType.GetElementType() != null)
            {
                return targetType.GetElementType();
            }

            var genericArgs = targetType.GetGenericArguments().FirstOrDefault();
            if (genericArgs == null)
            {
                throw new Exception("Can't map to collection");
            }
            return genericArgs;
        }

        private static IPassengerElement WrapIntoPassengerElement(object singleElement, Type targetType)
        {
            var wrapper = (IPassengerElement)Activator.CreateInstance(targetType);
            wrapper.Inner = (IWebElement)singleElement;
            return wrapper;
        }
    }
}