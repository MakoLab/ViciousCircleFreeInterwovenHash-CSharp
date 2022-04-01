using VCFIH.Core.Cryptography;
using VCFIH.Core.Utils;
using VCFIH.DotNetRDF;
using VDS.RDF.Parsing;

var g = new VDS.RDF.Graph();
FileLoader.Load(g, "rdf\\rdf1.ttl");
var ig = DotNetRDFReader.ReadGraph(g);
g.Dispose();
var calc = new Sha256HashCalculator(); 
var hash = ig.CalculateHash(calc);
Console.WriteLine(hash.ToHexString());
