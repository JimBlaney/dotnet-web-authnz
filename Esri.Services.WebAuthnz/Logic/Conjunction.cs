using System;
using System.Collections.Generic;
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

        public override string ToString()
        {
            List<string> tokens = new List<string>();

            foreach (Conjunction c in Conjunctions)
            {
                tokens.Add(c.ToString());
            }

            foreach (Disjunction d in Disjunctions)
            {
                tokens.Add(d.ToString());
            }

            foreach (Proposition p in Propositions)
            {
                tokens.Add(p.ToString());
            }

            return String.Format("({0})", String.Join(") AND (", tokens.ToArray()));
        }
    }
}
