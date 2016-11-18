using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace SpotLightSpider.Util
{
    /// <summary>
    /// 加密管理,线程安全
    /// </summary>
    public static class Hasher
    {
        private static readonly Encoding CHARSET = Encoding.UTF8;
        private static readonly char[] DIGITS_LOWER = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
        private static readonly char[] DIGITS_UPPER = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
        private static readonly ThreadLocal<MD5CryptoServiceProvider> MESSAGE_DIGEST_MD5 = new ThreadLocal<MD5CryptoServiceProvider>(() => new MD5CryptoServiceProvider());
        private static readonly ThreadLocal<SHA1CryptoServiceProvider> MESSAGE_DIGEST_SHA1 = new ThreadLocal<SHA1CryptoServiceProvider>(() => new SHA1CryptoServiceProvider());
        private static readonly ThreadLocal<SHA256CryptoServiceProvider> MESSAGE_DIGEST_SHA256 = new ThreadLocal<SHA256CryptoServiceProvider>(() => new SHA256CryptoServiceProvider());
        private static readonly ThreadLocal<SHA384CryptoServiceProvider> MESSAGE_DIGEST_SHA384 = new ThreadLocal<SHA384CryptoServiceProvider>(() => new SHA384CryptoServiceProvider());
        private static readonly ThreadLocal<SHA512CryptoServiceProvider> MESSAGE_DIGEST_SHA512 = new ThreadLocal<SHA512CryptoServiceProvider>(() => new SHA512CryptoServiceProvider());

        /// <summary>
        /// 字节数组转16进制字符数组
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="lowerCase">是否小写</param>
        /// <returns>16进制字符数组</returns>
        private static char[] EncodeHex(byte[] bytes, bool lowerCase)
        {
            return EncodeHex(bytes, lowerCase ? DIGITS_LOWER : DIGITS_UPPER);
        }

        /// <summary>
        /// 字节数组转16进制字符数组
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="digits">编码表</param>
        /// <returns>16进制字符数组</returns>
        private static char[] EncodeHex(byte[] bytes, char[] digits)
        {
            char[] result = new char[bytes.Length << 1];
            int index = 0;
            foreach (byte bbyte in bytes)
            {
                result[index++] = digits[(0xF0 & bbyte) >> 4];
                result[index++] = digits[0x0F & bbyte];
            }
            return result;
        }


        /// <summary>
        /// 获取字符串的 MD5 值
        /// </summary>
        /// <param name="value">指定字符串</param>
        /// <returns>MD5 值</returns>
        public static byte[] MD5Hash(string value)
        {
            return MESSAGE_DIGEST_MD5.Value.ComputeHash(CHARSET.GetBytes(value));
        }

        /// <summary>
        /// 获取字符串的 SHA1 值
        /// </summary>
        /// <param name="value">指定字符串</param>
        /// <returns>SHA1 值</returns>
        public static byte[] SHA1Hash(string value)
        {
            return MESSAGE_DIGEST_SHA1.Value.ComputeHash(CHARSET.GetBytes(value));
        }

        /// <summary>
        /// 获取字符串的 SHA256 值
        /// </summary>
        /// <param name="value">指定字符串</param>
        /// <returns>SHA256 值</returns>
        public static byte[] SHA256Hash(string value)
        {
            return MESSAGE_DIGEST_SHA256.Value.ComputeHash(CHARSET.GetBytes(value));
        }

        /// <summary>
        /// 获取字符串的 SHA384 值
        /// </summary>
        /// <param name="value">指定字符串</param>
        /// <returns>SHA384 值</returns>
        public static byte[] SHA384Hash(string value)
        {
            return MESSAGE_DIGEST_SHA384.Value.ComputeHash(CHARSET.GetBytes(value));
        }

        /// <summary>
        /// 获取字符串的 SHA512 值
        /// </summary>
        /// <param name="value">指定字符串</param>
        /// <returns>SHA512 值</returns>
        public static byte[] SHA512Hash(string value)
        {
            return MESSAGE_DIGEST_SHA512.Value.ComputeHash(CHARSET.GetBytes(value));
        }


        /// <summary>
        /// 获取字符串的 MD5 值
        /// </summary>
        /// <param name="value">指定字符串</param>
        /// <returns>小写的 MD5 值</returns>
        public static string MD5(string value)
        {
            return new string(EncodeHex(MD5Hash(value), true));
        }

        /// <summary>
        /// 获取字符串的 SHA1 值
        /// </summary>
        /// <param name="value">指定字符串</param>
        /// <returns>小写的 SHA1 值</returns>
        public static string SHA1(string value)
        {
            return new string(EncodeHex(SHA1Hash(value), true));
        }

        /// <summary>
        /// 获取字符串的 SHA256 值
        /// </summary>
        /// <param name="value">指定字符串</param>
        /// <returns>小写的 SHA256 值</returns>
        public static string SHA256(string value)
        {
            return new string(EncodeHex(SHA256Hash(value), true));
        }

        /// <summary>
        /// 获取字符串的 SHA384 值
        /// </summary>
        /// <param name="value">指定字符串</param>
        /// <returns>小写的 SHA384 值</returns>
        public static string SHA384(string value)
        {
            return new string(EncodeHex(SHA384Hash(value), true));
        }

        /// <summary>
        /// 获取字符串的 SHA512 值
        /// </summary>
        /// <param name="value">指定字符串</param>
        /// <returns>小写的 SHA512 值</returns>
        public static string SHA512(string value)
        {
            return new string(EncodeHex(SHA512Hash(value), true));
        }
    }
}
