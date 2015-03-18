using System;

namespace Ariane.Attributes
{
    public class IdAttribute : Attribute
    {
        public string Id { get; set; }

        public IdAttribute(string id)
        {
            Id = id;
        }
    }
}