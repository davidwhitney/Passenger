using System;
using System.Text.RegularExpressions;

namespace Passenger.Attributes
{
    public class UriAttribute : Attribute
    {
        public Uri Uri { get; set; }
        public string VerificationPattern { get; set; }

        public Regex VerificationRegex
        {
            get
            {
                if (string.IsNullOrWhiteSpace(VerificationPattern))
                {
                    throw new Exception("Invalid verification pattern.");
                }

                return new Regex(VerificationPattern);
            }
        }

        /// <param name="relativeOrAbsoluteUri">Relative Uris require a WebRoot to be configured.</param>
        /// <param name="verificationPattern">Optional regular expression for stricter url verification.</param>
        public UriAttribute(string relativeOrAbsoluteUri, string verificationPattern = null)
        {
            Uri = relativeOrAbsoluteUri.StartsWith("http")
                ? new Uri(relativeOrAbsoluteUri, UriKind.Absolute)
                : new Uri(relativeOrAbsoluteUri, UriKind.Relative);

            VerificationPattern = verificationPattern;
        }
    }
}