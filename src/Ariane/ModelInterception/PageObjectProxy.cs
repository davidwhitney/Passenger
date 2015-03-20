using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ariane.Attributes;
using Ariane.CommandHandlers;
using Ariane.Drivers;
using Castle.DynamicProxy;

namespace Ariane.ModelInterception
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
            if (!invocation.IsProperty())
            {
                invocation.Proceed();
                return;
            }

            var result = InterceptProperty(invocation, invocation.ToPropertyInfo());
            if (result == InvocationResult.Proceed)
            {
                invocation.Proceed();
                return;
            }
         
            invocation.ReturnValue = result.Value;
        }

        public InvocationResult InterceptProperty(IInvocation invocation, PropertyInfo property)
        {
            var typeSubstitution = _typeSubstitution.FindSubstituteFor(property);
            var attributes = (property.GetCustomAttributes() ?? new List<Attribute>()).ToList();
            var firstAttribute = attributes.FirstOrDefault();

            var validations = new Dictionary<Func<bool>, Func<InvocationResult>>
            {
                {() => typeSubstitution != null, () => InvocationResult.Assign(typeSubstitution.GetInstance())},
                {() => property.IsPageComponent(), () => InvocationResult.Assign(GenerateComponentProxy(property))},
                {() => !attributes.Any(), () => InvocationResult.Proceed },
                {() => attributes.Count > 1, () => {throw new Exception("Only one selection attribute is valid per property.");} },
                {() => invocation.IsSetProperty(), () => {throw new Exception("You can't set a property that has a selection attribute.");} },
                {() => _elementSelectionHandler.SelectElement(firstAttribute, property) == null, () => InvocationResult.Proceed },
            };

            foreach (var rule in validations.Where(rule => rule.Key()))
            {
                return rule.Value();
            }

            var selectionHandlerResult = _elementSelectionHandler.SelectElement(attributes.First(), property);
            return InvocationResult.Assign(selectionHandlerResult);
        }

        private object GenerateComponentProxy(PropertyInfo property)
        {
            return PageObjectProxyGenerator.Generate(property.PropertyType, _driver);
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
