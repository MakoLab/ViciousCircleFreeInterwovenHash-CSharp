using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCFIH.Core.Cryptography;
using VCFIH.Core.GraphElements;
using VDS.RDF;

namespace VCFIH.DotNetRDF
{
    public static class DotNetRDFReader
    {
        public static Core.GraphElements.Graph ReadGraph(IGraph g)
        {
            var graph = new Core.GraphElements.Graph();
            foreach (var t in g.Triples)
            {
                Node s = t.Subject.NodeType switch
                {
                    NodeType.Blank => new Core.GraphElements.BlankNode(((VDS.RDF.BlankNode)t.Subject).InternalID),
                    NodeType.Uri => new Core.GraphElements.IriNode(((VDS.RDF.UriNode)t.Subject).Uri),
                    _ => throw new InvalidDataException("Unknown node type"),
                };
                Node o = t.Object.NodeType switch
                {
                    NodeType.Blank => new Core.GraphElements.BlankNode(((VDS.RDF.BlankNode)t.Object).InternalID),
                    NodeType.Uri => new Core.GraphElements.IriNode(((VDS.RDF.UriNode)t.Object).Uri),
                    NodeType.Literal => new Core.GraphElements.LiteralNode(((VDS.RDF.LiteralNode)t.Object).Value,
                                                                           ((VDS.RDF.LiteralNode)t.Object).DataType?.ToString(),
                                                                           ((VDS.RDF.LiteralNode)t.Object).Language),
                    _ => throw new InvalidDataException("Unknown node type"),
                };
                graph.AddTriple(s, ((VDS.RDF.UriNode)t.Predicate).Uri, o);
            }
            return graph;
        }

        public static byte[] CalculateHash(IGraph g, IHashCalculator? hashCalculator = null)
        {
            if (hashCalculator == null)
            {
                return ReadGraph(g).CalculateHash();
            }
            else
            {
                return ReadGraph(g).CalculateHash(hashCalculator);
            }
        }
    }
}
