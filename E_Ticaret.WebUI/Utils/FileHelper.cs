namespace E_Ticaret.WebUI.Utils
{
    public class FileHelper
    {
        
        public static async Task<string> FileLoaderAsync(IFormFile formFile , string filePath = "/Img/")
        { 
            string fileName = "";
            if (formFile != null && formFile.Length > 0)
            {
                fileName = formFile.FileName.ToLower();
                string directory = Directory.GetCurrentDirectory() + "/wwwroot" + filePath + fileName; // Uygulamanın çalıştığı dizini bulur.

                using var stream = new FileStream(directory, FileMode.Create);
                
                await formFile.CopyToAsync(stream);

            }

            return fileName;
        }

        public static bool FileRemover(string fileName , string filePath = "/Img/")
        {
            string directory = Directory.GetCurrentDirectory() + "/wwwroot" + filePath + fileName;

            if (File.Exists(directory)) // Bir dosya varmı bunu kontrol eder.
            {
                File.Delete(directory); // Varsa siler ve geriye true döner yoksa false döner.
                return true;
            }
            return false;
        }

    }
}
