using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace Passenger.ModelInterception
{
    public class NavigationProxy : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
        }
    }
}
