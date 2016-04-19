using System;
using System.Linq;
using System.Xml.Serialization;

using Esri.Services.WebAuthnz.DataStructures;

namespace Esri.Services.WebAuthnz.Logic
{
    public class Disjunction : ILogicalTruthCollection
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
            return Negate ^ ((Conjunctions != null ? Conjunctions.Any(c => c.Evaluate(properties)) : false)
                          || (Disjunctions != null ? Disjunctions.Any(d => d.Evaluate(properties)) : false)
                          || (Propositions != null ? Propositions.Any(p => p.Evaluate(properties)) : false));
        }
    }
}
