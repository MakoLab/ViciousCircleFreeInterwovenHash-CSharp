using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VCFIH.Core.Utils;

namespace VCFIH.Test
{
    [TestClass]
    public class TestByteArrayUtils
    {
        [TestMethod]
        [DataRow("test1", "1b4f0e9851971998e732078544c96b36c3d01cedf7caa332359d6f1d83567014")]
        [DataRow("test2", "60303ae22b998861bce3b28f33eec1be758a213c86c93c076dbe9f558c11c752")]
        [DataRow("test3", "fd61a03af4f77d870fc21e05e7e80678095c92d808cfb3b5c279ee04c74aca13")]
        [DataRow("test4", "a4e624d686e03ed2767c0abd85c14426b0b1157d2ce81d27bb4fe4f6f01d688a")]
        [DataRow("test5", "a140c0c1eda2def2b830363ba362aa4d7d255c262960544821f556e16661b6ff")]
        public void ShouldGiveProperHexString(string input, string hashString)
        {
            var arr = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            var str = arr.ToHexString();
            Assert.AreEqual(hashString, str);
        }

        [TestMethod]
        [DataRow("1b4f", 27, 79)]
        [DataRow("6030", 96, 48)]
        [DataRow("f9aa", 249, 170)]
        public void ShouldGiveProperByteArray(string input, int first, int second)
        {
            var arr = input.ToByteArray();
            Assert.AreEqual(first, arr[0]);
            Assert.AreEqual(second, arr[1]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ShouldThrowException_NotHexChar()
        {
            "6j303ae22b998861bce3b28f33eec1be758a213c86c93c076dbe9f558c11c752".ToByteArray();
        }

        [TestMethod]
        [DataRow("01010101", "02020202")]
        [DataRow("fafafafa", "f5f5f5f4")]
        [DataRow("7cd565c5", "f9aacb8a")]
        public void ShouldAddTwoByteArrays(string addend, string expectedSum)
        {
            var arr1 = addend.ToByteArray();
            var arr2 = addend.ToByteArray();
            var sum = ByteArrayUtils.AddHashes(arr1, arr2);
            Assert.IsTrue(sum.SequenceEqual(expectedSum.ToByteArray()));
        }
    }
}
