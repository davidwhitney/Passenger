using System;
using System.Reflection;
using Passenger.Attributes;
using Passenger.ModelInterception;

namespace Passenger
{
    public class PageObject
    {
        public PassengerConfiguration Configuration { get; set; }
        public object CurrentProxy { get; set; }
    }
    
    public class PageObject<TPageObjectType> 
        : PageObject, IDisposable where TPageObjectType : class
    {
        public PageObject()
        {
        }

        public PageObject(PassengerConfiguration configuration, TPageObjectType currentProxy)
        {
            Configuration = configuration;
            CurrentProxy = currentProxy;
        }

        public PageObject(PassengerConfiguration configuration)
        {
            Configuration = configuration;

            GoTo<TPageObjectType>();
        }

        public TNextPageObjectType GoTo<TNextPageObjectType>() where TNextPageObjectType : class
        {
            CurrentProxy = CreateOrReturnProxy<TNextPageObjectType>();
            Configuration.Driver.NavigateTo(UrlFor(CurrentProxy));
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
            if (!Configuration.Driver.Url.Contains(urlOfNextPage.PathAndQuery))
            {
                throw new Exception("We're not where we should be.");
            }
        }

        private TCurrentPageObjectType CreateOrReturnProxy<TCurrentPageObjectType>() where TCurrentPageObjectType : class
        {
            if (CurrentProxy == null || CurrentProxy.GetType().BaseType != typeof (TCurrentPageObjectType))
            {
                CurrentProxy = ProxyGenerator.Generate<TCurrentPageObjectType>(Configuration);
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

            if (string.IsNullOrWhiteSpace(Configuration.WebRoot))
            {
                throw new Exception("You need to configure a WebRoot to use relative Uris");
            }

            return new Uri(new Uri(Configuration.WebRoot), attr.Uri);
        }

        public void Dispose()
        {
            Configuration.Driver.Dispose();
        }
    }
}