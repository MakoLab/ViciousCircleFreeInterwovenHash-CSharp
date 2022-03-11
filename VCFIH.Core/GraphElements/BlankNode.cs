using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCFIH.Core.GraphElements.Other;

namespace VCFIH.Core.GraphElements
{
    internal class BlankNode : Node
    {
        public BlankNode(string identifier) : base(identifier)
        {
        }

        public override string Translate(NodeRole role)
        {
            var sb = new StringBuilder();
            sb.Append("blvl:");
            sb.Append(StructureLevel);
            sb.Append("::bind:");
            sb.Append(BlankInDegree);
            sb.Append("::ind:");
            sb.Append(InDegree);
            sb.Append("::boud:");
            sb.Append(BlankOutDegree);
            sb.Append(":outd:");
            sb.Append(OutDegree);
            sb.Append("::role:");
            sb.Append(role is NodeRole.Subject ? "Sblank" : "Oblank");
            return sb.ToString();
        }
    }
}
