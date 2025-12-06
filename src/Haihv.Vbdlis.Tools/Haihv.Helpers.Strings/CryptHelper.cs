using System.Security.Cryptography;
using System.Text;

namespace Haihv.Tools.Hsq.Helpers;

public static class CryptHelper
{
    // App-specific seed - thay đổi này cho mỗi ứng dụng
    private const string AppSeed = "qh7WZW@7j16mdIWG";
    private static string? _cachedEncryptionKey;
    private static readonly Lock KeyLock = new();

    public static string GetEncryptionKey()
    {
        // Sử dụng double-checked locking pattern để đảm bảo thread-safe
        if (_cachedEncryptionKey != null)
            return _cachedEncryptionKey;

        lock (KeyLock)
        {
            const string seed = "@haihv.vn";
            try
            {
                // Lấy đặc trưng hệ thống từ môi trường hiện tại
                var deviceId = $"{Environment.MachineName}-{Environment.UserName}";
                var platform = Environment.OSVersion.Platform.ToString();
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var processPath = AppContext.BaseDirectory;

                // Sử dụng stable hash thay vì GetHashCode() - lấy path và tạo consistent hash
                var pathBytes = Encoding.UTF8.GetBytes($"{appDataPath}|{processPath}");
                var pathHashBytes = SHA256.HashData(pathBytes);
                var pathHash = Convert.ToHexString(pathHashBytes)[..8]; // Lấy 8 ký tự đầu

                // Kết hợp app seed với system characteristics
                var combinedString = $"{AppSeed}|{deviceId}|{platform}|{pathHash}";

                // Tạo hash 256-bit từ combined string
                var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(combinedString));

                // Chuyển thành hex string (64 chars) và lấy 32 chars đầu cho AES-256
                var hexString = Convert.ToHexString(hashBytes).ToLowerInvariant();
                _cachedEncryptionKey = hexString[..(32 - seed.Length)] + seed; // Lấy thông tin key có ký tự đặc biệt

                return _cachedEncryptionKey;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generating encryption key: {ex.Message}");
                // Fallback key nếu có lỗi (không nên xảy ra trong production)
                _cachedEncryptionKey = "GA5#l4cNSLmqfUHwRTYQznD4eB#vZP#%";
                return _cachedEncryptionKey;
            }
        }
    }

    private static byte[] GenerateKey(this string input, int keySize = 256)
    {
        // Sử dụng hàm băm SHA256 để tạo một bản rút gọn của chuỗi
        var hashedInput = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        // Chuyển đổi độ dài của khóa từ bit sang byte
        var keyLengthBytes = keySize / 8; //Phép chia lấy phần nguyên
        // Nếu độ dài của bản băm ngắn hơn độ dài yêu cầu của khóa, hãy lặp lại chuỗi ban đầu để đủ độ dài
        if (hashedInput.Length < keyLengthBytes)
        {
            var repeatedInput = new byte[keyLengthBytes];
            var remainingBytes = keyLengthBytes;
            var position = 0;
            // Lặp lại chuỗi ban đầu để tạo khóa có độ dài mong muốn
            while (remainingBytes > 0)
            {
                var bytesToCopy = Math.Min(hashedInput.Length, remainingBytes);
                Array.Copy(hashedInput, 0, repeatedInput, position, bytesToCopy);
                remainingBytes -= bytesToCopy;
                position += bytesToCopy;
            }

            return repeatedInput;
        }
        // Nếu độ dài của bản băm dài hơn độ dài yêu cầu của khóa, hãy cắt bớt

        var trimmedKey = new byte[keyLengthBytes];
        Array.Copy(hashedInput, trimmedKey, keyLengthBytes);
        return trimmedKey;
    }

    public static (string, string) Encrypt(this string plainText, string secretKey, int keySize = 256)
    {
        using var aesAlg = Aes.Create();
        aesAlg.Key = GenerateKey(secretKey, keySize);
        aesAlg.GenerateIV(); // Tạo IV mới cho mỗi lần mã hóa
        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        return (Convert.ToBase64String(encryptedBytes), Convert.ToBase64String(aesAlg.IV));
    }

    public static string Decrypt(this string cipherText, string secretKey, string iv, int keySize = 256)
    {
        using var aesAlg = Aes.Create();
        aesAlg.Key = GenerateKey(secretKey, keySize);
        aesAlg.IV = Convert.FromBase64String(iv);
        var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
        var cipherBytes = Convert.FromBase64String(cipherText);
        var decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

        return Encoding.UTF8.GetString(decryptedBytes);
    }
}
