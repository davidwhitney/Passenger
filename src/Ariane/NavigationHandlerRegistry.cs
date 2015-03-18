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
        public IHandleNavigation HandlerFor(Attribute attr, RemoteWebDriver driver)
        {
            return new NavigationAttributeHandler(attr, driver);
        }
    }
}