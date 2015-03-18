using System;
using System.Reflection;
using Ariane.Attributes;
using OpenQA.Selenium.Remote;

namespace Ariane
{
    public class PageObjectTestContext<TPageObjectType> : IDisposable where TPageObjectType : class
    {
        public RemoteWebDriver Driver { get; set; }
        public string WebRoot { get; set; }

        private object _currentProxy;

        public PageObjectTestContext(RemoteWebDriver driver, string webRoot)
        {
            Driver = driver;
            WebRoot = webRoot;

            GoTo<TPageObjectType>();
        }

        public TNextPageObjectType GoTo<TNextPageObjectType>() where TNextPageObjectType : class
        {
            _currentProxy = CreateOrReturnProxy<TNextPageObjectType>();
            Driver.Navigate().GoToUrl(WebRootOf(_currentProxy));
            return (TNextPageObjectType)_currentProxy;
        }

        public TCurrentPageObjectType Page<TCurrentPageObjectType>() where TCurrentPageObjectType : class
        {
            return CreateOrReturnProxy<TCurrentPageObjectType>();
        }

        private TCurrentPageObjectType CreateOrReturnProxy<TCurrentPageObjectType>() where TCurrentPageObjectType : class
        {
            if (_currentProxy == null || _currentProxy.GetType().BaseType != typeof (TCurrentPageObjectType))
            {
                _currentProxy = PageObjectProxyGenerator.Generate<TCurrentPageObjectType>(Driver);
            }

            return (TCurrentPageObjectType) _currentProxy;
        }

        private string WebRootOf(object classProxy)
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