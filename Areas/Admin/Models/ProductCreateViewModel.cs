using Microsoft.AspNetCore.Mvc.Rendering;

namespace Allup.Areas.Admin.Models
{
    public class ProductCreateViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Rate { get; set; }
        public int Discount { get; set; }
        public int ExTax { get; set; }
        public string Brand { get; set; }
        public IFormFile[] Images { get; set; }
        public List<SelectListItem>? ParentCatigories { get; set; }
        public int ParentCategoryId { get; set; }
        public List<SelectListItem>? ChildCatigories { get; set; }
        public int ChildCategoryId { get; set; }
    }
}
