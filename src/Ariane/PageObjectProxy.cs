using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using OpenQA.Selenium.Remote;

namespace Ariane
{
    public class PageObjectProxy : IInterceptor
    {
        private readonly RemoteWebDriver _driver;
        private readonly NavigationHandlerRegistry _handlerRegistry;

        public PageObjectProxy(RemoteWebDriver driver, NavigationHandlerRegistry handlerRegistry)
        {
            _driver = driver;
            _handlerRegistry = handlerRegistry;
        }

        public void Intercept(IInvocation invocation)
        {
            if (!invocation.IsProperty())
            {
                invocation.Proceed();
                return;
            }

            var property = invocation.ToPropertyInfo();
            if (property == null)
            {
                invocation.Proceed();
                return;
            }

            var attributes = (property.GetCustomAttributes() ?? new List<Attribute>()).ToList();
            if (!attributes.Any())
            {
                invocation.Proceed();
                return;
            }

            var handler = attributes.Select(attr => _handlerRegistry.HandlerFor(attr, _driver)).SingleOrDefault(h => h != null);
            if (handler == null)
            {
                invocation.Proceed();
                return;
            }
            
            invocation.ReturnValue = handler.InvokeSeleniumSelection(property);
        }
    }
}
