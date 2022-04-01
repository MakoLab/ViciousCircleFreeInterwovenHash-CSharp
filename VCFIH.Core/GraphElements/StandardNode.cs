using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCFIH.Core.GraphElements.Other;

namespace VCFIH.Core.GraphElements
{
    public abstract class StandardNode : Node
    {
        public override string Translate(NodeRole role)
        {
            var sb = new StringBuilder();
            sb.Append("grounded_node::role:");
            sb.Append(role == NodeRole.Subject ? "S" : "O");
            sb.Append("::name:");
            sb.Append(Identifier);
            return sb.ToString();
        }
    }
}
