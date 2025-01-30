using E_Ticaret.Core.Entities;
using E_Ticaret.Service.Abstract;
using E_Ticaret.Service.Concrete;
using E_Ticaret.WebUI.ExtensionMethods;
using E_Ticaret.WebUI.Models;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace E_Ticaret.WebUI.Controllers
{
    public class CartController : Controller
    {
        private readonly IService<Product> _productService;
        private readonly IService<Core.Entities.Address> _addressService;
        private readonly IService<AppUser> _appUserService;
        private readonly IService<Order> _orderService;
        private readonly IConfiguration _configuration;

        public CartController(IService<Product> productService, IService<Core.Entities.Address> addressService, IService<AppUser> appUserService, IService<Order> orderService, IConfiguration configuration)
        {
            _productService = productService;
            _addressService = addressService;
            _appUserService = appUserService;
            _orderService = orderService;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            var model = new CartViewModel()
            {
                CartLines = cart.CartLines,
                TotalPrice = cart.TotalPrice()
            };
            return View(model);
        }

        public IActionResult Add(int ProductId, int quantity = 1)
        {
            var product = _productService.Find(ProductId);
            if (product != null)
            {
                var cart = GetCart();
                cart.AddProduct(product, quantity);
                HttpContext.Session.SetJson("Cart", cart);
                return Redirect(Request.Headers["Referer"].ToString());
            }
            return RedirectToAction("Index");
        }

        public IActionResult Update(int ProductId, int quantity = 1)
        {
            var product = _productService.Find(ProductId);
            if (product != null)
            {
                var cart = GetCart();
                cart.UpdateProduct(product, quantity);
                HttpContext.Session.SetJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Remove(int ProductId)
        {
            var product = _productService.Find(ProductId);
            if (product != null)
            {
                var cart = GetCart();
                cart.RemoveProduct(product);
                HttpContext.Session.SetJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var cart = GetCart();

            var appUser = await _appUserService.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);
            if (appUser == null) { return RedirectToAction("SignIn", "Account"); }

            var addresses = await _addressService.GetAllAsync(a => a.AppUserId == appUser.Id && a.IsActive);
            var model = new CheckoutViewModel()
            {
                CartProducts = cart.CartLines,
                TotalPrice = cart.TotalPrice(),
                Addresses = addresses
            };
            return View(model);
        }
        

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Checkout(string CardNumber, string CardNameSurname, string CardMonth, string CardYear, string CVV, string DeliveryAddress, string BillingAddress)
        {
            var cart = GetCart();

            var appUser = await _appUserService.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);
            if (appUser == null) { return RedirectToAction("SignIn", "Account"); }

            var addresses = await _addressService.GetAllAsync(a => a.AppUserId == appUser.Id && a.IsActive);
            var model = new CheckoutViewModel()
            {
                CartProducts = cart.CartLines,
                TotalPrice = cart.TotalPrice(),
                Addresses = addresses
            };

            if (string.IsNullOrEmpty(CardNumber) || string.IsNullOrEmpty(CardNameSurname) || string.IsNullOrEmpty(CardMonth) || string.IsNullOrEmpty(CardYear) || string.IsNullOrEmpty(CVV) || string.IsNullOrEmpty(DeliveryAddress) )
            {
                return View(model);
            }


            var faturaAdresi = addresses.FirstOrDefault(a => a.AddressGuid.ToString() == BillingAddress);
            var teslimatAdresi = addresses.FirstOrDefault(a=>a.AddressGuid.ToString() == DeliveryAddress);

            // Ödeme İşlemi

            var siparis = new Order
            {
                AppUserId = appUser.Id,
                BillingAddress = $"{faturaAdresi.OpenAddress} {faturaAdresi.District} - {faturaAdresi.City} ", // BillingAddress,
                DeliveryAddress = $"{teslimatAdresi.OpenAddress} {teslimatAdresi.District} - {teslimatAdresi.City} ", // DeliveryAddress,

                CustomerId = appUser.UserGuid.ToString(),
                OrderDate = DateTime.Now,
                TotalPrice = cart.TotalPrice(),
                OrderNumber = Guid.NewGuid().ToString(),
                OrderState = 0,
                OrderLines = []
            };



            // #region OdemeIslemi

            Options options = new Options();
            options.ApiKey = "sandbox-qVrJZDJLAOvSrf6opchDFezVBJtO7jyt"; // _configuration["IyzicOptions:ApiKey"];
            options.SecretKey = "sandbox-cPCNHmDRPqyMgUFxvKY2AjV6f8Eq2Fi1";//  _configuration["IyzicOptions:SecretKey"];
            options.BaseUrl = "https://sandbox-api.iyzipay.com";// _configuration["IyzicOptions:BaseUrl"];

            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = HttpContext.Session.Id;
            request.Price = siparis.TotalPrice.ToString().Replace(",",".");
            request.PaidPrice = siparis.TotalPrice.ToString().Replace(",", ".");
            request.Currency = Currency.TRY.ToString();
            request.Installment = 1;
            request.BasketId = "B" + HttpContext.Session.Id;
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

            PaymentCard paymentCard = new PaymentCard();
            paymentCard.CardHolderName = CardNameSurname; // "John Doe";
            paymentCard.CardNumber = CardNumber; // "5528790000000008";
            paymentCard.ExpireMonth = CardMonth; // "12";
            paymentCard.ExpireYear = CardYear; // "2030";
            paymentCard.Cvc = CVV; // "123";
            paymentCard.RegisterCard = 0;
            request.PaymentCard = paymentCard;

            Buyer buyer = new Buyer();
            buyer.Id = "BY" + appUser.Id;
            buyer.Name = appUser.Name; // "John";
            buyer.Surname = appUser.SurName; // "Doe";
            buyer.GsmNumber = appUser.Phone; // "+905350000000";
            buyer.Email = appUser.Email; // "email@email.com";
            buyer.IdentityNumber = "11111111111";
            buyer.LastLoginDate = DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss");
            buyer.RegistrationDate = appUser.CreateDate.ToString("yyyy-mm-dd hh:mm:ss"); //"2013-04-21 15:12:09";
            buyer.RegistrationAddress = siparis.DeliveryAddress; // "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            buyer.Ip = HttpContext.Connection.RemoteIpAddress?.ToString(); // "85.34.78.112";
            buyer.City = teslimatAdresi.City; // "Istanbul";
            buyer.Country = "Turkey";
            buyer.ZipCode = "";
            request.Buyer = buyer;

            var shippingAddress = new Iyzipay.Model.Address();
            shippingAddress.ContactName = appUser.Name + " " + appUser.SurName;
            shippingAddress.City = teslimatAdresi.City;
            shippingAddress.Country = "Turkey";
            shippingAddress.Description = teslimatAdresi.OpenAddress; // "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            shippingAddress.ZipCode = "";
            request.ShippingAddress = shippingAddress;

            var billingAddress = new Iyzipay.Model.Address();
            billingAddress.ContactName = appUser.Name + " " + appUser.SurName;
            billingAddress.City = teslimatAdresi.City;
            billingAddress.Country = "Turkey";
            billingAddress.Description = faturaAdresi.OpenAddress; // "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            billingAddress.ZipCode = "";
            request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();

            //BasketItem firstBasketItem = new BasketItem();
            //firstBasketItem.Id = "BI101";
            //firstBasketItem.Name = "Binocular";
            //firstBasketItem.Category1 = "Collectibles";
            //firstBasketItem.Category2 = "Accessories";
            //firstBasketItem.ItemType = BasketItemType.PHYSICAL.ToString();
            //firstBasketItem.Price = "0.3";
            //basketItems.Add(firstBasketItem);


            foreach (var item in cart.CartLines)
            {
                siparis.OrderLines.Add(new OrderLine
                {
                    ProductId = item.Product.Id,
                    OrderId = siparis.Id,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price,

                });
                basketItems.Add(new BasketItem
                {
                    Id = item.Product.Id.ToString(),
                    Name = item.Product.Name,
                    Category1 = "Kategori",
                    ItemType = BasketItemType.PHYSICAL.ToString(),
                    Price = (item.Product.Price * item.Quantity).ToString().Replace(",", ".")
                });
            }


            request.BasketItems = basketItems;

            Payment payment = await Payment.Create(request, options);



            //  #endregion

            try
            {
                if (payment.Status == "success")
                {
                    // Ödeme başarılı , sipariş oluştur.

                    await _orderService.AddAsync(siparis);
                    var sonuuc = await _orderService.SaveChangesAsync();
                    if (sonuuc > 0)
                    {
                        HttpContext.Session.Remove("Cart");
                        return RedirectToAction("Thanks");
                    }
                }
                else
                {
                    TempData["Message"] = $"<div class='alert alert-danger' >Bir hata oluştu! ödeme işlemi başarısız!</div> ({payment.ErrorMessage})";
                }
            }
            catch (Exception)
            {

                TempData["Message"] = "<div class='alert alert-danger' >Bir Hata Oluştu!</div>";
            }

            return View(model);
        }

        public IActionResult Thanks()
        {
            return View();  
        }

        private CartService GetCart()
        {
            return HttpContext.Session.GetJson<CartService>("Cart") ?? new CartService();
        }

    }
}
