using System;
using Passenger.Drivers;
using Castle.DynamicProxy;

namespace Passenger.ModelInterception
{
    public static class PageObjectProxyGenerator
    {
        public static PageObject Generate(IDriverBindings driver, Type pageObjectType)
        {
            var wrappedProxy = Generate(pageObjectType, driver);
            var typePageObjectOfT = typeof(PageObject<>).MakeGenericType(pageObjectType);
            return (PageObject)Activator.CreateInstance(typePageObjectOfT, driver, wrappedProxy);
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

            var generator = new ProxyGenerator();
            var pageObjectProxy = new PageObjectProxy(driver);
            return generator.CreateClassProxy(propertyType, pageObjectProxy);
        }

        public static T GenerateNavigationProxy<T>()
        {
            var generator = new ProxyGenerator();
            var pageObjectProxy = new NavigationProxy();
            return (T)generator.CreateClassProxy(typeof(T), pageObjectProxy);
        }
    }
}