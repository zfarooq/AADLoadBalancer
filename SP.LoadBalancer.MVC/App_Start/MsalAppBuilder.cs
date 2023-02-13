using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace SP.LoadBalancer.MVC
{
    public static class MsalAppBuilder
	{
		static string AADAuthority = $"{ConfigurationManager.AppSettings["ida:AADInstance"]}{ConfigurationManager.AppSettings["ida:Tenant"]}";
		static string AADClientSecret = ConfigurationManager.AppSettings["ida:ClientSecret"];
		static string AADClientId = ConfigurationManager.AppSettings["ida:ClientId"];
		static string AADRedirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"];
		public static IConfidentialClientApplication BuildConfidentialClientApplication()
		{
			return BuildConfidentialClientApplication(ClaimsPrincipal.Current);
		}
		public static IConfidentialClientApplication BuildConfidentialClientApplication(ClaimsPrincipal currentUser)
		{

			IConfidentialClientApplication clientapp = ConfidentialClientApplicationBuilder.Create(AADClientId)
				  .WithClientSecret(AADClientSecret)
				  .WithRedirectUri(AADRedirectUri)
				  .WithAuthority(new Uri(AADAuthority))
				  .Build();
			MSALPerUserMemoryTokenCache userTokenCache = new MSALPerUserMemoryTokenCache(clientapp.UserTokenCache, currentUser ?? ClaimsPrincipal.Current);
			return clientapp;
		}

		public static async Task ClearUserTokenCache()
		{
			IConfidentialClientApplication clientapp = ConfidentialClientApplicationBuilder.Create(AADClientId)
				  .WithClientSecret(AADClientSecret)
				  .WithRedirectUri(AADRedirectUri)
				  .WithAuthority(new Uri(AADAuthority))
				  .Build();

			// We only clear the user's tokens.
			MSALPerUserMemoryTokenCache userTokenCache = new MSALPerUserMemoryTokenCache(clientapp.UserTokenCache);
			var userAccount = await clientapp.GetAccountAsync(ClaimsPrincipal.Current.GetMsalAccountId());

			//Remove the users from the MSAL's internal cache
			await clientapp.RemoveAsync(userAccount);
			userTokenCache.Clear();
		}
	}
}