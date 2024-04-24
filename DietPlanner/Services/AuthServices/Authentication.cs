
using System.Security.Cryptography;


namespace Services.AuthServices
{
    public static class Authentication
    {
        public static string Encrypt(string Password, string KeyBase64, out string IVKey)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Padding = PaddingMode.Zeros;
                aes.Key = Convert.FromBase64String(KeyBase64);

                aes.GenerateIV();
                IVKey = Convert.ToBase64String(aes.IV);

                ICryptoTransform encryptor = aes.CreateEncryptor();

                byte[] encryptedData;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(Password);
                        }
                        encryptedData = ms.ToArray();

                    }
                }
                return Convert.ToBase64String(encryptedData);

            }
        }


        public static string Checking(string password, string key,string iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Padding = PaddingMode.Zeros;
                aes.Key = Convert.FromBase64String(key);
                aes.IV = Convert.FromBase64String(iv);

               
                ICryptoTransform encryptor = aes.CreateEncryptor();
                byte[] encryptedData;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(password);
                        }
                        encryptedData = ms.ToArray();
                    }
                }
                return Convert.ToBase64String(encryptedData);
            }
        }

        public static string Decrypt(string Passwordhash, string KeyBase64, string IVKey)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Padding = PaddingMode.Zeros;
                aes.Key = Convert.FromBase64String(KeyBase64);
                aes.IV = Convert.FromBase64String(IVKey);



                ICryptoTransform decryptor = aes.CreateDecryptor();

                string Password = "";
                byte[] ciper = Convert.FromBase64String(Passwordhash);

                using (MemoryStream ms = new MemoryStream(ciper))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            Password = sr.ReadToEnd();
                        }
                    }
                }
                return Password;
            }
        }


    
    }
}
