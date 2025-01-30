using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Ticaret.Core.Entities
{
    public class Contact : IEntity
    {
        public int Id { get; set; }
        [Display(Name = "Adı"),Required(ErrorMessage ="{0} Alanı Boş Geçilemez!")]
        public string Name { get; set; }
        [Display(Name = "Soyadı"), Required(ErrorMessage = "{0} Alanı Boş Geçilemez!")]
        public string SurName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [Display(Name = "Telefon")]
        public string? Phone { get; set; }
        [Display(Name = "Mesaj"), Required(ErrorMessage = "{0} Alanı Boş Geçilemez!")]
        public string Message { get; set; }
        [Display(Name = "Kayıt Tarihi")]
        public DateTime CreateDate { get; set; } = DateTime.Now;
        
    }
}
