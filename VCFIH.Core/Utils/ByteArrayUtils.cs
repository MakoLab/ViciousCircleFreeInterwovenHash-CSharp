using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCFIH.Core.Utils
{
    public static class ByteArrayUtils
    {
        public static string ToHexString(this byte[] bytes)
        {
            char[] c = new char[bytes.Length * 2];
            int b;
            for (int i = 0; i < bytes.Length; i++)
            {
                b = bytes[i] >> 4;
                c[i * 2] = (char)(87 + b + (((b - 10) >> 31) & -39));
                b = bytes[i] & 0xF;
                c[i * 2 + 1] = (char)(87 + b + (((b - 10) >> 31) & -39));
            }
            return new string(c);
        }

        public static byte[] ToByteArray(this string hexString)
        {
            if (hexString.Length % 2 == 1)
            {
                throw new ArgumentException("Hex string cannot have odd number of digits.");
            }
            var byteArrLength = hexString.Length >> 1;
            var result = new byte[byteArrLength];
            int val, upper, res;
            for (var i = 0; i < byteArrLength; i++)
            {
                val = hexString[i << 1];
                upper = val + (((96 - val) >> 31) & -32);
                res = (((64 - upper) >> 31) & -7) + upper - 48;
                if (res < 0 || res > 15)
                {
                    throw new ArgumentOutOfRangeException($"Char {hexString[i << 1]} is not a proper hex char.");
                }
                result[i] = (byte)(res << 4);
                val = hexString[(i << 1) + 1];
                upper = val + (((96 - val) >> 31) & -32);
                res = (((64 - upper) >> 31) & -7) + upper - 48;
                if (res < 0 || res > 15)
                {
                    throw new ArgumentOutOfRangeException($"Char {hexString[(i << 1) + 1]} is not a proper hex char.");
                }
                result[i] |= (byte)res;
            }
            return result;
        }

        public static byte[] AddHashes(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
            {
                throw new ArgumentException("Array lengths must be equal.");
            }
            int sum = 0;
            var result = new byte[a.Length];
            for (var i = a.Length - 1; i >= 0; i--)
            {
                sum += a[i] + b[i];
                result[i] = (byte)(sum & 0x000000FF);
                sum >>= 8;
            }
            return result;
        }

        public static byte[] AddHashes(IEnumerable<byte[]> hashes)
        {
            if (!hashes.Any())
            {
                throw new ArgumentException("Hash collection is empty");
            }
            byte[] sum = new byte[hashes.First().Length];
            foreach (var hash in hashes)
            {
                sum = AddHashes(sum, hash);
            }
            return sum;
        }
    }
}
