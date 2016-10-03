using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

            if (invocation.ReturnValue.GetType().IsAPageTransitionObject())
            {
                var rebaseUri = invocation.ReturnValue.GetRebaseOn();
                if (!string.IsNullOrWhiteSpace(rebaseUri))
                {
                    _configuration.WebRoot = rebaseUri;
                }
                invocation.ReturnValue = ProxyGenerator.GenerateWrappedPageObject(invocation.Method.ReturnType.GetGenericParam(), _configuration);
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

            var snapshotOfCollection = new List<object>();
            if (current.IsCollection())
            {
                snapshotOfCollection.AddRange(((IEnumerable)current).Cast<object>());
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
                MapSnapshotToInnerProperty(current, snapshotOfCollection);
            }

            return current;
        }

        private static void MapSnapshotToInnerProperty(object current, IReadOnlyList<object> snapshotOfCollection)
        {
            var en = (IEnumerable) current;
            var i = 0;
            foreach (var item in en)
            {
                if (item.IsAPassengerElement())
                {
                    var wrapper = (IPassengerElement) item;
                    wrapper.Inner = (IWebElement) snapshotOfCollection[i];
                }
                i++;
            }
        }
    }
}