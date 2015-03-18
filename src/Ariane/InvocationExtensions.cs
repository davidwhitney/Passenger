using System.Reflection;
using Castle.DynamicProxy;

namespace Ariane
{
    public static class InvocationExtensions
    {
        public static bool IsProperty(this IInvocation invocation)
        {
            return invocation.Method.Name.StartsWith("get_") || invocation.Method.Name.StartsWith("set_");
        }

        public static PropertyInfo ToPropertyInfo(this IInvocation invocation)
        {
            var declaringType = invocation.Method.DeclaringType;
            var propertyName = invocation.Method.Name.Remove(0, 4);
            return declaringType.GetProperty(propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            
        }
    }
}