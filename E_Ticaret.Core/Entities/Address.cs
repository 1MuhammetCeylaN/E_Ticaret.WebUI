using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace E_Ticaret.Core.Entities
{
    public class Address : IEntity
    {
        public int Id { get; set; }
        [Display(Name ="Adres Başlığı"),Required(ErrorMessage = "{0} Alanı Zorunludur!")]
        [StringLength(50)]
        public string Title { get; set; }
        [Display(Name ="Şehir"),Required(ErrorMessage ="{0} Alanı Zorunludur!")]
        [StringLength(50)]
        public string City { get; set; }
        [Display(Name = "İlçe"), Required(ErrorMessage = "{0} Alanı Zorunludur!")]
        [StringLength(50)]
        public string District { get; set; }
        [Display(Name = "Mahalle"), Required(ErrorMessage = "{0} Alanı Zorunludur!")]
        [StringLength(100)]
        public string Street { get; set; }
        [Display(Name = "Açık Adres"), Required(ErrorMessage = "{0} Alanı Zorunludur!")]
        [DataType(DataType.MultilineText)]
        public string OpenAddress { get; set; }
        [Display(Name = "Aktif mi?"), Required(ErrorMessage = "{0} Alanı Zorunludur!")]
        public bool IsActive { get; set; }
        [Display(Name = "Fatura Adresi mi?"), Required(ErrorMessage = "{0} Alanı Zorunludur!")]
        public bool IsBillingAddress { get; set; }
        [Display(Name = "Teslimat Adresi mi?"), Required(ErrorMessage = "{0} Alanı Zorunludur!")]
        public bool IsDeliveryAddress { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public Guid? AddressGuid { get; set; } = Guid.NewGuid();
        public int? AppUserId { get; set; }
        [Display(Name ="Kullanıcı")]
        public AppUser? AppUser { get; set; }

    }
}
