using System;
using System.Reflection;
using Castle.DynamicProxy;
using OpenQA.Selenium.Remote;

namespace Ariane
{
    public class PageObjectFactory
    {
        public Func<RemoteWebDriver> WithDriver { get; set; } 

        public TPageObjectType Load<TPageObjectType>() where TPageObjectType : class
        {
            var driver = WithDriver();
            var generator = new ProxyGenerator();
            var pageObjectProxy = new PageObjectProxy(driver);

            var classProxy = generator.CreateClassProxy<TPageObjectType>(pageObjectProxy);

            var root = GetRoot(classProxy);
            driver.Navigate().GoToUrl(root);

            return classProxy;
        }

        private string GetRoot(object classProxy)
        {
            var attr = classProxy.GetType().GetCustomAttribute<UriAttribute>();

            if (attr == null)
            {
                throw new Exception("Cannot navigate to a Page Object that doesn't have a [Uri(\"http://tempuri.org\")] attribute.");
            }

            return attr.Uri.ToString();
        }
    }
}
