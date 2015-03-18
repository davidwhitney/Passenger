using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Ariane.Attributes;
using Castle.Core.Internal;
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
            if (!IsProperty(invocation))
            {
                invocation.Proceed();
                return;
            }

            var property = GetPropertyInfo(invocation);
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

        private static bool IsProperty(IInvocation invocation)
        {
            return invocation.Method.Name.StartsWith("get_") || invocation.Method.Name.StartsWith("set_");
        }
        
        private static PropertyInfo GetPropertyInfo(IInvocation invocation)
        {
            var declaringType = invocation.Method.DeclaringType;
            var propertyName = invocation.Method.Name.Remove(0, 4);
            return declaringType.GetProperty(propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
    }
}
