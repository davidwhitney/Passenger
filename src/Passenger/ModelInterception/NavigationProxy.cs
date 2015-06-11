using Castle.DynamicProxy;

namespace Passenger.ModelInterception
{
    /// <summary>
    /// Marker type for post processing return values
    /// </summary>
    public class NavigationProxy : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
        }
    }
}
