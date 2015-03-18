using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ariane.CommandHandlers;
using OpenQA.Selenium.Remote;

namespace Ariane
{
    public class NavigationHandlerRegistry
    {
        private readonly List<Type> _navigationHandlers;

        public NavigationHandlerRegistry()
        {
            _navigationHandlers =
                Assembly.GetCallingAssembly()
                    .GetTypes()
                    .Where(x => x.GetInterfaces().Any(i => i.Name == "IHandleNavigation"))
                    .ToList();
        }

        public IHandleNavigation HandlerFor(Attribute attr, RemoteWebDriver driver)
        {
            var handler =_navigationHandlers.SingleOrDefault(
                x => x.GetConstructors().First().GetParameters().First().ParameterType == attr.GetType());

            if (handler == null)
            {
                return null;
            }

            return (IHandleNavigation)Activator.CreateInstance(handler, attr, driver);
        }
    }
}