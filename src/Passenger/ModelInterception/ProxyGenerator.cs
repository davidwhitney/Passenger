using System;

namespace Passenger.ModelInterception
{
    public static class ProxyGenerator
    {
        public static PageObject GenerateWrappedPageObject(Type pageObjectType, PassengerConfiguration driver)
        {
            var wrappedProxy = Generate(pageObjectType, driver);
            var typePageObjectOfT = typeof(PageObject<>).MakeGenericType(pageObjectType);
            return (PageObject)Activator.CreateInstance(typePageObjectOfT, driver, wrappedProxy);
        }

        public static TPageObjectType Generate<TPageObjectType>(PassengerConfiguration configuration) where TPageObjectType : class
        {
            return (TPageObjectType)Generate(typeof (TPageObjectType), configuration);
        }

        public static T GenerateNavigationProxy<T>()
        {
            return (T)new Castle.DynamicProxy.ProxyGenerator().CreateClassProxy(typeof(T));
        }

        public static object Generate(Type propertyType, PassengerConfiguration configuration)
        {
            if (propertyType == null) throw new ArgumentNullException(nameof(propertyType));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return new Castle.DynamicProxy.ProxyGenerator().CreateClassProxy(propertyType, new PageObjectProxy(configuration));
        }
    }
}