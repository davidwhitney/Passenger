using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ariane.CommandHandlers;
using Castle.DynamicProxy;
using OpenQA.Selenium.Remote;

namespace Ariane
{
    public class PageObjectProxy : IInterceptor
    {
        private readonly RemoteWebDriver _driver;

        public PageObjectProxy(RemoteWebDriver driver)
        {
            _driver = driver;
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

            var handler = attributes.Select(attr => new NavigationAttributeHandler(attr, _driver)).SingleOrDefault(h => h != null);
            if (handler == null)
            {
                invocation.Proceed();
                return;
            }

            var selectionHandlerResult = handler.InvokeSeleniumSelection(property);
            if (selectionHandlerResult == null)
            {
                invocation.Proceed();
                return;
            }

            invocation.ReturnValue = handler.InvokeSeleniumSelection(property);
        }
    }
}
