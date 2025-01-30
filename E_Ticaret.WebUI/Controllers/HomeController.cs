using System.Diagnostics;
using E_Ticaret.Core.Entities;
using E_Ticaret.Data;
using E_Ticaret.Service.Abstract;
using E_Ticaret.WebUI.Models;
using E_Ticaret.WebUI.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Ticaret.WebUI.Controllers
{
    public class HomeController : Controller
    {
        //private readonly DatabaseContext _context;

        //public HomeController(DatabaseContext context)
        //{
        //    _context = context;
        //}

        private readonly IService<Product> _serviceProduct;
        private readonly IService<News> _serviceNews;
        private readonly IService<Slider> _serviceSlider;
        private readonly IService<Contact> _serviceContact;

        public HomeController(IService<Product> serviceProduct, IService<News> serviceNews, IService<Slider> serviceSlider, IService<Contact> serviceContact)
        {
            _serviceProduct = serviceProduct;
            _serviceNews = serviceNews;
            _serviceSlider = serviceSlider;
            _serviceContact = serviceContact;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomePageViewModel()
            {
                //Sliders = await _context.Sliders.ToListAsync(),
                //Products = await _context.Products.Where(p => p.IsActive && p.IsHome).ToListAsync(),
                //News = await _context.News.ToListAsync()

                Sliders = await _serviceSlider.GetAllAsync(),
                Products = await _serviceProduct.GetAllAsync(p => p.IsActive && p.IsHome),
                News = await _serviceNews.GetAllAsync(n=>n.IsActive)
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ContactUs(Contact contact)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // await _context.Contacts.AddAsync(contact);
                    await _serviceContact.AddAsync(contact);
                    var sonuc = await _serviceContact.SaveChangesAsync();
                    if (sonuc > 0)
                    {
                        TempData["Message"] = @"<div class=""alert alert-success alert-dismissible fade show"" role=""alert"">
                        <strong>Mesajınız Başarıyla İletilmiştir!</strong>
    <button type=""button"" class=""btn-close"" data-bs-dismiss=""alert"" aria-label=""Close""></button>
    </div>";
                        // await MailHelper.SendMailAsync(contact);
                        return RedirectToAction("ContactUs");
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Hata Oluştu!");
                }
            }
            return View(contact);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("AccessDenied")] // Buraya özel bir route ile geliecek yani /Home/AccessDenied değilde direk /AccessDenied ile gelinebilr.
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
