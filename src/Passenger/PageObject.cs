using System;
using System.Linq;
using Passenger.ModelInterception;
using Passenger.PageObjectInspections.UrlDiscovery;

namespace Passenger
{
    public class PageObject
    {
        public PassengerConfiguration Configuration { get; set; }
        public object CurrentProxy { get; set; }

        public TNextPageObjectType GoTo<TNextPageObjectType>() where TNextPageObjectType : class
        {
            CurrentProxy = CreateOrReturnProxy<TNextPageObjectType>();
            Configuration.Driver.NavigateTo(UrlFor(CurrentProxy).Url);
            return (TNextPageObjectType)CurrentProxy;
        }

        public TCurrentPageObjectType Page<TCurrentPageObjectType>() where TCurrentPageObjectType : class
        {
            return CreateOrReturnProxy<TCurrentPageObjectType>();
        }

        public void VerifyRedirectionTo<TNextPageObjectType>() where TNextPageObjectType : class
        {
            var nextPage = CreateOrReturnProxy<TNextPageObjectType>();
            var expectedUrl = UrlFor(nextPage);

            var verifier = Configuration.UrlVerificationStrategies.StrategyFor(expectedUrl);
            if (!verifier.UrlMatches(Configuration.Driver.Url, expectedUrl))
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

        private DiscoveredUrl UrlFor(object classProxy)
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