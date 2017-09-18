using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
                return this["providerType"] as string;
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

        [ConfigurationProperty("setPrincipal")]
        public bool SetPrincipal
        {
            get
            {
                return bool.Parse(this["setPrincipal"].ToString());
            }
        }

        [ConfigurationProperty("clientDnHeader")]
        public string ClientDNHeader
        {
            get
            {
                return this["clientDnHeader"] as string;
            }
        }

        [ConfigurationProperty("providerSettings", IsDefaultCollection = true)]
        public ConfigNameValueCollection ProviderSettings
        {
            get
            {
                return this["providerSettings"] as ConfigNameValueCollection;
            }
            set
            {
                this["providerSettings"] = value;
            }
        }

        [ConfigurationProperty("whitelistedIPs", IsDefaultCollection = true)]
        public ConfigNameValueCollection WhitelistedIPs
        {
            get
            {
                return this["whitelistedIPs"] as ConfigNameValueCollection;
            }
            set
            {
                this["whitelistedIPs"] = value;
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

    [ConfigurationCollection(typeof(ConfigNameValueElement))]
    public class ConfigNameValueCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ConfigNameValueElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return element.ElementInformation.LineNumber;
        }

        public NameValueCollection ConvertToNVC()
        {
            ConfigurationElement[] elements = new ConfigurationElement[this.Count];
            this.CopyTo(elements, 0);

            NameValueCollection nvc = new NameValueCollection();
            foreach (ConfigurationElement element in elements)
            {
                ConfigNameValueElement cnve = (ConfigNameValueElement)element;
                nvc.Add(cnve.Name, cnve.Value);
            }

            return nvc;
        }
    }

    public class ConfigNameValueElement : ConfigurationElement
    {
        [ConfigurationProperty("key")]
        public string Name
        {
            get { return this["key"] as string; }
            set { this["key"] = value; }
        }

        [ConfigurationProperty("value")]
        public string Value
        {
            get { return this["value"] as string; }
            set { this["value"] = value; }
        }
    }
}
