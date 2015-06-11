using System;
using System.Reflection;
using Passenger.Attributes;
using Passenger.Drivers;
using Passenger.ModelInterception;

namespace Passenger
{
    public class PageObject
    {
        public IDriverBindings Driver { get; set; }
        public string WebRoot { get; set; }
        public object CurrentProxy { get; set; }
    }
    
    public class PageObject<TPageObjectType> 
        : PageObject, IDisposable where TPageObjectType : class
    {
        public PageObject()
        {
        }

        public PageObject(IDriverBindings driver, TPageObjectType currentProxy)
        {
            Driver = driver;
            CurrentProxy = currentProxy;
        }

        public PageObject(IDriverBindings driver, string webRoot)
        {
            Driver = driver;
            WebRoot = webRoot;

            GoTo<TPageObjectType>();
        }

        public TNextPageObjectType GoTo<TNextPageObjectType>() where TNextPageObjectType : class
        {
            CurrentProxy = CreateOrReturnProxy<TNextPageObjectType>();
            Driver.NavigateTo(UrlFor(CurrentProxy));
            return (TNextPageObjectType)CurrentProxy;
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
            if (CurrentProxy == null || CurrentProxy.GetType().BaseType != typeof (TCurrentPageObjectType))
            {
                CurrentProxy = ProxyGenerator.Generate<TCurrentPageObjectType>(Driver);
            }

            return (TCurrentPageObjectType) CurrentProxy;
        }

        private Uri UrlFor(object classProxy)
        {
            var attr = classProxy.GetType().GetCustomAttribute<UriAttribute>();

            if (attr == null)
            {
                throw new Exception("Cannot navigate to a PageObject Object that doesn't have a [Uri(\"http://tempuri.org\")] attribute.");
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