using System;
using System.Reflection;
using Passenger.Attributes;
using Passenger.Drivers;
using Passenger.ModelInterception;

namespace Passenger
{
    public class PageObjectTestContext<TPageObjectType> : IDisposable where TPageObjectType : class
    {
        public IDriverBindings Driver { get; set; }
        public string WebRoot { get; set; }

        private object _currentProxy;

        public PageObjectTestContext(IDriverBindings driver, string webRoot)
        {
            Driver = driver;
            WebRoot = webRoot;

            GoTo<TPageObjectType>();
        }

        public TNextPageObjectType GoTo<TNextPageObjectType>() where TNextPageObjectType : class
        {
            _currentProxy = CreateOrReturnProxy<TNextPageObjectType>();
            Driver.NavigateTo(UrlFor(_currentProxy));
            return (TNextPageObjectType)_currentProxy;
        }

        public TCurrentPageObjectType Page<TCurrentPageObjectType>() where TCurrentPageObjectType : class
        {
            return CreateOrReturnProxy<TCurrentPageObjectType>();
        }

        public void VerifyRedirectionTo<TNextPageObjectType>() where TNextPageObjectType : class
        {
            var nextPage = CreateOrReturnProxy<TNextPageObjectType>();
            var urlOfNextPage = UrlFor(nextPage);
            if (!Driver.Url.Contains(urlOfNextPage.PathAndQuery))
            {
                throw new Exception("We're not where we should be.");
            }
        }

        private TCurrentPageObjectType CreateOrReturnProxy<TCurrentPageObjectType>() where TCurrentPageObjectType : class
        {
            if (_currentProxy == null || _currentProxy.GetType().BaseType != typeof (TCurrentPageObjectType))
            {
                _currentProxy = PageObjectProxyGenerator.Generate<TCurrentPageObjectType>(Driver);
            }

            return (TCurrentPageObjectType) _currentProxy;
        }

        private Uri UrlFor(object classProxy)
        {
            var attr = classProxy.GetType().GetCustomAttribute<UriAttribute>();

            if (attr == null)
            {
                throw new Exception("Cannot navigate to a Page Object that doesn't have a [Uri(\"http://tempuri.org\")] attribute.");
            }

            if (attr.Uri.IsAbsoluteUri)
            {
                return attr.Uri;
            }

            if (string.IsNullOrWhiteSpace(WebRoot))
            {
                throw new Exception("You need to configure a WebRoot to use relative Uris");
            }

            return new Uri(new Uri(WebRoot), attr.Uri);
        }

        public void Dispose()
        {
            Driver.Dispose();
        }
    }
}