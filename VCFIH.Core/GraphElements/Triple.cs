using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCFIH.Core.GraphElements.Other;

namespace VCFIH.Core.GraphElements
{
    public class Triple
    {
        public Node Subject { get; set; }
        public Uri Predicate { get; set; }
        public Node Object { get; set; }

        public Triple(Node s, Uri p, Node o)
        {
            Subject = s;
            Predicate = p;
            Object = o;
        }

        public string PrepareTriple()
        {
            string conversionValue, sub, obj;

            sub = Subject.Translate(NodeRole.Subject);
            conversionValue = sub;
            conversionValue += Predicate.ToString();
            obj = Object.Translate(NodeRole.Object);
            conversionValue += obj;
            
            return conversionValue;
        }
    }
}
