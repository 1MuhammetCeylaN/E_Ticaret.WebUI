using E_Ticaret.Core.Entities;
using E_Ticaret.Data;
using E_Ticaret.Service.Abstract;
using E_Ticaret.WebUI.Models;
using E_Ticaret.WebUI.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Claims;

namespace E_Ticaret.WebUI.Controllers
{
    public class AccountController : Controller
    {
        //private readonly DatabaseContext _context;

        //public AccountController(DatabaseContext context)
        //{
        //    _context = context;
        //}

        private readonly IService<AppUser> _service;
        private readonly IService<Order> _orderService;

        public AccountController(IService<AppUser> service, IService<Order> orderService)
        {
            _service = service;
            _orderService = orderService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            // var user = _context.AppUsers.FirstOrDefault(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);

            AppUser user = await _service.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);

            if (user == null)
            {
                return NotFound();
            }
            var model = new UserEditViewModel()
            {
                Email = user.Email,
                Name = user.Name,
                Password = user.Password,
                Phone = user.Phone,
                SurName = user.SurName,
                Id = user.Id
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Index(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // var user = _context.AppUsers.FirstOrDefault(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);

                    AppUser user = await _service.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);


                    if (user != null)
                    {
                        user.Email = model.Email;
                        user.Name = model.Name;
                        user.Password = model.Password;
                        user.Phone = model.Phone;
                        user.SurName = model.SurName;

                        _service.Update(user);
                        var sonuc = _service.SaveChanges();

                        if (sonuc > 0)
                        {
                            TempData["Message"] = @"<div class=""alert alert-success alert-dismissible fade show"" role=""alert"">
                        <strong>Bilgileriniz Güncellenmiştir!</strong>
    <button type=""button"" class=""btn-close"" data-bs-dismiss=""alert"" aria-label=""Close""></button>
    </div>";
                            // await MailHelper.SendMailAsync(contact);
                            return RedirectToAction("Index");
                        }

                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Bir Hata Oluştu!");
                    throw;
                }
            }
            return View(model);
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignInAsync(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {

                try
                {
                    // var account = await _context.AppUsers.FirstOrDefaultAsync(x => x.Email == model.Email && x.Password == model.Password && x.IsActive);

                    var account = await _service.GetAsync(x => x.Email == model.Email && x.Password == model.Password && x.IsActive);

                    if ((account == null))
                    {
                        ModelState.AddModelError("", "Giriş Başarısız! Lütfen Email veya Şifreyi kontrol edin!");

                    }
                    else
                    {
                        var claims = new List<Claim>()
                        {
                            new(ClaimTypes.Name , account.Name),
                            new(ClaimTypes.Email , account.Email),
                            new(ClaimTypes.Role,account.IsAdmin ? "Admin" : "Customer"),
                            new("UserId",account.Id.ToString()),
                            new("UserGuid",account.UserGuid.ToString())
                        };
                        var userIdentity = new ClaimsIdentity(claims, "Login");
                        ClaimsPrincipal userPrincipal = new ClaimsPrincipal(userIdentity);
                        await HttpContext.SignInAsync(userPrincipal);
                        return Redirect(string.IsNullOrEmpty(model.ReturnUrl) ? "/" : model.ReturnUrl); // Eğer returnurl yoksa yani boşsa anasayfaya yönlendir varsada returnurldeki yere yönlendir.
                    }


                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Bir Hata Oluştu");
                }
            }

            return View(model);
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(AppUser appUser)
        {
            appUser.IsAdmin = false;
            appUser.IsActive = true;
            if (ModelState.IsValid)
            {
                await _service.AddAsync(appUser);
                await _service.SaveChangesAsync();
                return RedirectToAction("Index");

            }

            return View();
        }

        public async Task<IActionResult> SignOutAsync()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("SignIn");
        }

        [Authorize]
        public async Task<IActionResult> MyOrders()
        {
            AppUser user = await _service.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);

            if (user == null)
            {
                await HttpContext.SignOutAsync();
                return RedirectToAction("SignIn");
            }

            var model = _orderService.GetQueryable().Where(s => s.AppUserId == user.Id).Include(o => o.OrderLines).ThenInclude(p => p.Product);

            return View(model);
        }

        public async Task<IActionResult> PasswordChange(string user)
        {
            if (user == null)
            {
                return BadRequest("Geçersiz istek!!");
            }

            AppUser appUser = await _service.GetAsync(x => x.UserGuid.ToString() == user);

            if (appUser == null)
            {
                return NotFound("Geçersiz Değer!!");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordChange(string user, string Password)
        {
            if (user == null)
            {
                return BadRequest("Geçersiz istek!!");
            }

            AppUser appUser = await _service.GetAsync(x => x.UserGuid.ToString() == user);

            if (appUser == null)
            {
                ModelState.AddModelError("", "Geçersiz Değer!!");
                return View();
            }
            appUser.Password = Password;
            var sonuc =  await _service.SaveChangesAsync();

            if(sonuc > 0)
            {
                TempData["Message"] = @"<div class=""alert alert-success alert-dismissible fade show"" role=""alert"">
                        <strong>Şifreniz başarıyla güncellenmiştir.</strong>
    <button type=""button"" class=""btn-close"" data-bs-dismiss=""alert"" aria-label=""Close""></button>
    </div>";
            }
            else 
            {
                ModelState.AddModelError("", "Güncelleme başarısız!!");  
            }
            return View();
        }

        public IActionResult PasswordRenew()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordRenew(string Email)
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ModelState.AddModelError("", "Email Boş Geçilemez");
            }
            AppUser user = await _service.GetAsync(x => x.Email == Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Geçersiz Email , Email Bulunamadı!");
                return View();  
            }
            string mesaj = $"Sayın {user.Name} {user.SurName} <br> şifrenizi yenilemek için lütfen <a href='https://localhost:7282/Account/PasswordChange?user={user.UserGuid.ToString()}' >Buraya Tıklayınız</a>.";
            
            var sonuc = await MailHelper.SendMailAsync(Email, mesaj, "Şifremi Yenile");

            if (sonuc)
            {
                TempData["Message"] = @"<div class=""alert alert-success alert-dismissible fade show"" role=""alert"">
                        <strong>Şifre yenileme bağlantınız email adresinize gönderilmiştir.</strong>
    <button type=""button"" class=""btn-close"" data-bs-dismiss=""alert"" aria-label=""Close""></button>
    </div>";
            }
            else
            {
                TempData["Message"] = @"<div class=""alert alert-success alert-dismissible fade show"" role=""alert"">
                        <strong>Bir hata oluştu!! Şifre yenileme bağlantınız email adresinize gönderilemedi! </strong>
    <button type=""button"" class=""btn-close"" data-bs-dismiss=""alert"" aria-label=""Close""></button>
    </div>";
            }

            
            return View();
        }

    }
}
