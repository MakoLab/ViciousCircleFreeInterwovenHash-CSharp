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
            Assert.AreEqual("495e88dfba09abdf20c3d3377af5d7338c1c9b5898d1932f4c5f9591359b774d", hash);
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
            Assert.AreEqual("46596a5ee27ade4659bb40fb2d34c84ee88ee0b46830333472c9109371b0ea67", hash1);
            Assert.AreEqual("46596a5ee27ade4659bb40fb2d34c84ee88ee0b46830333472c9109371b0ea67", hash2);
            Assert.AreEqual("46596a5ee27ade4659bb40fb2d34c84ee88ee0b46830333472c9109371b0ea67", hash3);
        }

        [TestMethod]
        public void LeiGraphTest()
        {
            var g = new VDS.RDF.Graph();
            FileLoader.Load(g, "rdf\\rdf3.ttl");
            var hash = DotNetRDFReader.CalculateHash(g).ToHexString();
            Assert.IsNotNull(hash);
            Assert.AreEqual("105b977a67544242e503de5b6da15e8b890fe8681e2ca10805eeeedf7d73f86b", hash);
        }
    }
}