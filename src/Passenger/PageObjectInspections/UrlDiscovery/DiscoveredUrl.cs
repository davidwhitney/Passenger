using System;
using Passenger.Attributes;

namespace Passenger.PageObjectInspections.UrlDiscovery
{
    public class DiscoveredUrl
    {
        public Uri Url { get; set; }
        public UriAttribute SourceAttribute { get; set; }

        public DiscoveredUrl(Uri url, UriAttribute sourceAttribute)
        {
            Url = url;
            SourceAttribute = sourceAttribute;
        }
    }
}