using System.Security.Cryptography;
using System.Text;

namespace SuVac.Application.Utils;

internal static class Cryptography
{
    public static string Encrypt(string texto, string secret)
    {
        byte[] plainBytes = Encoding.UTF8.GetBytes(texto);
        string hash = ComputeHash(secret.Substring(0, 32));
        byte[] key = Encoding.UTF8.GetBytes(hash); // 32 bytes
        byte[] iv = [33, 24, 31, 46, 75, 64, 97, 18, 89, 10, 111, 132, 131, 144, 145, 250]; // 16 bytes

        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using ICryptoTransform encryptor = aes.CreateEncryptor();
        byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        return Convert.ToBase64String(encryptedBytes);
    }

    private static string ComputeHash(string input)
    {
        byte[] data = MD5.HashData(Encoding.UTF8.GetBytes(input));
        var sb = new StringBuilder();
        foreach (var c in data)
            sb.Append(c.ToString("x2"));
        return sb.ToString();
    }
}
