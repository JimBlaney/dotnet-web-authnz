using System.Collections.Specialized;
using System.Web;

using Esri.Services.WebAuthnz.Principal;

namespace Esri.Services.WebAuthnz.Providers
{
    public interface IEsriWebIdentityProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        void Initialize(NameValueCollection properties);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">Value will never be passed in as <tt>null</tt></param>
        /// <returns></returns>
        EsriWebIdentity GetIdentity(HttpContext context);
    }
}
