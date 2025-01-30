using E_Ticaret.Core.Entities;
using System.Net;
using System.Net.Mail;

namespace E_Ticaret.WebUI.Utils
{
    public class MailHelper
    {
        public static async Task<bool> SendMailAsync(Contact contact)
        {
            SmtpClient smtpClient = new SmtpClient("mail.siteadi.com",587); // Host ve port
            smtpClient.Credentials = new NetworkCredential("info@siteadi.com","mailşifresi");
            smtpClient.EnableSsl = false;
            MailMessage message = new MailMessage();
            message.From = new MailAddress("info@siteadi.com"); // maili gönderen adres
            message.To.Add($"{contact.Email}"); // Mailin gideceği adresi 
            message.Subject = "Mail'in konu başlığı...";
            message.Body = $"İsim:{contact.Name} Soyisim: {contact.SurName} Email:{contact.Email} Phone:{contact.Phone} Mesaj:{contact.Message}";
            message.IsBodyHtml = true;

            try
            {
                await smtpClient.SendMailAsync(message);
                smtpClient.Dispose();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }


        public static async Task<bool> SendMailAsync(string email,string mailBody , string subject)
        {
            SmtpClient smtpClient = new SmtpClient("mail.siteadi.com", 587); // Host ve port
            smtpClient.Credentials = new NetworkCredential("info@siteadi.com", "mailşifresi");
            smtpClient.EnableSsl = false;
            MailMessage message = new MailMessage();
            message.From = new MailAddress("info@siteadi.com"); // maili gönderen adres
            message.To.Add(email); // Mailin gideceği adresi 
            message.Subject = subject;
            message.Body = mailBody;
            message.IsBodyHtml = true;

            try
            {
                await smtpClient.SendMailAsync(message);
                smtpClient.Dispose();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
    }
}
