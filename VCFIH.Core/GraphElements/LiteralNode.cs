using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCFIH.Core.GraphElements.Other;

namespace VCFIH.Core.GraphElements
{
    internal class LiteralNode : Node
    {
        public LiteralNode(string value, string? datatype, string? lang)
        {
            if (datatype != null && lang != null)
            {
                throw new ArgumentException("Datatype and language cannot be present at the same time.");
            }
            Identifier = value;
            if (datatype != null)
            {
                Identifier += datatype;
            }
            if (lang != null)
            {
                Identifier += lang;
            }
            // TODO: Implement literal normalization
        }

        public override string Translate(NodeRole role)
        {
            var sb = new StringBuilder();
            sb.Append("grounded_node::role:");
            sb.Append(role == NodeRole.Subject ? "S" : "O");
            sb.Append(Identifier);
            return sb.ToString();
        }
    }
}
