using System;

namespace Passenger.Attributes
{
    public abstract class NavigationAttributeBase : Attribute
    {
        public string Key { get; set; }

        protected NavigationAttributeBase(string key)
        {
            Key = key;
        }

        public override string ToString()
        {
            return Key;
        }
    }
}