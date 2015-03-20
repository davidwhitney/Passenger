using System;

namespace Ariane.Attributes
{
    public class NameAttribute : Attribute
    {
        public string Name { get; set; }

        public NameAttribute(string name = "")
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}