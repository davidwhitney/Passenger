namespace Ariane.Attributes
{
    public class XPathAttribute : NavigationAttributeBase { public XPathAttribute(string xpathQuery = "") : base(xpathQuery) { } }
    public class TagNameAttribute : NavigationAttributeBase { public TagNameAttribute(string domTag = "") : base(domTag) { } }
    public class ClassNameAttribute : NavigationAttributeBase { public ClassNameAttribute(string elementClass = "") : base(elementClass) { } }
    public class NameAttribute : NavigationAttributeBase { public NameAttribute(string elementName = "") : base(elementName) { } }
    public class IdAttribute : NavigationAttributeBase { public IdAttribute(string elementId = "") : base(elementId) { } }
    public class CssSelectorAttribute : NavigationAttributeBase { public CssSelectorAttribute(string selector = "") : base(selector) { } }
    public class LinkTextAttribute : NavigationAttributeBase { public LinkTextAttribute(string elementText = "") : base(elementText) { } }
    public class PartialLinkTextAttribute : NavigationAttributeBase { public PartialLinkTextAttribute(string elementText = "") : base(elementText) { } }
}