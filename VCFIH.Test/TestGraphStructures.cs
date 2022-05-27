using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCFIH.Core.GraphElements;
using VCFIH.Core.Utils;
using VCFIH.DotNetRDF;
using VDS.RDF.Parsing;

namespace VCFIH.Test
{
    [TestClass]
    public class TestGraphStructures
    {
        [TestMethod]
        public void TestHashesOfSameDatabaseInDifferentOrderShouldRemainSame()
        {
            var g1 = new VDS.RDF.Graph();
            FileLoader.Load(g1, "rdf\\rdf1.nt");
            var g2 = new VDS.RDF.Graph();
            FileLoader.Load(g2, "rdf\\rdf2.nt");
            var g3 = new VDS.RDF.Graph();
            FileLoader.Load(g3, "rdf\\rdf3.nt");
            var hash1 = DotNetRDFReader.CalculateHash(g1).ToHexString();
            var hash2 = DotNetRDFReader.CalculateHash(g2).ToHexString();
            var hash3 = DotNetRDFReader.CalculateHash(g3).ToHexString();
            Assert.IsNotNull(hash1);
            Assert.IsNotNull(hash2);
            Assert.IsNotNull(hash3);
            Assert.AreEqual("781305ab37fd328f57c757473181a62903cf271af976353a678a48a330fc7d6a", hash1);
            Assert.AreEqual("781305ab37fd328f57c757473181a62903cf271af976353a678a48a330fc7d6a", hash2);
            Assert.AreEqual("781305ab37fd328f57c757473181a62903cf271af976353a678a48a330fc7d6a", hash3);
        }

        [TestMethod]
        public void SameDateTimeInDifferentZoneShouldGiveSameHash()
        {
            var g1 = new Graph();
            g1.AddTriple(new IriNode("uri:id1"), new Uri("p1:pred"), new LiteralNode("2002-05-30T09:30:10+06:00", "http://www.w3.org/2001/XMLSchema#dateTime", null));
            var h1 = g1.CalculateHash().ToHexString();
            var g2 = new Graph();
            g2.AddTriple(new IriNode("uri:id1"), new Uri("p1:pred"), new LiteralNode("2002-05-30T03:30:10Z", "http://www.w3.org/2001/XMLSchema#dateTime", null));
            var h2 = g2.CalculateHash().ToHexString();
            Assert.AreEqual(h1, h2);
        }
    }
}
