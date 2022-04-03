using Microsoft.VisualStudio.TestTools.UnitTesting;
using VCFIH.Core.Utils;
using VCFIH.DotNetRDF;
using VDS.RDF.Parsing;

namespace VCFIH.Test
{
    [TestClass]
    public class TestGraphHashes
    {
        [TestMethod]
        public void SimpleGraphTest()
        {
            var g = new VDS.RDF.Graph();
            FileLoader.Load(g, "rdf\\rdf1.ttl");
            var hash = DotNetRDFReader.CalculateHash(g).ToHexString();
            Assert.IsNotNull(hash);
            Assert.AreEqual("389a4b27a8e5e693096781c8ee59df40c8d2a29f93f2e5f83e6940bd2c4533d9", hash);
        }

        [TestMethod]
        public void SimpleGraphTestWithBlankNode()
        {
            // Graphs differ only by a blank node label - should give the same hash
            var g1 = new VDS.RDF.Graph();
            FileLoader.Load(g1, "rdf\\rdf2.ttl");
            var g2 = new VDS.RDF.Graph();
            FileLoader.Load(g2, "rdf\\rdf2a.ttl");
            var g3 = new VDS.RDF.Graph();
            FileLoader.Load(g3, "rdf\\rdf2b.ttl");
            var hash1 = DotNetRDFReader.CalculateHash(g1).ToHexString();
            var hash2 = DotNetRDFReader.CalculateHash(g2).ToHexString();
            var hash3 = DotNetRDFReader.CalculateHash(g3).ToHexString();
            Assert.IsNotNull(hash1);
            Assert.IsNotNull(hash2);
            Assert.IsNotNull(hash3);
            Assert.AreEqual("ebe55e3e1064098abfbac668db24a3dedb630fbc7dbaf7e61219ca20c9459035", hash1);
            Assert.AreEqual("ebe55e3e1064098abfbac668db24a3dedb630fbc7dbaf7e61219ca20c9459035", hash2);
            Assert.AreEqual("ebe55e3e1064098abfbac668db24a3dedb630fbc7dbaf7e61219ca20c9459035", hash3);
        }

        [TestMethod]
        public void LeiGraphTest()
        {
            var g = new VDS.RDF.Graph();
            FileLoader.Load(g, "rdf\\rdf3.ttl");
            var hash = DotNetRDFReader.CalculateHash(g).ToHexString();
            Assert.IsNotNull(hash);
            Assert.AreEqual("4949f3ce018302db6299f294a3f12e1f73c83dbee5654717b209c9853945431d", hash);
        }
    }
}