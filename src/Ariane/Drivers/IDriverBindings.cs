using System;
using System.Collections.Generic;

namespace Ariane.Drivers
{
    public interface IDriverBindings
    {
        string Url { get; }
        IEnumerable<DriverBindings.IHandle> NavigationHandlers { get; }
        void NavigateTo(Uri url);
        void Dispose();
    }
}