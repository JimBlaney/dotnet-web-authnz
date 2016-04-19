using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace Esri.Services.WebAuthnz.Principal
{
    public sealed class EsriWebIdentity : GenericIdentity
    {
        private readonly Dictionary<string, string[]> _attributes;

        public EsriWebIdentity(string name, Dictionary<string, string[]> attributes = null, string providerName = "?")
            : base(name, string.Format("Esri.Services.WebAuthnz/{0}", providerName))
        {
            this._attributes = attributes;
        }

        public string[] Keys
        {
            get
            {
                string[] keys = new string[this._attributes.Keys.Count];

                this._attributes.Keys.CopyTo(keys, 0);

                return keys;
            }
        }

        public string[] this[string attributeName]
        {
            get
            {
                string[] returnValue = null;

                if (this._attributes != null && this._attributes.ContainsKey(attributeName))
                {
                    string[] tempArray = this._attributes[attributeName];

                    returnValue = new string[tempArray.Length];
                    Array.Copy(tempArray, returnValue, returnValue.Length);
                }

                return returnValue;
            }
        }
    }
}
