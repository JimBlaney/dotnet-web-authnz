using System;
using System.Collections.Specialized;
using System.Web;

using Esri.Services.WebAuthnz.Principal;

using log4net;

namespace Esri.Services.WebAuthnz.Providers
{
    public abstract class EsriWebIdentityProvider<T> : IEsriWebIdentityProvider
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(T));

        public EsriWebIdentity GetIdentity(HttpContext context)
        {
            // TODO caching (optional)

            return GetIdentityImpl(context);
        }

        public abstract void Initialize(NameValueCollection properties);

        protected abstract EsriWebIdentity GetIdentityImpl(HttpContext context);

        #region Logging

        protected void Debug(object message)
        {
            log.Debug(message);
        }

        protected void DebugFormat(string format, params object[] args)
        {
            log.DebugFormat(format, args);
        }

        protected void Info(object message)
        {
            log.Info(message);
        }

        protected void InfoFormat(string format, params object[] args)
        {
            log.InfoFormat(format, args);
        }

        protected void Warn(object message)
        {
            log.Warn(message);
        }

        protected void WarnFormat(string format, params object[] args)
        {
            log.WarnFormat(format, args);
        }

        #endregion
    }
}
