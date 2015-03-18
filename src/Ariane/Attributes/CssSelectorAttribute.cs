namespace Ariane.Attributes
{
    public class CssSelectorAttribute : BaseAttribute
    {
        public string Selector { get; set; }

        public CssSelectorAttribute(string selector)
            : base(selector)
        {
            Selector = selector;
        }
    }
}