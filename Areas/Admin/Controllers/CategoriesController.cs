using Allup.Areas.Admin.Data;
using Allup.Areas.Admin.Models;
using Allup.DAL;
using Allup.DAL.Entities;
using Allup.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Allup.Areas.Admin.Controllers
{
    public class CategoriesController : BaseController
    {
        public readonly AppDbContext _dbContext;

        public CategoriesController(AppDbContext dbContex)
        {
            _dbContext = dbContex;
        }

        public async Task<IActionResult> Index()
        {
            List<Category> categories = await _dbContext.Categories
                .ToListAsync();

            return View(categories);
        }

        public async Task<IActionResult> Create()
        {
            var catigories = await _dbContext.Categories
                .Where(c => c.IsMain)
                .ToListAsync();

            var catigoryListItem = new List<SelectListItem>
            {
                new SelectListItem("---Select Category---", "0")
            };

            catigories.ForEach(c => catigoryListItem.Add(new SelectListItem(c.Name, c.Id.ToString())));

            var model = new CategoryCreateViewModel
            {
                ParentCategories = catigoryListItem
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateViewModel model)
        {
            var catigories = await _dbContext.Categories
                .Where(c => c.IsMain)
                .Include(c=>c.Children)
                .ToListAsync();

            var catigoryListItem = new List<SelectListItem>
            {
                new SelectListItem("---Select Category---", "0")
            };

            catigories.ForEach(c => catigoryListItem.Add(new SelectListItem(c.Name, c.Id.ToString())));

            var viewModel = new CategoryCreateViewModel
            {
                ParentCategories = catigoryListItem
            };

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var createdCategory = new Category();          
                

            if (model.IsMain)
            {
                if (!model.Image.IsImage())
                {
                    ModelState.AddModelError("", "Shekil Secmelisiz");
                    return View(viewModel); 
                }

                if (!model.Image.IsAllowedSize(10))
                {
                    ModelState.AddModelError("", "Shekilin olcusu 10 mbdan az omalidi");
                    return View(viewModel);
                }

                if (catigories.Any(c => c.Name.ToLower().Equals(model.Name.ToLower())))
                {
                    ModelState.AddModelError("", "Eyni Adda Category Artiq Bazada Var");
                    return View(viewModel);
                }

                var unicalName = await model.Image.GenerateFile(Constants.CategoryPath);
                createdCategory.ImageUrl=unicalName;
            }
            else
            {
                if (model.ParentId == 0)
                {
                    ModelState.AddModelError("", "Parent Category Secilmelidir");
                    return View(viewModel);
                }

                var parentCategory = catigories.FirstOrDefault(x => x.Id == model.ParentId);

                if (parentCategory.Children.Any(x => x.Name.ToLower().Equals(model.Name.ToLower())))
                {
                    ModelState.AddModelError("", "Bu adda alt kateqori var");
                    return View(viewModel);
                }
                createdCategory.ImageUrl = "";
                createdCategory.ParentId = model.ParentId;
            }

            createdCategory.IsMain = model.IsMain;
            createdCategory.IsDeleted = false;
            createdCategory.Name=model.Name;

            await _dbContext.AddAsync(createdCategory);
            await _dbContext.SaveChangesAsync();    

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Update(int? id)
        {
            if (id is null || id == 0) return NotFound();

            var category = await _dbContext.Categories.FindAsync(id);

            if (category == null) return NotFound();

            return View(new CategoryUpdateModel
            {
                Name = category.Name,
                Description = category.Description,
                Row = category.Row,
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, CategoryUpdateModel model)
        {
            if (id is null || id == 0) return NotFound();
            if (!ModelState.IsValid) return View();

            var category = await _dbContext.Categories.FindAsync(id);

            if (category.Id != model.Id) return BadRequest();

            if (category == null) return NotFound();


            var isExistName = await _dbContext.Categories.AnyAsync(x => x.Id == id);
            if (isExistName)
            {
                ModelState.AddModelError("Name", "Eyni adı 2 dəfə təkrar yaza bilməzsən");
                return View(model);
            }
            category.Name = model.Name;
            category.Description = model.Description;

            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
