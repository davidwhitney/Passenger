using Passenger.ModelInterception;

namespace Passenger
{
    public class Arrives
    {
        public static TPageObjectType At<TPageObjectType>()
            where TPageObjectType : class, new()
        {
            return ProxyGenerator.GenerateNavigationProxy<TPageObjectType>();
        }

        public static PageObject<TPageObjectType> AtPageObject<TPageObjectType>()
            where TPageObjectType : class
        {
            return At<PageObject<TPageObjectType>>();
        }
    }
}