using System;
using System.Security.Cryptography;
using System.Text;
using VCFIH.Core.Utils;

namespace VCFIH.Core.Cryptography;

public class Sha256HashCalculator : IHashCalculator
{
    private const string SHA256_NAME = "SHA-256";
    private const int HASH_SIZE = 256;

    public int HashSize { get => HASH_SIZE; }

    public string CalculateHash(string input)
    {
        byte[] digest = CalculateHashAsBytes(input);
        return digest.ToHexString();
    }

    public byte[] CalculateHashAsBytes(string input)
    {
        try
        {
            return SHA256.HashData(Encoding.UTF8.GetBytes(input));
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Exception occurred while calculating hash.", ex);
        }
    }

    public string CombineOrdered(string[] tup)
    {
        return CalculateHash(string.Concat(tup));
    }

    public string CombineUnordered(string[] tup)
    {
        IEnumerable<byte[]> enumerable()
        {
            foreach (var t in tup)
            {
                yield return Encoding.UTF8.GetBytes(t);
            }
        }
        return ByteArrayUtils.AddHashes(enumerable()).ToHexString();
    }
}
