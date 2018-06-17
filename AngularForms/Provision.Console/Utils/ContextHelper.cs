using Provision.Console;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Provision.Console.Utils
{
	public class ContextHelper
	{
		public static ClientContext GetClientContext(string siteUrl, string user)
		{
			SecureString securePassword = PasswordHelper.
                GetPasswordFromConsoleInput();
			return GetClientContext(siteUrl, user, securePassword);
		}
		public static ClientContext GetClientContext(string siteUrl, string user, string password)
		{
			SecureString securePassword = StringExtentions.GetSecureString(password);
			return GetClientContext(siteUrl, user, securePassword);
      }
		public static ClientContext GetClientContext(string siteUrl, string user, SecureString password)
		{
			bool onPrem = true;

			if (siteUrl.Contains("sharepoint.com"))
			{
				onPrem = false;
			}

			System.Console.WriteLine();
			System.Console.WriteLine("Current domain: {0}", Environment.UserDomainName);

			ClientContext clientContext = new ClientContext(siteUrl);
			clientContext.AuthenticationMode = ClientAuthenticationMode.Default;

			if (onPrem)
			{
				clientContext.Credentials = new NetworkCredential(user, password);
			}
			else
			{
				clientContext.Credentials = new SharePointOnlineCredentials(user, password);
			}

			clientContext.Load(clientContext.Web);
			clientContext.ExecuteQuery();
			System.Console.ForegroundColor = ConsoleColor.Green;
			System.Console.WriteLine("Connected to the site: {0} | {1}", clientContext.Web.Title, siteUrl);
			System.Console.ForegroundColor = ConsoleColor.White ;

			return clientContext;
		}
   }
}
