using System;
using System.Configuration;
using Autofac;
using Mailer;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;
using WebUI.App_Start;
using WebUI.Auth;
using WebUI.Extensions;
using WebUI.Globals;

[assembly: OwinStartup(typeof(WebUI.ApiStartup))]
namespace WebUI
{
    public class ApiStartup
    {
        public void Configuration(IAppBuilder app)
        {
            string rootFolder = SettingsContext.Instance.GetRootPath();
            string uploadsPath = SettingsContext.Instance.GetUploadsPath();
            string requestPath = SettingsContext.Instance.RequestPath;

            string defaultPage = "/index.html";

            AppConfiguration.ConfigureAutomapper();
            AppConfiguration.ConfigureJsonConverter();
            AppConfiguration.ConfigureDatabaseInitializer();
            AppConfiguration.ConfigureDirrectory(rootFolder);
            AppConfiguration.ConfigureDirrectory(uploadsPath);
            AppConfiguration.ConfigureMailAgent(SettingsContext.Instance.Email,
                SettingsContext.Instance.Password);

            TemplateLoader.SetupFile("Data/email.html");

            var config = WebApiConfig
                .Create()
#if RELEASE
                .ConfigureExceptionLogging()
#endif
                .ConfigureCors()
                .ConfigureRouting()
                .ConfigJsonSerialization();

            var container = AutofacConfig.Initialize(config);

            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(config);
            app.UseWebApi(config);
            app.UseStaticFilesServer(uploadsPath, requestPath);
            app.UseHtml5Routing(rootFolder, defaultPage);


            //stub
            var issuer = ConfigurationManager.AppSettings["issuer"];
            var secret = Microsoft.Owin.Security.DataHandler.Encoder.TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["secret"]);

            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                AllowedAudiences = new[] { "Any" },
                IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                {
                    new SymmetricKeyIssuerSecurityTokenProvider(issuer, secret)
                }
            });

            using (var scope = container.BeginLifetimeScope())
            {
                app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
                {
                    //stub till we haven't enabled ssl
                    AllowInsecureHttp = true,
                    TokenEndpointPath = new PathString("/account/signin"),
                    AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(60),
                    Provider = scope.Resolve<OAuthAuthorizationServerProvider>(),
                    AccessTokenFormat = new ApplicationJwt(issuer)
                });
            }
        }
    }
}
