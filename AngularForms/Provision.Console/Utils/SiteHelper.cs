using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provision.Console.Utils
{
    public class SiteHelper
    {
        public static void PurgeRecycleBin(ClientContext clientContext)
        {
            Site site = clientContext.Site;
            var recycleBinCollection = site.RecycleBin;
            try
            {
                clientContext.Load(site);
                clientContext.Load(recycleBinCollection);
                clientContext.ExecuteQuery();
            }
            catch
            {
                return;
            }

            recycleBinCollection.DeleteAll();
            clientContext.Load(recycleBinCollection);
            clientContext.ExecuteQuery();
        }

		public static void SetSiteLogoUrl(ClientContext clientContext, string siteLogoUrl)
		{
			clientContext.Web.SiteLogoUrl = siteLogoUrl;
			clientContext.Web.Update();
			clientContext.ExecuteQuery();
		}
	}
}
