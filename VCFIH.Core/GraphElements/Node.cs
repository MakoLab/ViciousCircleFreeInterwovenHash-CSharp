using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCFIH.Core.GraphElements.Other;

namespace VCFIH.Core.GraphElements
{
    using RealPriorityTuple = ValueTuple<string, uint, uint, uint, uint, string>;
    using BlankPriorityTuple = ValueTuple<int, uint, uint, uint, uint, string>;
    using BlankInterwovenPriorityTuple = ValueTuple<int, uint, uint, uint, uint, string, string>;

    abstract internal class Node
    {
        internal string Identifier { get; set; }
        internal Dictionary<string, IList<Uri>> IncomingIris { get; set; } = new Dictionary<string, IList<Uri>>();
        internal Dictionary<string, IList<Uri>> IncomingBlanks { get; set; } = new Dictionary<string, IList<Uri>>();
        internal uint InDegree { get; set; }
        internal uint OutDegree { get; set; }
        internal Dictionary<string, IList<Uri>> IriNeighbours { get; set; } = new Dictionary<string, IList<Uri>>();
        internal Dictionary<string, IList<Uri>> BlankNeighbours { get; set; } = new Dictionary<string, IList<Uri>>();
        internal uint BlankInDegree { get; set; }
        internal uint BlankOutDegree { get; set; }
        internal uint TempDegree { get; set; }
        internal int StructureNumber { get; set; }
        internal int StructureLevel { get; set; }

        public bool IsBlank => this is BlankNode;

        public Node(string identifier)
        {
            Identifier = identifier;
        }

        internal void AddBlankNeighbour(Node @object, Uri predicate)
        {
            if (BlankNeighbours.ContainsKey(@object.Identifier))
            {
                BlankNeighbours[@object.Identifier].Add(predicate);
            }
            else
            {
                BlankNeighbours.Add(@object.Identifier, new List<Uri>() { predicate });
            }
            BlankOutDegree++;
            OutDegree++;
        }

        internal void AddRealNeighbour(Node @object, Uri predicate)
        {
            if (IriNeighbours.ContainsKey(@object.Identifier))
            {
                IriNeighbours[@object.Identifier].Add(predicate);
            }
            else
            {
                IriNeighbours.Add(@object.Identifier, new List<Uri>() { predicate });
            }
            OutDegree++;
        }

        internal void AddIncomingBlank(Node subject, Uri predicate)
        {
            if (IncomingBlanks.ContainsKey(subject.Identifier))
            {
                IncomingBlanks[subject.Identifier].Add(predicate);
            }
            else
            {
                IncomingBlanks.Add(subject.Identifier, new List<Uri>() { predicate });
            }
            BlankInDegree++;
            InDegree++;
        }

        internal void AddIncomingReal(Node subject, Uri predicate)
        {
            if (IncomingIris.ContainsKey(subject.Identifier))
            {
                IncomingIris[subject.Identifier].Add(predicate);
            }
            else
            {
                IncomingIris.Add(subject.Identifier, new List<Uri>() { predicate });
            }
            InDegree++;
        }

        internal RealPriorityTuple GenerateRealPriorityTuple(string predicate = "")
        {
            if (this is BlankNode)
            {
                throw new InvalidOperationException("This is method cannot be executed on a blank node object");
            }
            return (Identifier, BlankInDegree, InDegree, BlankOutDegree, OutDegree, predicate);
        }

        internal BlankPriorityTuple GenerateBlankPriorityTuple(string predicate = "")
        {
            if (this is not BlankNode)
            {
                throw new InvalidOperationException("This is method cannot be executed on a real node object");
            }
            return (StructureLevel, BlankInDegree, InDegree, BlankOutDegree, OutDegree, predicate);
        }

        internal BlankInterwovenPriorityTuple GenerateBlankInterwovenPriorityTuple(Graph g, string predicate = "")
        {
            var neighbours = "";
            var miniqueue = new PriorityQueue<(Node, Uri), RealPriorityTuple>();
            foreach (var neighbour in IncomingIris.Keys)
            {
                foreach (var pred in IncomingIris[neighbour])
                {
                    var neigh = g.IriNodes[neighbour];
                    miniqueue.Enqueue((neigh, pred), neigh.GenerateRealPriorityTuple(pred.ToString()));
                }
            }
            while (miniqueue.Count > 0)
            {
                var edge = miniqueue.Dequeue();
                neighbours += new Triple(edge.Item1, edge.Item2, this).PrepareTriple();
            }

            return (StructureLevel, BlankInDegree, InDegree, BlankOutDegree, OutDegree, neighbours, predicate);
        }

        public abstract string Translate(NodeRole role);
    }
}
