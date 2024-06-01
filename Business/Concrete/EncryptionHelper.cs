using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class EncryptionHelper
    {
        public static (byte[], byte[], byte[]) EncryptFile(Stream inputStream)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.GenerateKey();
                aesAlg.GenerateIV();

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV), CryptoStreamMode.Write))
                    {
                        inputStream.CopyTo(csEncrypt);
                    }
                    return (msEncrypt.ToArray(), aesAlg.Key, aesAlg.IV);
                }
            }
        }
        public static byte[] DecryptFile(byte[] encryptedData, string key, string iv)
        {
            // Anahtar ve IV'yi byte dizisine dönüştür
            byte[] keyBytes = Convert.FromBase64String(key);
            byte[] ivBytes = Convert.FromBase64String(iv);

            using (Aes aesAlg = Aes.Create())
            {
                // Şifreleme anahtarını ve IV'yi ayarla
                aesAlg.Key = keyBytes;
                aesAlg.IV = ivBytes;

                // Şifreleme modunu ve padding'i ayarla
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                // Decryptor oluştur
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(encryptedData))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (MemoryStream msPlainText = new MemoryStream())
                        {
                            csDecrypt.CopyTo(msPlainText);
                            return msPlainText.ToArray();
                        }
                    }
                }
            }
        }
    }
}
