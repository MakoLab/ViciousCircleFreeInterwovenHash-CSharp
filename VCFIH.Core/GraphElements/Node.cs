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

    public abstract class Node
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

        internal Node()
        {
            Identifier = "";
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
            if (this is not BlankNode)
            {
                throw new InvalidOperationException("This is method cannot be executed on a real node object");
            }

            var neighbours = "";

            // Adding real incoming neighbours to the hash material in predefined order

            var miniqueue1 = new PriorityQueue<(Node, Uri), RealPriorityTuple>();
            foreach (var neighbour in IncomingIris.Keys)
            {
                foreach (var pred in IncomingIris[neighbour])
                {
                    var neigh = g.StandardNodes[neighbour];
                    miniqueue1.Enqueue((neigh, pred), neigh.GenerateRealPriorityTuple(pred.ToString()));
                }
            }
            while (miniqueue1.Count > 0)
            {
                var edge = miniqueue1.Dequeue();
                neighbours += new Triple(edge.Item1, edge.Item2, this).PrepareTriple();
            }

            // Adding blank incoming neighbours to the hash material in predefined order
            var miniqueue2 = new PriorityQueue<(Node, Uri), BlankPriorityTuple>();
            foreach (var neighbour in IncomingBlanks.Keys)
            {
                foreach (var pred in IncomingBlanks[neighbour])
                {
                    var neigh = g.BlankNodes[neighbour];
                    miniqueue2.Enqueue((neigh, pred), neigh.GenerateBlankPriorityTuple(pred.ToString()));
                }
            }
            while (miniqueue2.Count > 0)
            {
                var edge = miniqueue2.Dequeue();
                neighbours += new Triple(edge.Item1, edge.Item2, this).PrepareTriple();
            }

            // Adding real neighbours to the hash material in predefined order

            var miniqueue3 = new PriorityQueue<(Node, Uri), RealPriorityTuple>();
            foreach (var neighbour in IriNeighbours.Keys)
            {
                foreach (var pred in IriNeighbours[neighbour])
                {
                    var neigh = g.StandardNodes[neighbour];
                    miniqueue3.Enqueue((neigh, pred), neigh.GenerateRealPriorityTuple(pred.ToString()));
                }
            }
            while (miniqueue3.Count > 0)
            {
                var edge = miniqueue3.Dequeue();
                neighbours += new Triple(this, edge.Item2, edge.Item1).PrepareTriple();
            }

            // Adding blank neighbours to the hash material in predefined order

            var miniqueue4 = new PriorityQueue<(Node, Uri), BlankPriorityTuple>();
            foreach (var neighbour in BlankNeighbours.Keys)
            {
                foreach (var pred in BlankNeighbours[neighbour])
                {
                    var neigh = g.BlankNodes[neighbour];
                    miniqueue4.Enqueue((neigh, pred), neigh.GenerateBlankPriorityTuple(pred.ToString()));
                }
            }
            while (miniqueue4.Count > 0)
            {
                var edge = miniqueue4.Dequeue();
                neighbours += new Triple(this, edge.Item2, edge.Item1).PrepareTriple();
            }

            return (StructureLevel, BlankInDegree, InDegree, BlankOutDegree, OutDegree, neighbours, predicate);
        }

        public abstract string Translate(NodeRole role);
    }
}
