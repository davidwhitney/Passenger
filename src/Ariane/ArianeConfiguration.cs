using System;
using Ariane.Drivers;

namespace Ariane
{
    public class ArianeConfiguration
    {
        public Func<DriverBindings> Driver { get; set; }
        public Func<string> WebRoot { get; set; }

        public PageObjectTestContext<TPageObjectType> StartTestAt<TPageObjectType>() where TPageObjectType : class
        {
            var driver = Driver();
            return new PageObjectTestContext<TPageObjectType>(driver, WebRoot());
        }
    }
}
