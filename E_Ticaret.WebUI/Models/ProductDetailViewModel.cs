using E_Ticaret.Core.Entities;

namespace E_Ticaret.WebUI.Models
{
    public class ProductDetailViewModel
    {
        public Product? Product { get; set; }
        public IEnumerable<Product>? RelatedProducts { get; set; }
    }
}
