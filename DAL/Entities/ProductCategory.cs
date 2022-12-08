namespace Allup.DAL.Entities
{
    public class ProductCategory : Entity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int CategoryId { get; set; } 
        public Category Category { get; set; }
    }
}
