using System;
using Castle.DynamicProxy;
using OpenQA.Selenium.Remote;

namespace Ariane
{
    public class PageObjectTest
    {
        public Func<RemoteWebDriver> WithDriver { get; set; }
        public Func<string> WebRoot { get; set; }

        public PageObjectTestContext<TPageObjectType> StartAt<TPageObjectType>() where TPageObjectType : class
        {
            var driver = WithDriver();
            var generator = new ProxyGenerator();
            var pageObjectProxy = new PageObjectProxy(driver);
            var classProxy = generator.CreateClassProxy<TPageObjectType>(pageObjectProxy);

            return new PageObjectTestContext<TPageObjectType>(classProxy, driver, WebRoot());
        }
    }
}
