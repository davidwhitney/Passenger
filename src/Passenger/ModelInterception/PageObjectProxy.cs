using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Passenger.CommandHandlers;
using Passenger.Drivers;
using Castle.DynamicProxy;

namespace Passenger.ModelInterception
{
    public class PageObjectProxy : IInterceptor
    {
        private readonly IDriverBindings _driver;
        private readonly TypeSubstitutionHandler _typeSubstitution;
        private readonly ElementSelectionHandler _elementSelectionHandler;

        public PageObjectProxy(IDriverBindings driver)
        {
            _driver = driver;
            _typeSubstitution = new TypeSubstitutionHandler(driver);
            _elementSelectionHandler = new ElementSelectionHandler(driver);
        }

        public void Intercept(IInvocation invocation)
        {
            if (!invocation.Method.IsProperty())
            {
                invocation.Proceed();
                return;
            }

            var property = invocation.Method.ToPropertyInfo();
            var attributes = (property.GetCustomAttributes() ?? new List<Attribute>()).ToList();
            var firstAttribute = attributes.FirstOrDefault();

            var result = new InvocationSwitchboard
            {
                {() => _typeSubstitution.FindSubstituteFor(property.PropertyType) != null, () => InvocationResult.Assign(_typeSubstitution.FindSubstituteFor(property.PropertyType).GetInstance())},
                {() => property.IsPageComponent(), () => InvocationResult.Assign(GenerateComponentProxy(property))},
                {() => !attributes.Any(), () => InvocationResult.Proceed},
                {() => attributes.Count > 1, () => { throw new Exception("Only one selection attribute is valid per property."); }},
                {() => invocation.Method.IsSetProperty(), () => { throw new Exception("You can't set a property that has a selection attribute."); }},
                {() => _elementSelectionHandler.SelectElement(firstAttribute, property) == null, () => InvocationResult.Proceed },
                {
                    () => _elementSelectionHandler.SelectElement(firstAttribute, property) != null,
                    () => InvocationResult.Assign(_elementSelectionHandler.SelectElement(attributes.First(), property))
                },
            }.Route();

            Invoke(invocation, result);
        }

        private static void Invoke(IInvocation invocation, InvocationResult result)
        {
            if (result == InvocationResult.Proceed)
            {
                invocation.Proceed();
                return;
            }

            invocation.ReturnValue = result.Value;
        }

        private object GenerateComponentProxy(PropertyInfo property)
        {
            return PageObjectProxyGenerator.Generate(property.PropertyType, _driver);
        }

        private class InvocationSwitchboard : Dictionary<Func<bool>, Func<InvocationResult>>
        {
            public InvocationResult Route()
            {
                foreach (var rule in this.Where(rule => rule.Key()))
                {
                    return rule.Value();
                }
                return InvocationResult.Proceed;
            }
        }

        public class InvocationResult
        {
            public object Value { get; set; }
            public static InvocationResult Proceed { get { return ProceedBacking; } }
            private static readonly InvocationResult ProceedBacking = new InvocationResult();
            public static InvocationResult Assign(object result) { return new InvocationResult {Value = result};}
        }
    }
}
