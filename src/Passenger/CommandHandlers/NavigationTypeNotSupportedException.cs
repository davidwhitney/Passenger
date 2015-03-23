using System;

namespace Passenger.CommandHandlers
{
    public class NavigationTypeNotSupportedException : NotImplementedException
    {
        public Attribute Attribute { get; set; }

        public NavigationTypeNotSupportedException(Attribute attribute, string elementName)
            : base(string.Format(@"A navigation attribute of '{0}' was found on your page object, " +
                                 "but your driver doesn't currently support this navigation method.\r\n" +
                                 "Navigating to the element identified by the property '{1}' failed.", 
                attribute.GetType().Name, elementName))
        {
            Attribute = attribute;
        }
    }
}