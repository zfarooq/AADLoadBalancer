using Microsoft.Owin;
using Owin;
using System;
using System.Threading.Tasks;
using Microsoft.Owin.Cors;
using System.Web.Http;
using Swashbuckle.Application;
using System.Linq;
using System.Configuration;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Jwt;
using Microsoft.IdentityModel.Tokens;

[assembly: OwinStartup(typeof(SP.LoadBalancer.WebAPI.Startup))]

namespace SP.LoadBalancer.WebAPI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888


            app.UseCors(CorsOptions.AllowAll);

            var config = new HttpConfiguration();

            config.EnableSwagger(x =>
            {
                x.SingleApiVersion("v1", "Fortress Services");
                x.DescribeAllEnumsAsStrings();
                x.ResolveConflictingActions(desc => desc.First());
            });

            ConfigureWebApi(app, config);

            RegisterGlobalFilters(config.Filters);

        }

        private void ConfigureWebApi(IAppBuilder app, HttpConfiguration config)
        {
           
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        }

        private void RegisterGlobalFilters(System.Web.Http.Filters.HttpFilterCollection filters)
        {
            filters.Add(new System.Web.Http.AuthorizeAttribute());
        }

        private void ConfigureAuth(IAppBuilder app)
        {
            var audience = ConfigurationManager.AppSettings["ida:Audience"];
            var tenant = ConfigurationManager.AppSettings["ida:Tenant"];

            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions
            {
                AccessTokenFormat = new JwtFormat(
                    new TokenValidationParameters
                    {
                        ValidAudience = audience,
                        ValidateIssuer = true,
                        ValidIssuer = string.Format("https://sts.windows.net/{0}/", tenant)
                    }, new OpenIdConnectSecurityKeyProvider(
                        string.Format("https://login.microsoftonline.com/{0}/v2.0/.well-known/openid-configuration", tenant)
                ))
            });
        }


    }
}
