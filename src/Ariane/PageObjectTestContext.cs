using System;
using System.Reflection;
using Ariane.Attributes;
using OpenQA.Selenium.Remote;

namespace Ariane
{
    public class PageObjectTestContext<TPageObjectType> : IDisposable
    {
        public TPageObjectType Page { get; set; }
        public RemoteWebDriver Driver { get; set; }
        public string WebRoot { get; set; }

        public PageObjectTestContext(TPageObjectType pageObjectProxy, RemoteWebDriver driver, string webRoot)
        {
            Page = pageObjectProxy;
            Driver = driver;
            WebRoot = webRoot;

            var root = GetRoot(pageObjectProxy);
            driver.Navigate().GoToUrl(root);
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

            if (string.IsNullOrWhiteSpace(WebRoot))
            {
                throw new Exception("You need to configure a WebRoot to use relative Uris");
            }

            return new Uri(new Uri(WebRoot), attr.Uri).ToString();
        }

        public void Dispose()
        {
            Driver.Close();
        }
    }
}