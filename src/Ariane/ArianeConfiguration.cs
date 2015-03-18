using System;
using OpenQA.Selenium.Remote;

namespace Ariane
{
    public class ArianeConfiguration
    {
        public Func<RemoteWebDriver> Driver { get; set; }
        public Func<string> WebRoot { get; set; }

        public PageObjectTestContext<TPageObjectType> StartTestAt<TPageObjectType>() where TPageObjectType : class
        {
            var driver = Driver();
            return new PageObjectTestContext<TPageObjectType>(driver, WebRoot());
        }
    }
}
