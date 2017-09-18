using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

using Esri.Services.WebAuthnz.Principal;

using log4net;

namespace Esri.Services.WebAuthnz.Providers.Impl
{
    public class CommonNameIdentityProvider : EsriWebIdentityProvider
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EsriWebIdentityProvider));

        private int _maxLength = 64; // max length of CN per X.500 specification

        public override void Initialize(NameValueCollection properties)
        {
            string tmp = properties["CN_MAX_LENGTH"];
            if (!string.IsNullOrEmpty(tmp))
            {
                if (!int.TryParse(tmp, out _maxLength))
                {
                    log.WarnFormat("could not parse value for CN_MAX_LENGTH -- using default of {0}", _maxLength);
                }
            }
        }

        protected override EsriWebIdentity GetIdentityImpl(HttpApplication context)
        {
            EsriWebIdentity identity = null;

            try
            {
                string dn = context.Request.ClientCertificate.Subject;
                if (string.IsNullOrEmpty(dn))
                {
                    throw new Exception("client certificate subject was null or empty");
                }

                log.DebugFormat("distinguished name: '{0}'", dn);

                string[] tokens = dn.Split(new char[] { ',', '/' });
                string cn = (from token in tokens where token.TrimStart().ToUpper().StartsWith("CN") select token.Split('=').LastOrDefault()).FirstOrDefault();

                log.DebugFormat("common name parsed: '{0}'", cn);

                if (string.IsNullOrEmpty(cn))
                {
                    throw new Exception("could not parse common name from distinguished name");
                }

                cn = cn.Replace(' ', '_');

                Dictionary < string, string[]> attributes = new Dictionary<string, string[]>();
                //attributes.Add("AWESOME", new string[] { "VERY" });

                identity = new EsriWebIdentity(cn, attributes, this.GetType().Name);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return identity;
        }
    }
}
