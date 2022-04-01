using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCFIH.Core.Cryptography;
using VCFIH.Core.Utils;

namespace VCFIH.Core.GraphElements
{
    using RealPriorityTuple = ValueTuple<string, uint, uint, uint, uint, string>;
    using BlankPriorityTuple = ValueTuple<int, uint, uint, uint, uint, string>;
    using BlankInterwovenPriorityTuple = ValueTuple<int, uint, uint, uint, uint, string, string>;

    public class Graph
    {
        internal IList<Triple> Triples { get; set; } = new List<Triple>();
        internal Dictionary<string, BlankNode> BlankNodes { get; set; } = new Dictionary<string, BlankNode>();
        internal Dictionary<string, StandardNode> StandardNodes { get; set; } = new Dictionary<string, StandardNode>();
        internal Dictionary<string, IList<string>>? WeaklyCC { get; set; }
        internal Dictionary<int, byte[]> ComponentHashValue { get; set; } = new Dictionary<int, byte[]>();

        public byte[]? HashValue { get; set; }

        public byte[] CalculateHash(IHashCalculator hashCalculator)
        {
            var hashValueForGraph = new byte[hashCalculator.HashSize];
            WeaklyCC = TreeMarking();
            foreach (var component in WeaklyCC)
            {
                var leadNode = component.Value[0];
                PrepareSingleComponent(component.Value, preparing: true);
                var q = hashCalculator.CalculateHashAsBytes(PrepareSingleComponent(component.Value, preparing: false));
                if (!ComponentHashValue.ContainsKey(BlankNodes[leadNode].StructureNumber))
                {
                    ComponentHashValue.Add(BlankNodes[leadNode].StructureNumber, q);
                }
                else
                {
                    ComponentHashValue[BlankNodes[leadNode].StructureNumber] = q;
                }
                hashValueForGraph = ByteArrayUtils.AddHashes(hashValueForGraph, q);
            }

            foreach (var t in Triples)
            {
                if (t.Subject.IsBlank && t.Object.IsBlank)
                    continue;
                else
                {
                    var q = hashCalculator.CalculateHashAsBytes(t.PrepareTriple());
                    hashValueForGraph = ByteArrayUtils.AddHashes(hashValueForGraph, q);
                }
            }
            HashValue = hashValueForGraph;
            return HashValue;
        }

        public bool ContainsBlankNode(BlankNode bn) => BlankNodes.ContainsKey(bn.Identifier);

        public bool ContainsIriNode(StandardNode iNode) => StandardNodes.ContainsKey(iNode.Identifier);

        public void AddTriple(Node s, Uri p, Node o)
        {
            if (o is BlankNode bnode)
            {
                if (!BlankNodes.ContainsKey(o.Identifier))
                {
                    BlankNodes.Add(o.Identifier, bnode);
                }
                o = BlankNodes[o.Identifier];
            }
            else
            {
                if (!StandardNodes.ContainsKey(o.Identifier))
                {
                    StandardNodes.Add(o.Identifier, (StandardNode)o);
                }
                o = StandardNodes[o.Identifier];
            }

            if (s is BlankNode)
            {
                if (!BlankNodes.ContainsKey(s.Identifier))
                {
                    BlankNodes.Add(s.Identifier, (BlankNode)o);
                }
                s = BlankNodes[s.Identifier];
            }
            else
            {
                if (!StandardNodes.ContainsKey(s.Identifier))
                {
                    StandardNodes.Add(s.Identifier, (StandardNode)o);
                }
                s = StandardNodes[s.Identifier];
            }

            if (o is BlankNode)
            {
                s.AddBlankNeighbour(o, p);
            }
            else
            {
                s.AddRealNeighbour(o, p);
            }
            if (s is BlankNode)
            {
                o.AddIncomingBlank(s, p);
            }
            else
            {
                o.AddIncomingReal(s, p);
            }

            Triples.Add(new Triple(s, p, o));
        }

