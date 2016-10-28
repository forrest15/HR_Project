using System.Threading.Tasks;
using DAL.Services;
using Microsoft.Owin.Security.OAuth;

namespace WebUI.Auth
{
    public class JwtAuthServer : OAuthAuthorizationServerProvider
    {
        IAuthContainer<string> _container;

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            await context.OwinContext.Get<UserService>().AuthentificateAsync(context.UserName, context.Password);
        }

    }
}