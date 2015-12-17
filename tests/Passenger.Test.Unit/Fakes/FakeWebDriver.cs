using System;
using System.Collections.Generic;
using Passenger.Drivers;

namespace Passenger.Test.Unit.Fakes
{
    public class FakeWebDriver : IDriverBindings
    {
        public FakeWebDriver()
        {
            Substitutes = new List<DriverBindings.TypeSubstitution>();
            NavigationHandlers = new List<DriverBindings.IHandle>();
        }

        public string UrlBacking { get; set; }
        public string Url
        {
            get { return UrlBacking; }
        }

        public IList<DriverBindings.IHandle> NavigationHandlers { get; private set; }
        public IList<DriverBindings.TypeSubstitution> Substitutes { get; private set; }

        public void NavigateTo(Uri url)
        {
            UrlBacking = url.ToString();
        }

        public void Dispose()
        {
        }
    }
}