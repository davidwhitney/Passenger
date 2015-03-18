using System;

namespace Ariane.Attributes
{
    public abstract class BaseAttribute : Attribute
    {
        public string StringMatcher { get; set; }

        protected BaseAttribute(string stringMatcher)
        {
            StringMatcher = stringMatcher;
        }
    }
}