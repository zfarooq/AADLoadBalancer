using System;
using Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Configuration;
using System.Globalization;
using System.Threading.Tasks;
using System.Web;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt.Extended;
using Microsoft.Owin.Security.Notifications;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Identity.Client;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace SP.LoadBalancer.MVC
{
    public partial class Startup
    {

        private static string scope = ConfigurationManager.AppSettings["ida:Scopes"];
        private string AUTH_COOKIE_KEY_NAME = "auth";
        private string SPA_COOKIE_KEY_NAME = "spaauth";
        public void ConfigureAuth(IAppBuilder app)
        {
            string AADAuthority = $"{ConfigurationManager.AppSettings["ida:AADInstance"]}{ConfigurationManager.AppSettings["ida:Tenant"]}";
            string AADClientId = ConfigurationManager.AppSettings["ida:ClientId"];
            string AADRedirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"];

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    Authority = AADAuthority, //authority,
                    ClientId = AADClientId,
                    RedirectUri = AADRedirectUri,
                    PostLogoutRedirectUri = AADRedirectUri,
                    TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                    },
                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        AuthorizationCodeReceived = OnAuthorizationCodeReceived,
                    },
                    CookieManager = new SameSiteCookieManager(new SystemWebCookieManager())
                });


        }

        private async Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedNotification context)
        {
            // Upon successful sign in, get the access token & cache it using MSAL
            //var principal = context.AuthenticationTicket as System.Security.Claims.ClaimsPrincipal
            IConfidentialClientApplication clientApp = MsalAppBuilder.BuildConfidentialClientApplication(new System.Security.Claims.ClaimsPrincipal(context.AuthenticationTicket.Identity));
            AuthenticationResult result = await clientApp.AcquireTokenByAuthorizationCode(new[] { scope }, context.Code).WithSpaAuthorizationCode().ExecuteAsync();
            HttpContext.Current.Response.SetCookie(new HttpCookie(AUTH_COOKIE_KEY_NAME, result.AccessToken));
            HttpContext.Current.Response.SetCookie(new HttpCookie(SPA_COOKIE_KEY_NAME, result.SpaAuthCode));
        }
        private Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> notification)
        {
            notification.HandleResponse();
            notification.Response.Redirect("/Error?message=" + notification.Exception.Message);
            return Task.FromResult(0);
        }
    }
}