using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Passenger.CommandHandlers;
using Castle.DynamicProxy;

namespace Passenger.ModelInterception
{
    public class PageObjectProxy : IInterceptor
    {
        private readonly PassengerConfiguration _configuration;
        private readonly TypeSubstitutionHandler _typeSubstitution;
        private readonly ElementSelectionHandler _elementSelectionHandler;

        public PageObjectProxy(PassengerConfiguration configuration)
        {
            _configuration = configuration;
            _typeSubstitution = new TypeSubstitutionHandler(configuration);
            _elementSelectionHandler = new ElementSelectionHandler(configuration);
        }

        public void Intercept(IInvocation invocation)
        {
            if (!invocation.Method.IsProperty())
            {
                invocation.Proceed();
                PostProcessReturnValue(invocation);
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
            PostProcessReturnValue(invocation);
        }

        private void PostProcessReturnValue(IInvocation invocation)
        {
            if (invocation.ReturnValue == null
                || !invocation.ReturnValue.IsAProxy())
            {
                return;
            }

            invocation.ReturnValue = invocation.Method.ReturnType.IsAPageObject()
                ? ProxyGenerator.GenerateWrappedPageObject(invocation.Method.ReturnType.GetGenericParam(), _configuration)
                : ProxyGenerator.Generate(invocation.Method.ReturnType, _configuration);
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
            return ProxyGenerator.Generate(property.PropertyType, _configuration);
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
