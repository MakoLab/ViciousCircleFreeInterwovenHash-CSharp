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

        public static byte[] AddHashes(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
            {
                throw new ArgumentException("Array lenghts must be the same.");
            }
            ushort sum = 0;
            var result = new byte[a.Length];
            for (var i = a.Length - 1; i >= 0; i--)
            {
                sum += (ushort)(a[i] + b[i]);
                result[i] = (byte)(sum & 0x00FF);
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
