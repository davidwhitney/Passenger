using System;

namespace Ariane.Attributes
{
    public class UriAttribute : Attribute
    {
        public Uri Uri { get; set; }

        /// <param name="uri">A relative or absolute Uri. Relative Uris require a WebRoot to be configured.</param>
        public UriAttribute(string uri)
        {
            Uri = uri.StartsWith("http")
                ? new Uri(uri, UriKind.Absolute)
                : new Uri(uri, UriKind.Relative);
        }
    }
}