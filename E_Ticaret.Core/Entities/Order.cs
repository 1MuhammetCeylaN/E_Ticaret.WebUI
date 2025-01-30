using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Ticaret.Core.Entities
{
    public class Order : IEntity
    {
        public int Id { get; set; }
        [Display(Name ="Sipariş Numaarası"),StringLength(50)]
        public string OrderNumber { get; set; }
        [Display(Name = "Sipariş Toplam Tuatarı")]
        public decimal TotalPrice { get; set; }
        [Display(Name = "Müşteri Numarası")]
        public int AppUserId { get; set; }
        [Display(Name = "Müşteri")]
        public AppUser? AppUser { get; set; }
        [Display(Name = "Müşteri")]
        public string CustomerId { get; set; }
        [Display(Name = "Fatura Adresi")]
        public string BillingAddress { get; set; }
        [Display(Name = "Teslimat Adresi")]
        public string DeliveryAddress { get; set; }
        [Display(Name = "Sipariş Tarihi")]
        public DateTime OrderDate { get; set; }
        [Display(Name = "Siparişler")]
        public List<OrderLine>? OrderLines { get; set; }
        [Display(Name = "Sipariş Durumu")]
        public EnumOrderState OrderState { get; set; }
    }

        public enum EnumOrderState
        {
        [Display(Name ="Onay Bekliyor")]
        Waiting,
        [Display(Name = "Onaylandı")]
        Approved,
        [Display(Name = "Kargoya Verildi")]
        Shipped,
        [Display(Name = "Tamamlandı")]
        Complated,
        [Display(Name = "İptal Edildi")]
        Cancelled,
        [Display(Name = "İade Edildi")]
        Returned


    }
}
