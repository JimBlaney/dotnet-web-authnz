using System;
using System.Security.Principal;
using System.Web;

using Esri.Services.WebAuthnz.Config;
using Esri.Services.WebAuthnz.Principal;
using Esri.Services.WebAuthnz.Providers;

using log4net;

namespace Esri.Services.WebAuthnz.Modules
{
    public sealed class EsriWebAuthnzModule : IHttpModule
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EsriWebAuthnzModule));

        private AuthnzConfigSection config = null;
        private IEsriWebIdentityProvider identityProvider = null;

        public void Init(HttpApplication context)
        {
            context.BeginRequest += Module_BeginRequest;
            
            // load the configuration section from Web.config
            try
            {
                config = AuthnzConfigSection.GetConfig();
            }
            catch (Exception ex)
            {
                log.Error("unable to load configuration", ex);
                throw new HttpException(500, "configuration not available");
            }

            // resolve the provider type from the type's name
            Type providerType;
            try
            {
                string providerTypeName = config.ProviderType;
                providerType = Type.GetType(providerTypeName);
            }
            catch (Exception)
            {
                log.ErrorFormat("unable to resolve type from providerType '{0}'", config.ProviderType != null ? config.ProviderType : "null");
                throw new HttpException(500, "provider type not available");
            }

            // create an instance of the provider
            try
            {
                identityProvider = (IEsriWebIdentityProvider)Activator.CreateInstance(providerType);
            }
            catch (Exception)
            {
                log.ErrorFormat("unable to create instance of provider '{0}'", providerType.FullName);
                throw new HttpException(500, "provider not available");
            }

            // initialize the provider
            try
            {
                identityProvider.Initialize(config.ProviderSettings.ConvertToNVC());
            }
            catch (Exception)
            {
                log.ErrorFormat("unable to initialize provider");
                throw new HttpException(500, "provider error");
            }
        }

        void Module_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication context = (HttpApplication)sender;
            HttpRequest req = context.Request;
            HttpResponse res = context.Response;
            
            // refuse to service insecure requests if specified
            if (config.RequireHTTPS && !req.IsSecureConnection)
            {
                log.InfoFormat("{0} {1} {2} {3} {4}", req.Url.Scheme, req.HttpMethod, req.Url.PathAndQuery, "?", 403);
                throw new HttpException(403, "must use HTTPS");
            }

            // get the identity from the provider
            IIdentity identity;
            try
            {
                identity = identityProvider.GetIdentity(context);

                if (identity == null)
                {
                    log.InfoFormat("{0} {1} {2} {3} {4}", req.Url.Scheme, req.HttpMethod, req.Url.PathAndQuery, "?", 403);
                    throw new HttpException(403, "no identity found");
                }
            }
            catch (Exception ex)
            {
                log.Error("error while obtaining identity", ex);
                throw new HttpException(403, "unable to authorize with service provider");
            }

            // set a header with the client certificate's DN, if specified
            try
            {
                if (!string.IsNullOrEmpty(config.ClientDNHeader))
                {
                    req.Headers.Add(config.ClientDNHeader, req.ClientCertificate.Subject);
                }
            }
            catch (Exception)
            {
                log.Warn("did not set client DN header");
            }

            // perform the authorization check
            EsriWebIdentity esriIdentity = (EsriWebIdentity)identity;
            if (!config.IsAuthorized(esriIdentity))
            {
                throw new HttpException(403, "unauthorized");
            }

            // set the identity for the user
            log.InfoFormat("{0} {1} {2} {3} {4}", req.Url.Scheme, req.HttpMethod, req.Url.PathAndQuery, identity.Name, 200);
            HttpContext.Current.User = new GenericPrincipal(identity, null);
        }

        public void Dispose() { /* do nothing */ }
    }
}
