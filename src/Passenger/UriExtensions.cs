using System;

namespace Passenger
{
    public static class UriExtensions
    {
        public static Uri EnsureFullyQualifiedUri(this Uri uri, PassengerConfiguration config)
        {
            if (uri == null)
            {
                return null;
            }

            if (uri.IsAbsoluteUri)
            {
                return uri;
            }

            if (string.IsNullOrWhiteSpace(config.WebRoot))
            {
                throw new Exception("You need to configure a WebRoot to use relative Uris");
            }

            return new Uri(new Uri(config.WebRoot), uri);
        }
    }
}