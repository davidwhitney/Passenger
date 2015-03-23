using System;
using Passenger.Drivers;
using Castle.DynamicProxy;

namespace Passenger.ModelInterception
{
    public static class PageObjectProxyGenerator
    {
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
    }
}