using Allup.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Allup.ViewComponents
{
    public class FeaturedCatigoryViewComponent : ViewComponent
    {
        private readonly AppDbContext _dbContext;

        public FeaturedCatigoryViewComponent(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var catigories = await _dbContext.Categories
                .Where(c => c.IsMain && !c.IsDeleted)
                .ToListAsync();

            return View(catigories);
        }
    }
}
