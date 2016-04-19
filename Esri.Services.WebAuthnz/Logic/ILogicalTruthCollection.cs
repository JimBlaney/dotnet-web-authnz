using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esri.Services.WebAuthnz.Logic
{
    public interface ILogicalTruthCollection : ILogicalTruth
    {
        Conjunction[] Conjunctions { get; set; }

        Disjunction[] Disjunctions { get; set; }

        Proposition[] Propositions { get; set; }
    }
}
