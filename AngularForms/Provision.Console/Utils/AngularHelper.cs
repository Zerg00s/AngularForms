using Microsoft.SharePoint.Client;
using Provision.Console.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provision.Console.Utils
{
	public class AngularHelper
	{
		public static void GenerateView(ClientContext clientContext, List list)
		{
			clientContext.Load(list, x => x.RootFolder.Name);
			clientContext.ExecuteQuery();

         ListsHelper.GenerateListCsv(clientContext, list, list.Title + ".csv");
			clientContext.Load<List>(list, x => x.DefaultDisplayFormUrl);
			clientContext.Load<List>(list, x => x.DefaultNewFormUrl);
			clientContext.Load<List>(list, x => x.DefaultEditFormUrl);
			clientContext.ExecuteQuery();
			var listModel = new ExcelBasedListModel(@"Models\" + list.Title + ".csv", list.Title, list.DefaultDisplayFormUrl, new Guid("79658c1e-3096-5544-5535-4335501a5b96"), "", "");
			listModel.ListInternalName = list.RootFolder.Name;
         listModel.GenerateAngularView();
		}
	}
}
