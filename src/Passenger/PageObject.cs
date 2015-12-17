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
            if (!Configuration.UrlVerificationStrategy.UrlMatches(Configuration.Driver.Url, urlOfNextPage.PathAndQuery))
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
            return Configuration.UrlDiscoveryStrategy.UrlFor(classProxy, Configuration);
        }

        public void Dispose()
        {
            Configuration.Driver.Dispose();
        }
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
    }
}