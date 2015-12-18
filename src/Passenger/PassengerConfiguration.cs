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

        public PageObject<TPageObjectType> StartTestAt<TPageObjectType>() where TPageObjectType : class
        {
            if (Driver == null)
            {
                throw new ArgumentException("Must configure a driver.");
            }

            return new PageObject<TPageObjectType>(this);
        }
    }
}
