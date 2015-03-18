using System;
using System.Collections.Generic;
using System.Reflection;
using Ariane.Attributes;
using Castle.DynamicProxy;
using OpenQA.Selenium.Remote;

namespace Ariane
{
    public class PageObjectProxy : IInterceptor
    {
        private readonly RemoteWebDriver _driver;

        private readonly Dictionary<Type, Func<RemoteWebDriver, string, object>> _driverMapping = new Dictionary<Type, Func<RemoteWebDriver, string, object>>
        {
            {typeof (IdAttribute), (d,s) => d.FindElementById(s)},
            {typeof (NameAttribute), (d,s) => d.FindElementByName(s)},
            {typeof (CssSelectorAttribute), (d,s) => d.FindElementByCssSelector(s)},
        };

        public PageObjectProxy(RemoteWebDriver driver)
        {
            _driver = driver;
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

            ExecuteSeleniumMatchers(invocation, property);
        }

        private void ExecuteSeleniumMatchers(IInvocation invocation, MemberInfo property)
        {
            foreach (var mapping in _driverMapping)
            {
                var attr = property.GetCustomAttribute(mapping.Key);
                if (attr != null)
                {
                    var @base = (BaseAttribute) attr;
                    invocation.ReturnValue = mapping.Value(_driver, @base.StringMatcher);
                }
            }
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
