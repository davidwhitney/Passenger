using System;
using System.Collections;
using System.Collections.Generic;
using Castle.DynamicProxy;
using OpenQA.Selenium;
using Passenger.Attributes;
using Passenger.CommandHandlers;

namespace Passenger.ModelInterception
{
    public class ReturnValuePostProcessor
    {
        private readonly PassengerConfiguration _configuration;

        public ReturnValuePostProcessor(PassengerConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void PostProcessReturnValue(IInvocation invocation)
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

        public object PostProcessReturnValue(object current, PropertySelectionContext ctx, PassengerConfiguration configuration)
        {
            if (current == null)
            {
                return null;
            }

            if (!ctx.IsPageComponent)
            {
                if (ctx.RawSelectedElement.IsAWebElement()
                    && ctx.TargetProperty.PropertyType.IsAPassengerElement())
                {
                    current = TypeMapping.WrapIntoPassengerElement(ctx.RawSelectedElement, ctx.TargetProperty.PropertyType);
                }

                if (ctx.RawSelectedElement.IsCollection()
                    && ctx.TargetProperty.PropertyType.IsCollection())
                {
                    current = TypeMapping.BuildCollectionOfWrappers(ctx.RawSelectedElement, ctx.TargetProperty.PropertyType, _configuration);
                }
            }

            if (current.IsAPassengerElement())
            {
                ((IPassengerElement)current).Inner = ctx.RawSelectedElement as IWebElement;
            }

            if (current.IsCollection())
            {
                
            }

            return current;
        }
    }
}