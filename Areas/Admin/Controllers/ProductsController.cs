using Allup.Areas.Admin.Data;
using Allup.Areas.Admin.Models;
using Allup.Areas.Admin.Services;
using Allup.DAL;
using Allup.DAL.Entities;
using Allup.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

namespace Allup.Areas.Admin.Controllers
{
    public class ProductsController : BaseController
    {
        public readonly AppDbContext _dbContext;
        public readonly CategoryServices _categoryServices;

        public ProductsController(AppDbContext dbContex, CategoryServices categoryServices)
        {
            _dbContext = dbContex;
            _categoryServices = categoryServices;
        }

        public async Task<IActionResult> Index()
        {

            var products = await _dbContext.Products
                .Include(c => c.ProductCategories)
                .ThenInclude(c => c.Category)
                .ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            var model = await _categoryServices.GetCategory();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            var viewModel = await _categoryServices.GetCategory();

            if (!ModelState.IsValid)
                return View(viewModel);

            //var parentCategory = await _dbContext
            //   .Categories
            //   .Where(x => !x.IsDeleted && x.IsMain && x.Id == model.ParentCategoryId)
            //   .Include(x => x.Children)
            //   .ThenInclude(x => x.ProductCategories)
            //   .ThenInclude(x => x.Product).FirstOrDefaultAsync();

            var childCategory = parentCategory.Children
                .FirstOrDefault(c => c.Id == model.ChildCategoryId);

            foreach (var item in childCategory.ProductCategories)
            {
                if (item.Product.Name == model.Name)
                {
                    ModelState.AddModelError("", "Eyni Adda Product Yarana Bilmaz");

                    return View(viewModel);
                }
            }

            var product = new Product
            {
                Name = model.Name,
                Discount = model.Discount,
                Brand = model.Brand,
                Description = model.Description,
                Rate = model.Rate,
                ExTax = model.ExTax,
                Price = model.Price,
                ProductCategories = new List<ProductCategory>(),
                ProductImages = new List<ProductImage>(),

            };

            var productImages = new List<ProductImage>();
            foreach (var image in model.Images)
            {
                if (!image.IsImage())
                {
                    ModelState.AddModelError("", "Shekil Secmelisiz");
                    return View(viewModel);
                }

                if (!image.IsAllowedSize(10))
                {
                    ModelState.AddModelError("", "Shekilin olcusu 10 mbdan az omalidi");
                    return View(viewModel);
                }

                var unicalName = await image.GenerateFile(Constants.ProductPath);
                productImages.Add(new ProductImage
                {
                    Name = unicalName,
                    ProductId = product.Id,
                });

            }

            product.ProductImages.AddRange(productImages);

            var productCategory = new List<ProductCategory>
            {
                new ProductCategory
                {
                    ProductId = product.Id,
                    CategoryId = parentCategory.Id,
                },

                new ProductCategory
                {
                    ProductId = product.Id,
                    CategoryId = childCategory.Id,
                }
            };

            product.ProductCategories.AddRange(productCategory);

            await _dbContext.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id is null) return BadRequest();

            var product = await _dbContext.Products
                    .Where(n => n.Id == id)
                    .Include(n => n.ProductImages)
                    .Include(n => n.ProductCategories)
                    .ThenInclude(n => n.Category)
                    .FirstOrDefaultAsync();

            if (product is null) return NotFound();

            var parentCategory = product.ProductCategories
                .Where(n => n.Category.IsMain)
                .First();

            var childCategory = product.ProductCategories
                 .Where(n => !n.Category.IsMain)
                 .First();

            var categories = await _dbContext
                .Categories
                .Where(x => !x.IsDeleted && x.IsMain)
                .Include(x => x.Children)  
                .ToListAsync();

            var parentCategoriesSelectListItem = new List<SelectListItem>();
            var childCategoriesSelectListItem = new List<SelectListItem>();

            categories.ForEach(x => parentCategoriesSelectListItem.Add(new SelectListItem(x.Name, x.Id.ToString())));

           parentCategory.Category.Children
                .ToList()
                .ForEach(x => childCategoriesSelectListItem.Add(new SelectListItem(x.Name, x.Id.ToString())));

            var productUpdateViewModel = new ProductUpdateViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Discount = product.Discount,
                Brand = product.Brand,
                Description = product.Description,
                Rate = product.Rate,
                ExTax = product.ExTax,
                Price = product.Price,
                ProductImages = product.ProductImages,
                ParentCategoryId = parentCategory.CategoryId,
                ParentCatigories = parentCategoriesSelectListItem,
                ChildCategoryId = childCategory.CategoryId,
                ChildCatigories = childCategoriesSelectListItem
            };

            return View(productUpdateViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, ProductUpdateViewModel model)
        {
            var product = await _dbContext.Products
                    .Where(n => n.Id == id)
                    .FirstOrDefaultAsync();
            if (product is null) return NotFound();

            var updatedProduct = new Product
            {
                Name = model.Name,
                Discount = model.Discount,
                Brand = model.Brand,
                Description = model.Description,
                Rate = model.Rate,
                ExTax = model.ExTax,
                Price = model.Price,
                ProductCategories = new List<ProductCategory>(),
                ProductImages = new List<ProductImage>(),
            };

            var productCategories = new List<ProductCategory>
            {
                new ProductCategory
                {
                    ProductId = product.Id,
                    CategoryId = model.ParentCategoryId,
                },

                new ProductCategory
                {
                    ProductId = product.Id,
                    CategoryId = model.ChildCategoryId,
                }
            };

            //var removedImagesIds = model.RemovedImagesIds.Split(", ").Select(imageId => new ProductImage
            //{
            //    Id = imageId
            //});

            //_dbContext.ProductImages.RemoveRange(removedImagesIds);
           
            //updatedProduct = productCategories;

            return Ok(product);
        }


        public async Task<IActionResult> LoadChildCategories(int? parentCategoryId)
        {

            var parentCatigories = await _dbContext.Categories
                .Where(c => !c.IsDeleted && c.IsMain && c.Id == parentCategoryId)
                .Include(c => c.Children)
                .FirstOrDefaultAsync();

            var childCategoriesSelectListItem = new List<SelectListItem>();
            parentCatigories.Children
                .ToList()
                .ForEach(x => childCategoriesSelectListItem.Add(new SelectListItem(x.Name, x.Id.ToString())));


            return Json(childCategoriesSelectListItem);
        }
    }
}
