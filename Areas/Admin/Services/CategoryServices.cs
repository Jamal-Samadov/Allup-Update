using Allup.Areas.Admin.Models;
using Allup.DAL;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Allup.Areas.Admin.Services
{
    public class CategoryServices
    {
        public readonly AppDbContext _dbContext;

        public CategoryServices(AppDbContext dbContex)
        {
            _dbContext = dbContex;
        }

        public async Task<ProductCreateViewModel> GetCategory()
        {
            var categories = await _dbContext
               .Categories
               .Where(x => !x.IsDeleted && x.IsMain)
               .Include(x => x.Children).ToListAsync();

            var parentCategoriesSelectListItem = new List<SelectListItem>();
            var childCategoriesSelectListItem = new List<SelectListItem>();

            categories.ForEach(x => parentCategoriesSelectListItem.Add(new SelectListItem(x.Name, x.Id.ToString())));
            categories[0].Children.ToList().ForEach(x => childCategoriesSelectListItem.Add(new SelectListItem(x.Name, x.Id.ToString())));

            var model = new ProductCreateViewModel
            {
                ParentCatigories = parentCategoriesSelectListItem,
                ChildCatigories = childCategoriesSelectListItem
            };

            return model;
        }

    }
}
