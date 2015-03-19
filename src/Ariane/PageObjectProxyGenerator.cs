using System;
using Castle.DynamicProxy;
using OpenQA.Selenium.Remote;

namespace Ariane
{
    public static class PageObjectProxyGenerator
    {
        public static TPageObjectType Generate<TPageObjectType>(RemoteWebDriver driver) where TPageObjectType : class
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