using System;
using System.Linq;
using System.Reflection;

namespace Ariane.CommandHandlers
{
    public static class PropertyInfoExtensions
    {
        public static bool PropertyIsCollection(this PropertyInfo targetProperty)
        {
            return targetProperty.PropertyType.GetInterfaces().Any(x => x.Name.ToLower().Contains("enumerable"));
        }

        public static object WhenEnumerable(this PropertyInfo targetProperty, Func<object> selectMany, Func<object> otherwise)
        {
            return targetProperty.PropertyIsCollection() ? selectMany() : otherwise();
        }
    }
}