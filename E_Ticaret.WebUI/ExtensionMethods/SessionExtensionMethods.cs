using Newtonsoft.Json;

namespace E_Ticaret.WebUI.ExtensionMethods
{
    public static class SessionExtensionMethods
    {
        public static void SetJson(this ISession session , string key ,object value) // Json tipine dönüştür.
        {
            session.SetString(key, JsonConvert.SerializeObject(value)); // Bize gelen object türündeki valueyu serialize edeceğiz yani json tipine çevireceğiz.
        

        }
        // Herhangi bir tip ve null olabilir.
        public static T? GetJson<T>(this ISession session , string key) where T : class  // Jsondan 
        {
            var data = session.GetString(key);  // Parametreden gönderilen keyi al ve sessiondaki data yı getir.

            return data == null ? default(T) : JsonConvert.DeserializeObject<T>(data); // eğer data null sa dafult olarak T'yi dön değilse gelen objeyi jsondan objececte dönğştür. 
        }
    }
}
