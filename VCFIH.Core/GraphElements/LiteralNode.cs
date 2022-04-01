using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCFIH.Core.GraphElements.Other;

namespace VCFIH.Core.GraphElements
{
    public class LiteralNode : StandardNode
    {
        public LiteralNode(string value, string? datatype, string? lang)
        {
            if (datatype != null && !String.IsNullOrEmpty(lang))
            {
                throw new ArgumentException("Datatype and language cannot be present at the same time.");
            }
            Identifier = value;
            if (datatype != null)
            {
                Identifier += datatype;
            }
            if (!String.IsNullOrEmpty(lang))
            {
                Identifier += lang;
            }
            // TODO: Implement literal normalization
        }
    }
}
