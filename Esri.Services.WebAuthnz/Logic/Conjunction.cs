using System;
using System.Linq;
using System.Xml.Serialization;

using Esri.Services.WebAuthnz.DataStructures;

namespace Esri.Services.WebAuthnz.Logic
{
    [XmlRoot("accessControl")]
    public class Conjunction : ILogicalTruthCollection
    {
        [XmlElement("and")]
        public Conjunction[] Conjunctions { get; set; }

        [XmlElement("or")]
        public Disjunction[] Disjunctions { get; set; }

        [XmlElement("prop")]
        public Proposition[] Propositions { get; set; }

        [XmlAttribute("negate")]
        public bool Negate { get; set; }

        public bool Evaluate(ReadOnlyDictionary<string, string[]> properties)
        {
            return Negate ^ ((Conjunctions != null ? Conjunctions.All(c => c.Evaluate(properties)) : true)
                          && (Disjunctions != null ? Disjunctions.All(d => d.Evaluate(properties)) : true)
                          && (Propositions != null ? Propositions.All(p => p.Evaluate(properties)) : true));
        }
    }
}
