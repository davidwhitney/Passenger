using Passenger.ModelInterception;
using static Passenger.ModelInterception.ProxyGenerator;

namespace Passenger
{
    public static class Arrives
    {
        public static TPageObjectType At<TPageObjectType>(string rebaseOn = null)
            where TPageObjectType : class, new()
        {
            var proxy = GenerateNavigationProxy<TPageObjectType>();

            if (proxy.GetType().IsAPageTransitionObject())
            {
                proxy.SetRebaseOn(rebaseOn);
            }

            return proxy;
        }

        public static PageTransitionObject<TPageObjectType> AtPageObject<TPageObjectType>(string rebaseOn = null)
            where TPageObjectType : class
        {
            return At<PageTransitionObject<TPageObjectType>>(rebaseOn);
        }
    }
}