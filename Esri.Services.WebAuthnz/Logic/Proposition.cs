using System;
using System.Linq;
using System.Xml.Serialization;

using Esri.Services.WebAuthnz.DataStructures;

namespace Esri.Services.WebAuthnz.Logic
{
    public class Proposition : ILogicalTruth
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("value")]
        public string Value { get; set; }

        [XmlAttribute("negate")]
        public bool Negate { get; set; }

        public bool Evaluate(ReadOnlyDictionary<string, string[]> properties)
        {
            bool present = properties.ContainsKey(Name);
            bool match = present && properties[Name].Contains(Value);

            return present && (Negate ^ match);
        }
    }
}
