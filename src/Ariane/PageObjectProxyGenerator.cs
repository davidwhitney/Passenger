using Castle.DynamicProxy;
using OpenQA.Selenium.Remote;

namespace Ariane
{
    public static class PageObjectProxyGenerator
    {
        public static TPageObjectType Generate<TPageObjectType>(RemoteWebDriver driver) where TPageObjectType : class
        {
            var handlerRegistry = new NavigationHandlerRegistry();

            var generator = new ProxyGenerator();
            var pageObjectProxy = new PageObjectProxy(driver, handlerRegistry);
            return generator.CreateClassProxy<TPageObjectType>(pageObjectProxy);
        }
    }
}