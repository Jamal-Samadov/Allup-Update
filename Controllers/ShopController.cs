using Allup.DAL;
using Allup.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Allup.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _dbContext;

        public ShopController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            var mainCatigories = await _dbContext.Categories
                .Where(c => !c.IsDeleted && c.IsMain)
                .Include(c => c.Children.Where(c => !c.IsDeleted))
                .ToListAsync();

            var selectedCategory = mainCatigories.FirstOrDefault();

            if (categoryId != null)
            {
                selectedCategory = mainCatigories.FirstOrDefault(c => c.Id == categoryId);
                selectedCategory??= mainCatigories.SelectMany(c=>c.Children).FirstOrDefault(c => c.Id == categoryId);

            }

            var model = new ShopViewModel
            {
                SelectedCategory = selectedCategory,
                Categories = mainCatigories,
            };

            return View(model);
        }
    }
}
