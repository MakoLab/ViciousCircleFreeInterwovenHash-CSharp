namespace VCFIH.Core.Cryptography;

public interface IHashCalculator
{
    public int HashSize { get; }
    string CalculateHash(string input);
    byte[] CalculateHashAsBytes(string input);

    string CombineOrdered(string[] tup);

    string CombineUnordered(string[] tup);
}
