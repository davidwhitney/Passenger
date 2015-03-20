namespace Ariane.Attributes
{
    public class XPathAttribute : NavigationAttributeBase { public XPathAttribute(string xpathQuery = "") : base(xpathQuery) { } }
    public class TagNameAttribute : NavigationAttributeBase { public TagNameAttribute(string domTag = "") : base(domTag) { } }
    public class NameAttribute : NavigationAttributeBase { public NameAttribute(string elementName = "") : base(elementName) { } }
    public class IdAttribute : NavigationAttributeBase { public IdAttribute(string elementId = "") : base(elementId) { } }
    public class CssSelectorAttribute : NavigationAttributeBase { public CssSelectorAttribute(string selector = "") : base(selector) { } }
    public class TextAttribute : NavigationAttributeBase { public TextAttribute(string elementText = "") : base(elementText) { } }
}