using E_Ticaret.Data;
using E_Ticaret.Service.Abstract;
using E_Ticaret.Service.Concrete;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace E_Ticaret.WebUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Uygulamada session kullanacaðýmýzý bildidiyoruz
            builder.Services.AddSession(options=> { 
                options.Cookie.Name = "E_Ticaret.Session";
                options.Cookie.HttpOnly = true;  // Bu ayar, çerezin yalnýzca sunucu tarafýnda eriþilebilir olmasýný saðlar.
                options.Cookie.IsEssential = true; // Bu ayar, çerezin zorunlu bir çerez olduðunu belirtir. Kullanýcý çerez tercihleri sunulduðunda, bu çerez kullanýcýnýn onayýna bakýlmaksýzýn oluþturulur.
                options.IdleTimeout = TimeSpan.FromDays(1); // Bu ayar, oturumun maksimum boþta kalma süresini belirler.  1 gün içinde oturum kullanýlmazsa sona erer.
                options.IOTimeout = TimeSpan.FromMinutes(10); // Bu ayar, oturumun I/O iþlemleri sýrasýnda ne kadar süre bekleyeceðini belirler. Örnekte, oturum I/O (giriþ/çýkýþ) iþlemleri için maksimum 10 dakika bekler. Bu süre aþýlýrsa iþlem bir hata ile sonlanýr.

            });

            builder.Services.AddDbContext<DatabaseContext>(options=>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"));
            });

            builder.Services.AddScoped(typeof(IService<>), typeof(Service<>)); // Her IService isteði yaptýðýmýzda sistem arka tarafta service sýnýfýnýn bir örneðini oluþturup onu sunacak 

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(x=>
                {
                    x.LoginPath = "/Account/SignIn"; // Giriþ yolu
                    x.LogoutPath = "/Account/SignIn"; // Çýkýþ yolu
                    x.AccessDeniedPath = "/AccessDenied"; // Eriþim engellendi ekraný kullanýcýnýn yetkisinin olmadýðý zaman gösterilecek 
                    x.Cookie.Name = "Account";
                    x.Cookie.MaxAge = TimeSpan.FromDays(7); // Cookie 'nin yaþam süresi yani cookie ne kadar süre ayakta kalacak
                    x.Cookie.IsEssential = true; // Kalýcý cookie oluþmasý saðlanýyor.
                });
            builder.Services.AddAuthorization(x=>
            {
                x.AddPolicy("AdminPolicy", policy => policy.RequireClaim(ClaimTypes.Role, "Admin")); // kullanýcý admin rolüne sahipse bu ekranlara eriþebilecek
                x.AddPolicy("UserPolicy", policy => policy.RequireClaim(ClaimTypes.Role, "Admin","User","Customer")); // kullanýcý admin user veya customer rolüne sahipse bu ekranlara eriþebilecek
            }); // Yetkilendirme

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseSession();  // Session kullan


            app.UseAuthentication(); // Bu önce gelmeli önce oturum açma
            app.UseAuthorization();  // sonra yetkilendirme

            

            app.MapControllerRoute(
            name: "admin",
            pattern: "{area:exists}/{controller=Main}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
                

            app.Run();
        }
    }
}
