﻿using System;
using System.Reflection;
using Passenger.Attributes;

namespace Passenger.PageObjectInspections.UrlDiscovery
{
    public class DefaultUrlDiscoveryStrategy : IDiscoverUrls
    {
        public Uri UrlFor(object classProxy, PassengerConfiguration configuration)
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

            if (string.IsNullOrWhiteSpace(configuration.WebRoot))
            {
                throw new Exception("You need to configure a WebRoot to use relative Uris");
            }

            return new Uri(new Uri(configuration.WebRoot), attr.Uri);
        }
    }
}