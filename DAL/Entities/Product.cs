namespace Allup.DAL.Entities
{
    public class Product : Entity
    {
        public string Name { get; set; }    
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Rate { get; set; }
        public int Discount { get; set; }
        public int ExTax { get; set; }
        public string Brand { get; set; }   
        public ICollection<ProductImage> ProductImages { get; set; }   
        public ICollection<ProductCategory> ProductCategories { get; set; }   

    }
}
