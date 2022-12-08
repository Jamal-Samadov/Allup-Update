using Microsoft.AspNetCore.Mvc.Rendering;

namespace Allup.Areas.Admin.Models
{
    public class CategoryUpdateModel
    {

        public int Id { get; set; }
        public string? Description { get; set; }
        public int? Row { get; set; }
        public string Name { get; set; }
        public bool IsMain { get; set; }
        public IFormFile? Image { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int ParentId { get; set; }
        public List<SelectListItem> ParentCategories { get; set; } = new();
    }
}
