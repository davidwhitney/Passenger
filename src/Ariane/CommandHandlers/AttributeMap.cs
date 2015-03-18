using System;
using OpenQA.Selenium.Remote;

namespace Ariane.CommandHandlers
{
    public class AttributeMap
    {
        public Func<Attribute, string> GetLookupValue { get; set; }
        public Func<string, RemoteWebDriver, object> FindAllMatches { get; set; }

        public AttributeMap(Func<Attribute, string> getLookupValue, Func<string, RemoteWebDriver, object> findAllMatches)
        {
            GetLookupValue = getLookupValue;
            FindAllMatches = findAllMatches;
        }
    }
}