using System;
using Castle.DynamicProxy;
using OpenQA.Selenium.Remote;

namespace Ariane
{
    public class PageObjectFactory
    {
        public Func<RemoteWebDriver> WithDriver { get; set; } 

        public TPageObjectType GetPage<TPageObjectType>() where TPageObjectType : class, ICanBeNavigatedTo
        {
            var driver = WithDriver();
            var generator = new ProxyGenerator();
            var pageObjectProxy = new PageObjectProxy(driver);

            var classProxy = generator.CreateClassProxy<TPageObjectType>(pageObjectProxy);

            var root = classProxy.Url;
            driver.Navigate().GoToUrl(root);

            return classProxy;
        }
    }

    public interface ICanBeNavigatedTo
    {
        Uri Url { get; }
    }
}
