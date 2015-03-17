using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace Ariane
{
    public class PageObjectFactory
    {
        public TPageObjectType GetPage<TPageObjectType>() where TPageObjectType : class
        {
            var generator = new ProxyGenerator();
            var proxy = generator.CreateClassProxy<TPageObjectType>(new PageObjectProxy());

            return proxy;
        }
    }
}
