using E_Ticaret.Core.Entities;
using E_Ticaret.Data;
using E_Ticaret.Service.Abstract;
using E_Ticaret.WebUI.ViewComponents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Ticaret.WebUI.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly DatabaseContext _context;

        public CategoriesController(DatabaseContext context)
        {
            _context = context;
        }

        //private readonly IService<Category> _service;

        //public CategoriesController(IService<Category> service)
        //{
        //    _service = service;
        //}

        public async Task<IActionResult> IndexAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.Include(p => p.Products).FirstOrDefaultAsync(m => m.Id == id);
           // var category = await _service.GetQueryable().Include(p => p.Products).FirstOrDefaultAsync(m => m.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }


        public IActionResult AllProducts(int id)
        {
            // Ana kategoriyi ve alt kategorileri al
            var allCategories = _context.Categories
                .Where(c => c.Id == id || c.ParentId == id)
                .Select(c => c.Id)
                .ToList();

            // Bu kategorilere ait ürünleri al
            var products = _context.Products
                .Where(p => allCategories.Contains(p.Category.Id))
                .ToList();

            return View(products);
        }
    }
}
