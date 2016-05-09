using System;
using Passenger.Drivers;
using Passenger.PageObjectInspections.UrlDiscovery;
using Passenger.PageObjectInspections.UrlVerification;

namespace Passenger
{
    public class PassengerConfiguration
    {
        public IDriverBindings Driver { get; set; }
        public string WebRoot { get; set; }

        public UrlVerificationStrategyCollection UrlVerificationStrategies { get; set; }
        public IDiscoverUrls UrlDiscoveryStrategy { get; set; }

        public PassengerConfiguration()
        {
            UrlVerificationStrategies = new UrlVerificationStrategyCollection
            {
                new RegexStrategy(),
                new StringContainingStrategy(),
            };

            UrlDiscoveryStrategy = new DefaultUrlDiscoveryStrategy();
        }

        /// <param name="uri">Override any Attribute provided Uri for the start page</param>
        public PageObject<TPageObjectType> StartTestAt<TPageObjectType>(Uri uri = null) where TPageObjectType : class
        {
            if (Driver == null)
            {
                throw new ArgumentException("Must configure a driver.");
            }

            uri = uri.EnsureFullyQualifiedUri(this);
            return new PageObject<TPageObjectType>(this, uri);
        }

    }
}
