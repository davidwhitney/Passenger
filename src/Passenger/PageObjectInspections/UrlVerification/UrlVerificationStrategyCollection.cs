using System.Collections.Generic;
using System.Linq;
using Passenger.PageObjectInspections.UrlDiscovery;

namespace Passenger.PageObjectInspections.UrlVerification
{
    public class UrlVerificationStrategyCollection : List<IVerifyUrls>
    {
        public IVerifyUrls StrategyFor(DiscoveredUrl discoveredUrl)
        {
            return this.First(x => x.Supports(discoveredUrl));
        }
    }
}