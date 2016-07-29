using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;

namespace Passenger.ModelInterception
{
    public class PropertySelectionContext
    {
        private Lazy<List<Attribute>> LazyAttributes
            => new Lazy<List<Attribute>>(() => (TargetProperty.GetCustomAttributes() ?? new List<Attribute>()).ToList());

        public IInvocation Invocation { get; set; }
        public PropertyInfo TargetProperty { get; set; }
        public List<Attribute> Attributes => LazyAttributes.Value;
        public Attribute FirstAttribute => Attributes.FirstOrDefault();
        public bool IsPageComponent => TargetProperty.IsPageComponent();
        public bool IsPropertySetter => Invocation.Method.IsSetProperty();

        public object RawSelectedElement { get; set; }

        public PropertySelectionContext()
        {
        }

        public PropertySelectionContext(IInvocation invocation)
        {
            Invocation = invocation;
            TargetProperty = invocation.Method.ToPropertyInfo();
        }
    }
}