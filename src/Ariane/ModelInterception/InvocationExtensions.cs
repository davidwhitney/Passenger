using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ariane.Attributes;
using Castle.DynamicProxy;

namespace Ariane.ModelInterception
{
    public static class InvocationExtensions
    {
        public static bool IsProperty(this IInvocation invocation)
        {
            return invocation.Method.Name.StartsWith("get_") || invocation.IsSetProperty();
        }

        public static bool IsSetProperty(this IInvocation invocation)
        {
            return invocation.Method.Name.StartsWith("set_");
        }

        public static PropertyInfo ToPropertyInfo(this IInvocation invocation)
        {
            var declaringType = invocation.Method.DeclaringType;
            var propertyName = invocation.Method.Name.Remove(0, 4);
            return declaringType.GetProperty(propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        public static bool IsPageComponent(this PropertyInfo property)
        {
            return property.PropertyType.GetCustomAttributes().Any(attr => attr.GetType() == typeof (PageComponentAttribute));
        }
    }
}