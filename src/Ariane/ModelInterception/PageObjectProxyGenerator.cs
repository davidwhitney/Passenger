using System;
using Ariane.Drivers;
using Castle.DynamicProxy;

namespace Ariane.ModelInterception
{
    public static class PageObjectProxyGenerator
    {
        public static TPageObjectType Generate<TPageObjectType>(IDriverBindings driver) where TPageObjectType : class
        {
            if (driver == null)
            {
                throw new ArgumentNullException("driver");
            }

            var generator = new ProxyGenerator();
            var pageObjectProxy = new PageObjectProxy(driver);
            return generator.CreateClassProxy<TPageObjectType>(pageObjectProxy);
        }
    }
}