        public void HashIncrementTriple(Triple newTriple)
        {
            throw new NotImplementedException();

            //var caseNumber = 0;
            //var blankNumber = 0;
            //if (newTriple.Subject is BlankNode s)
            //{
            //    blankNumber++;
            //    if (ContainsBlankNode(s))
            //    {
            //        caseNumber++;
            //    }
            //}
            //if (newTriple.Object is BlankNode o)
            //{
            //    blankNumber += 2;
            //    if (ContainsBlankNode(o))
            //    {
            //        caseNumber += 2;
            //    }
            //}
            //var triplesToBeRehashed = new List<Triple>();
            //switch (caseNumber)
            //{
            //    case 1:
            //        foreach (var t in Triples)
            //        {
            //            if ((t.Subject.Identifier == newTriple.Subject.Identifier && !t.Object.IsBlank)
            //                || (t.Object.Identifier == newTriple.Subject.Identifier && !t.Subject.IsBlank))
            //            {
            //                triplesToBeRehashed.Add(t);
            //            }
            //        }
            //        break;
            //    case 2:
            //        foreach (var t in Triples)
            //        {
            //            if ((t.Subject.Identifier == newTriple.Object.Identifier && !t.Object.IsBlank)
            //                || (t.Object.Identifier == newTriple.Object.Identifier && !t.Subject.IsBlank))
            //            {
            //                triplesToBeRehashed.Add(t);
            //            }
            //        }
            //        break;
            //    case 3:
            //        foreach (var t in Triples)
            //        {
            //            if ((t.Subject.Identifier == newTriple.Subject.Identifier && !t.Object.IsBlank)
            //              || (t.Object.Identifier == newTriple.Subject.Identifier && !t.Subject.IsBlank)
            //             || (t.Subject.Identifier == newTriple.Object.Identifier  && !t.Object.IsBlank)
            //              || (t.Object.Identifier == newTriple.Object.Identifier  && !t.Subject.IsBlank))
            //            {
            //                triplesToBeRehashed.Add(t);
            //            }
            //        }
            //        break;
            //}
            //byte[] totalityToSubtract;
            //if (caseNumber != 0)
            //{
            //    foreach (var t in triplesToBeRehashed)
            //    {
                    
            //    }
            //}
        }

        public Dictionary<string, IList<string>> TreeMarking()
        {
            Dictionary<string, string> parent = new();
            foreach (var e in BlankNodes)
            {
                parent.Add(e.Key, e.Key);
            }
            List<string[]> edges = new();
            foreach (var e in BlankNodes)
            {
                foreach (var n in e.Value.BlankNeighbours.Keys)
                {
                    edges.Add(new string[] { e.Value.Identifier, n });
                }
            }
            foreach (var e in edges)
            {
                parent[Merge(parent, e[0])] = Merge(parent, e[1]);
            }

            var numberOfComponents = 0;
            foreach (var p in parent)
            {
                numberOfComponents += p.Key == p.Value ? 1 : 0;
            }
            foreach(var p in parent)
            {
                parent[p.Key] = Merge(parent, p.Value);
            }

            var result = new Dictionary<string, IList<string>>();
            foreach(var p in parent)
            {
                if (result.ContainsKey(p.Value))
                {
                    result[p.Value].Add(p.Key);
                }
                else
                {
                    result[p.Value] = new List<string>() { p.Key };
                }
            }

            var i = 0;
            foreach (var r in result)
            {
                foreach (var n in r.Value)
                {
                    BlankNodes[n].StructureNumber = i;
                }
                i++;
            }

            return result;
        }

        public string PrepareSingleComponent(IList<string> component, bool preparing)
        {
            var valueForComponent = "";
            var priorityQueue = new PriorityQueue<Node, BlankInterwovenPriorityTuple>();
            foreach (var node in component)
            {
                BlankNodes[node].TempDegree = BlankNodes[node].BlankInDegree;
                if (BlankNodes[node].BlankInDegree == 0)
                {
                    priorityQueue.Enqueue(BlankNodes[node], BlankNodes[node].GenerateBlankInterwovenPriorityTuple(this));
                    BlankNodes[node].StructureLevel = 0;
                }
            }
            while (priorityQueue.Count > 0)
            {
                var node = priorityQueue.Dequeue();

                // Get all edges coming out of node for hashing

                if (!preparing)
                {
                    var miniqueue1 = new PriorityQueue<Node, BlankInterwovenPriorityTuple>();
                    foreach (var neighbour in node.BlankNeighbours.Keys)
                    {
                        miniqueue1.Enqueue(BlankNodes[neighbour], BlankNodes[neighbour].GenerateBlankInterwovenPriorityTuple(this));
                    }
                    while (miniqueue1.Count > 0)
                    {
                        var neigh = miniqueue1.Dequeue();
                        foreach (var predicate in node.BlankNeighbours[neigh.Identifier].OrderBy(uri => uri.ToString()))
                        {
                            valueForComponent += new Triple(node, predicate, neigh).PrepareTriple();
                        }
                    }
                }

                // Proceed with handling subsequent parts of our DAG.

                foreach (var neighbour in node.BlankNeighbours.Keys)
                {
                    BlankNodes[neighbour].TempDegree -= (uint)node.BlankNeighbours[neighbour].Count;
                    if (BlankNodes[neighbour].TempDegree == 0)
                    {
                        if (preparing)
                        {
                            BlankNodes[neighbour].StructureLevel = node.StructureLevel + 1;
                        }
                        priorityQueue.Enqueue(BlankNodes[neighbour], BlankNodes[neighbour].GenerateBlankInterwovenPriorityTuple(this));
                    }
                }
            }
            return valueForComponent;
        }

        private static string Merge(Dictionary<string, string> parent, string x)
        {
            if (parent[x] == x)
            {
                return x;
            }
            else
            {
                return Merge(parent, parent[x]);
            }
        }
    }
}
