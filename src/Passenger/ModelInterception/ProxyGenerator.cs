using System;
using Passenger.Drivers;

namespace Passenger.ModelInterception
{
    public static class ProxyGenerator
    {
        public static PageObject GenerateWrappedPageObject(Type pageObjectType, IDriverBindings driver)
        {
            var wrappedProxy = Generate(pageObjectType, driver);
            var typePageObjectOfT = typeof(PageObject<>).MakeGenericType(pageObjectType);
            return (PageObject)Activator.CreateInstance(typePageObjectOfT, driver, wrappedProxy);
        }

        public static T GenerateNavigationProxy<T>()
        {
            return (T)new Castle.DynamicProxy.ProxyGenerator().CreateClassProxy(typeof (T), new NavigationProxy());
        }

        public static TPageObjectType Generate<TPageObjectType>(IDriverBindings driver) where TPageObjectType : class
        {
            return (TPageObjectType)Generate(typeof (TPageObjectType), driver);
        }

        public static object Generate(Type propertyType, IDriverBindings driver)
        {
            if (driver == null)
            {
                throw new ArgumentNullException("driver");
            }

            return new Castle.DynamicProxy.ProxyGenerator().CreateClassProxy(propertyType, new PageObjectProxy(driver));
        }
    }
}