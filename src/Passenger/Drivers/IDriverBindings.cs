using System;
using System.Collections.Generic;

namespace Passenger.Drivers
{
    public interface IDriverBindings
    {
        string Url { get; }
        void NavigateTo(Uri url);
        void Dispose();

        /// <summary>
        /// A list of Func's that handle the binding of navigation attributes to driver commands
        /// </summary>
        IList<DriverBindings.IHandle> NavigationHandlers { get; }

        /// <summary>
        /// Substitutes are a list of Type=>Func[object].
        /// The Func[object] will be called and its return value assigned to the Type whenever that type appears as a public virtual property on a page object.
        /// This is primarily used to "automagically" wire up IWebDriver and WebDriverRemote instances for use inside the page objects.
        /// </summary>
        IList<DriverBindings.TypeSubstitution> Substitutes { get; }
    }
}