using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VCFIH.Core.GraphElements.Other;
using VCFIH.Core.Utils;

namespace VCFIH.Core.GraphElements
{
    public class LiteralNode : StandardNode
    {
        private static readonly string dateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffK";

        public LiteralNode(string value, string? datatype, string? lang)
        {
            if (datatype != null)
            {
                switch (datatype)
                {
                    case "http://www.w3.org/2001/XMLSchema#dateTime":
                    case "http://www.w3.org/2001/XMLSchema#dateTimeStamp":
                        var nDate = DateTime.Parse(value).ToUniversalTime();
                        value = nDate.ToString(dateTimeFormat);
                        break;
                    case "http://www.w3.org/2001/XMLSchema#decimal":
                        var nDecimal = Double.Parse(value, CultureInfo.InvariantCulture);
                        value = nDecimal.ToString();
                        break;
                    case "http://www.w3.org/2001/XMLSchema#string":
                        datatype = "";
                        break;
                }
            }
            value = Regex.Replace(value, @"\p{C}", "").Normalize(NormalizationForm.FormC);
            value = EscapeN3(value);
            if (lang != null && !String.IsNullOrWhiteSpace(lang))
            {
                value += lang;
            }
            else
            {
                if (datatype != null && !String.IsNullOrWhiteSpace(datatype))
                {
                    value += datatype;
                }
            }
            Identifier = SHA256.HashData(Encoding.UTF8.GetBytes(value)).ToHexString();
        }

        private static string EscapeN3(string s)
        {
            return s.Replace("\\", "\\\\")
                    .Replace("\"", "\\\"")
                    .Replace("\t", "\\t")
                    .Replace("\r", "\\r")
                    .Replace("\n", "\\n")
                    .Replace("\b", "\\b")
                    .Replace("\f", "\\f");
        }
    }
}
