namespace Ariane.Attributes
{
    public class NameAttribute : BaseAttribute
    {
        public string Name { get; set; }

        public NameAttribute(string name)
            : base(name)
        {
            Name = name;
        }
    }
}