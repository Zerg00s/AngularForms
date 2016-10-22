using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provision.Console.Utils
{
    public class SPCleaner
    {
        public static void PurgeRecycleBin(ClientContext clientContext)
        {
            SiteHelper.PurgeRecycleBin(clientContext);
        }
    }
}
