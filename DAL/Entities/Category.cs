using System.ComponentModel.DataAnnotations.Schema;

namespace Allup.DAL.Entities
{
    public class Category : Entity
    {
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
        public bool? IsMain { get; set; }
        public string? Description { get; set; }
        public int? Row { get; set; }
        public Category? Parent { get; set; }
        public int? ParentId { get; set; }
        public ICollection<Category>? Children { get; set; }
        public ICollection<ProductCategory>? ProductCategories { get; set; }

    }
}
