using System;
using System.Reflection;
using Castle.DynamicProxy;
using OpenQA.Selenium.Remote;

namespace Ariane
{
    public class PageObjectFactory
    {
        public Func<RemoteWebDriver> WithDriver { get; set; }
        public Func<string> WebRoot { get; set; }

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

            if (attr.Uri.IsAbsoluteUri)
            {
                return attr.Uri.ToString();
            }

            var root = WebRoot();
            if (string.IsNullOrWhiteSpace(root))
            {
                throw new Exception("You need to configure a WebRoot to use relative Uris");
            }

            return new Uri(new Uri(root), attr.Uri).ToString();
        }
    }
}
