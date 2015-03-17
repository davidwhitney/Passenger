using System;

namespace Ariane
{
    public abstract class BaseAttribute : Attribute
    {
        public string StringMatcher { get; set; }

        protected BaseAttribute(string stringMatcher)
        {
            StringMatcher = stringMatcher;
        }
    }

    public class IdAttribute : BaseAttribute
    {
        public string Id { get; set; }

        public IdAttribute(string id) : base(id)
        {
            Id = id;
        }
    }

    public class NameAttribute : BaseAttribute
    {
        public string Name { get; set; }

        public NameAttribute(string name)
            : base(name)
        {
            Name = name;
        }
    }

    public class CssSelectorAttribute : BaseAttribute
    {
        public string Selector { get; set; }

        public CssSelectorAttribute(string selector)
            : base(selector)
        {
            Selector = selector;
        }
    }

    public class UriAttribute : Attribute
    {
        public Uri Uri { get; set; }

        public UriAttribute(Uri uri)
        {
            Uri = uri;
        }

        public UriAttribute(string uri) : this(new Uri(uri))
        {
        }
    }
}