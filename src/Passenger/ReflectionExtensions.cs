using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using OpenQA.Selenium;
using Passenger.Attributes;

namespace Passenger
{
    public static class ReflectionExtensions
    {
        public static bool IsProperty(this MemberInfo invocation)
        {
            return invocation.Name.StartsWith("get_") || invocation.IsSetProperty();
        }

        public static bool IsSetProperty(this MemberInfo invocation)
        {
            return invocation.Name.StartsWith("set_");
        }

        public static PropertyInfo ToPropertyInfo(this MethodInfo invocation)
        {
            var declaringType = invocation.DeclaringType;
            var propertyName = invocation.Name.Remove(0, 4);
            return declaringType.GetProperty(propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        public static Type EnumerableType(this IEnumerable col)
        {
            try
            {
                var enumerator = col.GetEnumerator();
                enumerator.MoveNext();
                var o = enumerator.Current;
                return o.GetType();
            }
            catch
            {
                return null;
            }
        }

        public static bool IsPageComponent(this object obj)
        {
            return obj.GetType().IsPageComponent();
        }

        public static bool IsPageComponent(this Type type)
        {
            return type.GetCustomAttributes().Any(attr => attr.GetType() == typeof(PageComponentAttribute));
        }

        public static bool IsPageComponent(this PropertyInfo property)
        {
            return property.PropertyType.IsPageComponent();
        }

        public static bool IsCollection(this object property)
        {
            if (property == null) return false;
            return property.GetType().IsCollection();
        }

        public static bool IsCollection(this Type property)
        {
            if (property == typeof(string))
            {
                return false;
            }

            return property.GetInterfaces().Any(x => x.Name.ToLower().Contains("enumerable"));
        }

        public static bool IsAProxy(this object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return obj.GetType().FullName.StartsWith("Castle.Proxies");
        }

        public static bool IsAPageObject(this Type type)
        {
            return type.Name.Contains("PageObject`1");
        }

        public static Type GetGenericParam(this Type type)
        {
            return type.GetGenericArguments().First();
        }

        public static bool IsAWebElement(this object obj)
        {
            if (obj == null) return false;
            return obj.GetType().IsAWebElement();
        }

        public static bool IsAWebElement(this Type type)
        {
            return type == typeof(IWebElement) || type.GetInterfaces().Contains(typeof (IWebElement));
        }

        public static bool IsAPassengerElement(this object obj)
        {
            if (obj == null) return false;
            return obj.GetType().IsAPassengerElement();
        }

        public static bool IsAPassengerElement(this Type type)
        {
            return type.GetInterfaces().Contains(typeof (IPassengerElement));
        }

        public static bool Implements(this Type type, Type interfaceType)
        {
            return type.GetInterfaces().Contains(interfaceType);
        }

        public static Type GetElementItemType(this Type collectionType)
        {
            if (collectionType.GetElementType() != null)
            {
                return collectionType.GetElementType();
            }

            var genericArgs = collectionType.GetGenericArguments().FirstOrDefault();
            if (genericArgs == null)
            {
                throw new NotSupportedException(string.Format("Cannot map to type '{0}' - try using List<T>, Type[] or other generic collection types.", collectionType.Name));
            }
            return genericArgs;
        }

        public static IEnumerable ToArray(this IList passengerItems, Type elementType)
        {
            var array = Array.CreateInstance(elementType, passengerItems.Count);
            for (var index = 0; index < passengerItems.Count; index++)
            {
                array.SetValue(passengerItems[index], index);
            }
            return array;
        }
    }
}