using System;
using System.Collections.Generic;

namespace Ariane.Drivers
{
    public interface IDriverBindings
    {
        string Url { get; }
        IList<DriverBindings.IHandle> NavigationHandlers { get; }
        IList<DriverBindings.TypeSubstitution> Substitutes { get; } 
        void NavigateTo(Uri url);
        void Dispose();
    }
}