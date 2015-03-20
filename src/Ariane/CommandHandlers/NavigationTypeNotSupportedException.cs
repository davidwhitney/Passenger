using System;

namespace Ariane.CommandHandlers
{
    public class NavigationTypeNotSupportedException : NotImplementedException
    {
        public Attribute Attribute { get; set; }

        public NavigationTypeNotSupportedException(Attribute attribute)
            : base(string.Format(@"A navigation attribute of '{0}' was found on your page object, " +
                                 "but your driver doesn't currently support this navigation method.\r\n" +
                                 "Navigating to the element identified as '{1}' failed.", 
                attribute.GetType().Name, attribute))
        {
            Attribute = attribute;
        }
    }
}