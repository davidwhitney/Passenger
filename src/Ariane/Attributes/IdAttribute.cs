namespace Ariane.Attributes
{
    public class IdAttribute : BaseAttribute
    {
        public string Id { get; set; }

        public IdAttribute(string id) : base(id)
        {
            Id = id;
        }
    }
}