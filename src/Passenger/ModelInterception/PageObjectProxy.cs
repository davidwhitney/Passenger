using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Passenger.CommandHandlers;
using Castle.DynamicProxy;
using static Passenger.ModelInterception.PageObjectProxy.InvocationResult;

namespace Passenger.ModelInterception
{
    public class PageObjectProxy : IInterceptor
    {
        private readonly PassengerConfiguration _configuration;
        private readonly TypeSubstitutionHandler _typeSubstitution;
        private readonly ElementSelectionHandler _elementSelectionHandler;
        private readonly ReturnValuePostProcessor _postProcessor;

        public PageObjectProxy(PassengerConfiguration configuration)
        {
            _configuration = configuration;
            _typeSubstitution = new TypeSubstitutionHandler(configuration);
            _elementSelectionHandler = new ElementSelectionHandler(configuration);
            _postProcessor = new ReturnValuePostProcessor(configuration);
        }

        public void Intercept(IInvocation invocation)
        {
            if (!invocation.Method.IsProperty())
            {
                invocation.Proceed();
                _postProcessor.PostProcessReturnValue(invocation);
                return;
            }

            var ctx = new PropertySelectionContext(invocation);

            var result = new InvocationSwitchboard
            {
                {() => SubstituteFor(ctx) != null, () => Assign(SubstituteFor(ctx))},
                {() => !ctx.Attributes.Any() && !ctx.IsPageComponent, () => Proceed },
                {() => ctx.IsPageComponent, () => Assign(ProxyFor(ctx.TargetProperty))},
                {() => ElementFor(ctx) != null, () => Assign(ElementFor(ctx))}
            }.Route();

            EnsureAnyElementsAreSaflyLoaded(ctx);

            invocation.Assign(result);
            invocation.ReturnValue = _postProcessor.PostProcessReturnValue(invocation.ReturnValue, ctx, _configuration);
        }

        private object SubstituteFor(PropertySelectionContext ctx)
        {
            return _typeSubstitution.FindSubstituteFor(ctx.TargetProperty.PropertyType)?.GetInstance();
        }

        private void EnsureAnyElementsAreSaflyLoaded(PropertySelectionContext ctx)
        {
            try { ElementFor(ctx); } catch { /* ¯_(ツ)_/¯ */ }
        }

        private object ElementFor(PropertySelectionContext ctx)
        {
            if (ctx.RawSelectedElement != null)
            {
                return ctx.RawSelectedElement;
            }

            if (ctx.Attributes.Count > 1) throw new Exception("Only one selection attribute is valid per property.");
            if (ctx.IsPropertySetter) throw new Exception("You can't set a property that has a selection attribute.");
            return ctx.RawSelectedElement = _elementSelectionHandler.SelectElement(ctx.FirstAttribute, ctx.TargetProperty);
        }

        private object ProxyFor(PropertyInfo property)
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
                return Proceed;
            }
        }

        public class InvocationResult
        {
            public object Value { get; set; }
            public static InvocationResult Proceed { get; } = new InvocationResult();
            public static InvocationResult Assign(object result) { return new InvocationResult {Value = result};}
        }
    }

    public static class InvocationExtensions
    {
        public static void Assign(this IInvocation invocation, PageObjectProxy.InvocationResult result)
        {
            if (result == Proceed)
            {
                invocation.Proceed();
                return;
            }

            invocation.ReturnValue = result.Value;
        }
    }
}
