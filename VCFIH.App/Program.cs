using VCFIH.DotNetRDF;
using VDS.RDF.Parsing;

var g = new VDS.RDF.Graph();
FileLoader.Load(g, "rdf\\rdf1.ttl");
var ig = DotNetRDFReader.ReadGraph(g);
Console.WriteLine(g);
