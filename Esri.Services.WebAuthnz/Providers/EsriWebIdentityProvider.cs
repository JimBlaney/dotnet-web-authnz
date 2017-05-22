using System;
using System.Collections.Specialized;
using System.Web;

using Esri.Services.WebAuthnz.Principal;

using log4net;

namespace Esri.Services.WebAuthnz.Providers
{
    public abstract class EsriWebIdentityProvider : IEsriWebIdentityProvider
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EsriWebIdentityProvider));

        public EsriWebIdentity GetIdentity(HttpApplication context)
        {
            // TODO caching (optional)

            return GetIdentityImpl(context);
        }

        public abstract void Initialize(NameValueCollection properties);

        protected abstract EsriWebIdentity GetIdentityImpl(HttpApplication context);
    }
}
