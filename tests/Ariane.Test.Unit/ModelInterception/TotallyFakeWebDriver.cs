using System;
using System.Collections.Generic;
using Ariane.Drivers;

namespace Ariane.Test.Unit.ModelInterception
{
    public class TotallyFakeWebDriver : IDriverBindings
    {
        public TotallyFakeWebDriver()
        {
            Substitutes = new List<DriverBindings.TypeSubstitution>();
            NavigationHandlers = new List<DriverBindings.IHandle>();
        }

        public string Url
        {
            get { throw new NotImplementedException(); }
        }

        public IList<DriverBindings.IHandle> NavigationHandlers { get; private set; }
        public IList<DriverBindings.TypeSubstitution> Substitutes { get; private set; }

        public void NavigateTo(Uri url)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}