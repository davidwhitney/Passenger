using System;
using System.Linq;
using System.Reflection;
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
            var property = GetAutoProp(invocation);
            
            if (property == null)
            {
                invocation.Proceed();
                return;
            }

            var attrInstance = property.GetCustomAttribute<ByIdAttribute>();
            if (attrInstance != null)
            {
                invocation.ReturnValue = _driver.FindElementById(attrInstance.Id);
            }
        }

        private static PropertyInfo GetAutoProp(IInvocation invocation)
        {
            if (!invocation.Method.Name.StartsWith("get_") && !invocation.Method.Name.StartsWith("set_"))
            {
                return null;
            }

            var declaringType = invocation.Method.DeclaringType;
            var propertyName = invocation.Method.Name.Remove(0, 4);
            return declaringType.GetProperty(propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
    }

    public class ByIdAttribute : Attribute
    {
        public string Id { get; set; }

        public ByIdAttribute(string id)
        {
            Id = id;
        }
    }
}
