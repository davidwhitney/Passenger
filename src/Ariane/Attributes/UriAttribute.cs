using System;

namespace Ariane.Attributes
{
    public class UriAttribute : Attribute
    {
        public Uri Uri { get; set; }

        /// <param name="relativeOrAbsoluteUri">Relative Uris require a WebRoot to be configured.</param>
        public UriAttribute(string relativeOrAbsoluteUri)
        {
            Uri = relativeOrAbsoluteUri.StartsWith("http")
                ? new Uri(relativeOrAbsoluteUri, UriKind.Absolute)
                : new Uri(relativeOrAbsoluteUri, UriKind.Relative);
        }
    }
}