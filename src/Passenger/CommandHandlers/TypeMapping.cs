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
            var wrapped = new List<IPassengerElement>();
            foreach (var item in ((IEnumerable) sourceElement))
            {
                var passengerElement = WrapIntoPassengerElement(item, elementType);
                wrapped.Add(passengerElement);
            }

            var inst = (IList)Activator.CreateInstance(targetType);
            foreach (var passengerElement in wrapped)
            {
                inst.Add(passengerElement);
            }
            return inst;
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