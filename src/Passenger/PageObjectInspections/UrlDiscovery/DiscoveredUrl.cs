using System;
using Passenger.Attributes;

namespace Passenger.PageObjectInspections.UrlDiscovery
{
    public class DiscoveredUrl
    {
        public Uri Url { get; set; }
        public UriAttribute SourceAttribute { get; set; }
    }
}