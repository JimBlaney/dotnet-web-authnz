using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;

using Esri.Services.WebAuthnz.Authorization;
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

        private DecisionPoint decisionPoint = null;

        private List<String> whitelist = new List<String>();

        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += Module_AuthenticateRequest;
            context.EndRequest += Module_EndRequest;
            
            // load the configuration section from Web.config
            try
            {
                config = AuthnzConfigSection.GetConfig();

                decisionPoint = new DecisionPoint(config.AccessControl);

                Type providerType = Type.GetType(config.ProviderType);
                
                identityProvider = (IEsriWebIdentityProvider)Activator.CreateInstance(providerType);
                identityProvider.Initialize(config.ProviderSettings.ConvertToNVC());

                if (config.WhitelistedIPs != null)
                {
                    NameValueCollection nvc = config.WhitelistedIPs.ConvertToNVC();
                    foreach (String key in nvc.Keys)
                    {
                        whitelist.Add(nvc[key]);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("unable to create instance of provider", ex);
            }
        }

        void Module_AuthenticateRequest(object sender, EventArgs e)
        {
            HttpApplication context = (HttpApplication)sender;
            HttpRequest req = context.Request;
            
            if (config == null)
            {
                throw new HttpException(500, "configuration not available");
            }
            
            if (identityProvider == null)
            {
                throw new HttpException(500, "identity provider not available");
            }
            
            // refuse to service insecure requests if specified
            if (config.RequireHTTPS && !req.IsSecureConnection)
            {
                LogEvent(req, "?", 403);
                throw new HttpException(403, "must use HTTPS");
            }

            // check the whitelist before checking the identity provider
            if (whitelist.IndexOf(req.UserHostAddress) > -1)
            {
                LogEvent(req, "WHITELIST", 200);
                log.InfoFormat("Whitelisted IP {0} allowed", req.UserHostAddress);
                return;
            }

            // get the identity from the provider
            IIdentity identity;
            try
            {
                identity = identityProvider.GetIdentity(context);

                if (identity == null)
                {
                    LogEvent(req, "?", 403);
                    throw new HttpException(403, "no identity found");
                }
            }
            catch (Exception ex)
            {
                log.Error("error while obtaining identity", ex);
                LogEvent(req, "?", 403);
                throw new HttpException(403, "unable to authorize with service provider");
            }

            // set a header with the client certificate's DN, if specified
            try
            {
                if (!string.IsNullOrEmpty(config.ClientDNHeader))
                {
                    req.Headers.Set(config.ClientDNHeader, req.ClientCertificate.Subject);
                }
            }
            catch (Exception)
            {
                log.Warn("could not set client DN header");
            }

            // perform the authorization check
            EsriWebIdentity esriIdentity = (EsriWebIdentity)identity;
            Decision authorizationDecision = decisionPoint.Decide(esriIdentity);
            if (authorizationDecision == null || !authorizationDecision.IsAuthorized)
            {
                LogEvent(req, identity.Name, 403);
                throw new HttpException(403, "unauthorized");
            }
            
            req.Headers.Set("X-Auth-Evidence", authorizationDecision.Evidence.ToString());

            // set the identity for the user if specified
            if (config.SetPrincipal)
            {
                HttpContext.Current.User = new GenericPrincipal(identity, null);
            }
        }
        
        private void Module_EndRequest(object sender, EventArgs e)
        {
            HttpApplication context = (HttpApplication)sender;
            HttpRequest req = context.Request;
            HttpResponse res = context.Response;

            LogEvent(
                req,
                context.Context.User != null ? context.Context.User.Identity.Name : "?",
                res.StatusCode);
        }

        public void Dispose() { /* do nothing */ }

        private void LogEvent(HttpRequest req, string identity, int statusCode)
        {
            log.InfoFormat(
                "{0} {1} {2} {3} {4} {5}",
                req.UserHostAddress,
                req.Url.Scheme,
                req.HttpMethod,
                req.Url.PathAndQuery,
                identity,
                statusCode);
        }
    }
}
