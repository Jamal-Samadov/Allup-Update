namespace Allup.DAL.Entities
{
    public class ProductImage : Entity
    {
        public string Name { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
