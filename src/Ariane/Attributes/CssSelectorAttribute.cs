using System;

namespace Ariane.Attributes
{
    public class CssSelectorAttribute : Attribute
    {
        public string Selector { get; set; }

        public CssSelectorAttribute(string selector = "")
        {
            Selector = selector;
        }

        public override string ToString()
        {
            return Selector;
        }
    }
}