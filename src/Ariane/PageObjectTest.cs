using System;
using OpenQA.Selenium.Remote;

namespace Ariane
{
    public class PageObjectTest
    {
        public Func<RemoteWebDriver> WithDriver { get; set; }
        public Func<string> WebRoot { get; set; }

        public PageObjectTestContext<TPageObjectType> StartAt<TPageObjectType>() where TPageObjectType : class
        {
            var driver = WithDriver();
            return new PageObjectTestContext<TPageObjectType>(driver, WebRoot());
        }
    }
}
