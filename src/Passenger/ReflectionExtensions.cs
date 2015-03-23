using System;
using System.Linq;
using System.Reflection;
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

        public static bool IsPageComponent(this PropertyInfo property)
        {
            return property.PropertyType.GetCustomAttributes().Any(attr => attr.GetType() == typeof (PageComponentAttribute));
        }

        public static bool IsCollection(this Type property)
        {
            if (property == typeof(string))
            {
                return false;
            }

            return property.GetInterfaces().Any(x => x.Name.ToLower().Contains("enumerable"));
        }
    }
}