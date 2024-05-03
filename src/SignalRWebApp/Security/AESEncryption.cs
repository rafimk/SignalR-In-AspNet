using System.Security.Cryptography;
using System.Text;

namespace SignalRWebApp.Security;

public class AESEncryption
{
    public static string DecryptStringAES(string encryptedValue)
    {
        var keybytes = Encoding.UTF8.GetBytes("8056483646328763");
        var iv = Encoding.UTF8.GetBytes("8056483646328763");
        var encrypted = Convert.FromBase64String(encryptedValue);
        var decryptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);
        return decryptedFromJavascript;
    }

    private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
    {
        // Check arguments.
        if (cipherText == null || cipherText.Length <= 0)
        {
            throw new ArgumentNullException("cipherText");
        }

        if (key == null || key.Length <= 0)
        {
            throw new ArgumentNullException("key");
        }

        if (iv == null || iv.Length <= 0)
        {
            throw new ArgumentNullException("key");
        }

        // Declare the string used to hold
        // the decrypted text.
        string plaintext = null;
        // Create an RijndaelManaged object
        // with the specified key and IV.
        using (var rijAlg = new RijndaelManaged())
        {
            //Settings
            rijAlg.Mode = CipherMode.CBC;
            rijAlg.Padding = PaddingMode.PKCS7;
            rijAlg.FeedbackSize = 128;
            rijAlg.Key = key;
            rijAlg.IV = iv;
            // Create a decrytor to perform the stream transform.
            var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
            // Create the streams used for decryption.
            using (var msDecrypt = new MemoryStream(cipherText))
            {
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        // Read the decrypted bytes from the decrypting stream
                        // and place them in a string.
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        return plaintext;
    }
}