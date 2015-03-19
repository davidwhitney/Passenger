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

            var attributes = (property.GetCustomAttributes() ?? new List<Attribute>()).ToList();
            if (!attributes.Any())
            {
                invocation.Proceed();
                return;
            }

            if (attributes.Count > 1)
            {
                throw new Exception("Only one selection attribute is valid per property.");
            }

            if (invocation.IsSetProperty())
            {
                throw new Exception("You can't set a property that has a selection attribute.");
            }

            var handler = new NavigationAttributeHandler(attributes.First(), _driver);
            var selectionHandlerResult = handler.InvokeSeleniumSelection(property);
            if (selectionHandlerResult == null)
            {
                invocation.Proceed();
                return;
            }

            invocation.ReturnValue = selectionHandlerResult;
        }
    }
}
