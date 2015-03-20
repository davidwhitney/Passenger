using System;

namespace Ariane.Attributes
{
    public class TextAttribute : Attribute
    {
        public string String { get; set; }

        public TextAttribute(string @string = "")
        {
            String = @string;
        }

        public override string ToString()
        {
            return String;
        }
    }
}