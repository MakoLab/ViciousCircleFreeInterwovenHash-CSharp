using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCFIH.Core.GraphElements
{
    public class IriNode : StandardNode
    {
        public IriNode(string identifier)
        {
            Identifier = identifier;
        }

        public IriNode(Uri identifier)
        {
            Identifier = identifier.ToString();
        }
    }
}
