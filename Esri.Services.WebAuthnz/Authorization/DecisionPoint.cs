using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Esri.Services.WebAuthnz.DataStructures;
using Esri.Services.WebAuthnz.Logic;
using Esri.Services.WebAuthnz.Principal;


namespace Esri.Services.WebAuthnz.Authorization
{
    internal sealed class DecisionPoint
    {
        public Conjunction AccessControl { get; private set; }

        public DecisionPoint(Conjunction accessControl)
        {
            this.AccessControl = accessControl;
        }

        public Decision Decide(EsriWebIdentity esriIdentity)
        {
            Evidence evidence = new Evidence();

            Dictionary<string, string[]> dict = new Dictionary<string, string[]>();
            foreach (string key in esriIdentity.Keys)
            {
                dict.Add(key, esriIdentity[key]);
            }
            
            // TODO implement chain of evidence reporting for decision

            // NOTE this implementation still correctly evaluates the decision

            bool isAuthorized = this.AccessControl != null && this.AccessControl.Evaluate(new ReadOnlyDictionary<string, string[]>(dict));
            
            return new Decision(isAuthorized, evidence);
        }
    }
}
