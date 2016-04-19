using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

using Esri.Services.WebAuthnz.DataStructures;
using Esri.Services.WebAuthnz.Logic;
using Esri.Services.WebAuthnz.Principal;

namespace Esri.Services.WebAuthnz.Config
{
    public sealed class AuthnzConfigSection : ConfigurationSection
    {
        public static AuthnzConfigSection GetConfig()
        {
            string sectionName = Assembly.GetExecutingAssembly().GetName().Name;

            return ConfigurationManager.GetSection(sectionName) as AuthnzConfigSection;
        }

        [ConfigurationProperty("providerType", IsRequired = true)]
        public string ProviderType
        {
            get
            {
                return this["providerType"].ToString();
            }
        }

        [ConfigurationProperty("requireHTTPS")]
        public bool RequireHTTPS
        {
            get
            {
                return bool.Parse(this["requireHTTPS"].ToString());
            }
        }

        public Conjunction AccessControl { get; set; }

        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            if (elementName == "accessControl")
            {
                XmlSerializer ser = new XmlSerializer(typeof(Conjunction));
                this.AccessControl = ser.Deserialize(reader) as Conjunction;

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsAuthorized(EsriWebIdentity esriIdentity)
        {
            Dictionary<string, string[]> dict = new Dictionary<string, string[]>();
            foreach (string key in esriIdentity.Keys)
            {
                dict.Add(key, esriIdentity[key]);
            }

            return this.AccessControl != null && this.AccessControl.Evaluate(new ReadOnlyDictionary<string, string[]>(dict));
        }
    }
}
