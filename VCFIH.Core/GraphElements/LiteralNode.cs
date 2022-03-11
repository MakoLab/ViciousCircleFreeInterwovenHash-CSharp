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
        public LiteralNode(string identifier) : base(identifier)
        {
        }

        public override string Translate(NodeRole role)
        {
            var sb = new StringBuilder();
            sb.Append("grounded_node::role:");
            sb.Append(role == NodeRole.Subject ? "S" : "O");
            sb.Append(Identifier); // TODO: Implement literal normalization
            return sb.ToString(); ;
        }
    }
}
