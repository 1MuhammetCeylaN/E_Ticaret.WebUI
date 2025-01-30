using E_Ticaret.Core.Entities;
using E_Ticaret.Service.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Ticaret.Service.Concrete
{
    public class CartService : ICartService
    {
        public List<CartLine> CartLines = new();
        public void AddProduct(Product product, int quantity)
        {
            var urun = CartLines.FirstOrDefault(p=>p.Product.Id == product.Id);

            if (urun != null) // Eğer o ürün sepette varsa miktarını arttır yoksa yeni bir ürün olarak sepete ekle
            {
                urun.Quantity += quantity;

            }
            else
            {
                CartLines.Add(new CartLine
                {
                    Quantity = quantity,
                    Product = product
                });
            }
        }

        public void ClearAll()
        {
            CartLines.Clear();
        }

        public void RemoveProduct(Product product)
        {
            CartLines.RemoveAll(p=> p.Product.Id == product.Id);
        }

        public decimal TotalPrice()
        {
            return  CartLines.Sum(p=> p.Product.Price * p.Quantity);
        }

        public void UpdateProduct(Product product, int quantity)
        {
            var urun = CartLines.FirstOrDefault(p => p.Product.Id == product.Id);

            if (urun != null) 
            {
                urun.Quantity = quantity;

            }
            else
            {
                CartLines.Add(new CartLine
                {
                    Quantity = quantity,
                    Product = product
                });
            }
        }
    }
}
