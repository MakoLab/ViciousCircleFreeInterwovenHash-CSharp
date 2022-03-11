using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCFIH.Core.Cryptography;
using VCFIH.Core.Utils;

namespace VCFIH.Core.GraphElements
{
    internal class Graph
    {
        internal IHashCalculator hashCalculator;
        internal IList<Triple> Triples { get; set; } = new List<Triple>();
        internal Dictionary<string, BlankNode> BlankNodes { get; set; } = new Dictionary<string, BlankNode>();
        internal Dictionary<string, IriNode> IriNodes { get; set; } = new Dictionary<string, IriNode>();

        public byte[] HashValue { get; set; }

        public Graph(IHashCalculator hashCalculator)
        {
            if (hashCalculator is null)
            {
                throw new ArgumentNullException(nameof(hashCalculator));
            }
            this.hashCalculator = hashCalculator;
            HashValue = new byte[hashCalculator.HashSize];
        }

        public bool ContainsBlankNode(BlankNode bn) => BlankNodes.ContainsKey(bn.Identifier);

        public bool ContainsIriNode(IriNode iNode) => IriNodes.ContainsKey(iNode.Identifier);

        public void AddTriple(Triple triple)
        {
            var o = triple.Object;
            if (o is BlankNode)
            {
                if (!BlankNodes.ContainsKey(o.Identifier))
                {
                    BlankNodes.Add(o.Identifier, (BlankNode)o);
                }
                o = BlankNodes[o.Identifier];
            }
            else
            {
                if (!IriNodes.ContainsKey(o.Identifier))
                {
                    IriNodes.Add(o.Identifier, (IriNode)o);
                }
                o = IriNodes[o.Identifier];
            }

            var s = triple.Subject;
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
                if (!IriNodes.ContainsKey(s.Identifier))
                {
                    IriNodes.Add(s.Identifier, (IriNode)o);
                }
                s = IriNodes[s.Identifier];
            }

            if (o is BlankNode)
            {
                s.AddBlankNeighbour(o, triple.Predicate);
            }
            else
            {
                s.AddRealNeighbour(o, triple.Predicate);
            }
            if (s is BlankNode)
            {
                o.AddIncomingBlank(s, triple.Predicate);
            }
            else
            {
                o.AddIncomingReal(s, triple.Predicate);
            }
        }

        public void HashIncrementTriple(Triple newTriple)
        {
            var caseNumber = 0;
            var blankNumber = 0;
            if (newTriple.Subject is BlankNode s)
            {
                blankNumber++;
                if (ContainsBlankNode(s))
                {
                    caseNumber++;
                }
            }
            if (newTriple.Object is BlankNode o)
            {
                blankNumber += 2;
                if (ContainsBlankNode(o))
                {
                    caseNumber += 2;
                }
            }
            var triplesToBeRehashed = new List<Triple>();
            switch (caseNumber)
            {
                case 1:
                    foreach (var t in Triples)
                    {
                        if ((t.Subject.Identifier == newTriple.Subject.Identifier && !t.Object.IsBlank)
                            || (t.Object.Identifier == newTriple.Subject.Identifier && !t.Subject.IsBlank))
                        {
                            triplesToBeRehashed.Add(t);
                        }
                    }
                    break;
                case 2:
                    foreach (var t in Triples)
                    {
                        if ((t.Subject.Identifier == newTriple.Object.Identifier && !t.Object.IsBlank)
                            || (t.Object.Identifier == newTriple.Object.Identifier && !t.Subject.IsBlank))
                        {
                            triplesToBeRehashed.Add(t);
                        }
                    }
                    break;
                case 3:
                    foreach (var t in Triples)
                    {
                        if ((t.Subject.Identifier == newTriple.Subject.Identifier && !t.Object.IsBlank)
                          || (t.Object.Identifier == newTriple.Subject.Identifier && !t.Subject.IsBlank)
                         || (t.Subject.Identifier == newTriple.Object.Identifier  && !t.Object.IsBlank)
                          || (t.Object.Identifier == newTriple.Object.Identifier  && !t.Subject.IsBlank))
                        {
                            triplesToBeRehashed.Add(t);
                        }
                    }
                    break;
            }
            byte[] totalityToSubtract;
            if (caseNumber != 0)
            {
                foreach (var t in triplesToBeRehashed)
                {
                    
                }
            }
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
                i++;
                foreach (var n in r.Value)
                {
                    BlankNodes[n].StructureNumber = i;
                }
            }

            return result;
        }

        public string PrepareSingleComponent(IList<string> component, bool preparing)
        {
            var valueForComponent = "";
            var priorityQueue = new PriorityQueue<Node, int>();
            
        }

        private string Merge(Dictionary<string, string> parent, string x)
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
