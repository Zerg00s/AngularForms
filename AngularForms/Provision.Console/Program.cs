using Microsoft.SharePoint.Client;
using Provision.Console.Models;
using Provision.Console.Utils;
using SPMeta2.CSOM.Services;
using SPMeta2.Definitions.Fields;
using SPMeta2.Syntax.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provision.Console
{
	class Program
	{
		#region constants
		public const string SourceFolder = "App";
		public const string AppDestinationLibraryTitle = "Assets";
		public const string AssetsLibraryInternalname = "Assets";
		#endregion

		static void Main(string[] args)
		{
			string targetSiteUrl = "https://365.sharepoint.com/sites/senate/subsite/subsite2";
			string userLogin = @"dmolodtsov@365.com";
			string targetListTitle = "SampleList2";

         var clientContext = ContextHelper.GetClientContext(targetSiteUrl, userLogin);
			List targetList = clientContext.Web.Lists.GetByTitle(targetListTitle);
			clientContext.Load(targetList);
			clientContext.ExecuteQuery();

			DeployListsAndLibraries(clientContext, targetList);

			AngularHelper.GenerateView(clientContext, targetList);

         FileHelper.UploadFoldersRecursively(clientContext, SourceFolder, AppDestinationLibraryTitle);

			WebPartsHelper.AddCEWPToList(clientContext, targetList);

			System.Console.WriteLine("PRESS ANY KEY TO EXIT");
			System.Console.ReadKey(true);
		}

		private static void DeployListsAndLibraries(ClientContext clientContext, List targetList)
		{
			var csomProvisionService = new CSOMProvisionService();

			var webModel = SPMeta2Model.NewWebModel();
			webModel.AddList(Assets.listDefinition);
			webModel.AddList(Attachments.listDefinition);
			csomProvisionService.DeployWebModel(clientContext, webModel);

			var attachmentsFieldDefinition = new LookupFieldDefinition
			{
				Title = "ParentItemID",
				InternalName = "ParentItemID",
				Group = "Angular",
				Id = new Guid("FEFC30A7-3B38-4034-BB2A-FFD538D46A62"),
				LookupListTitle = targetList.Title,
				//Error: SPMETA2: tries to get Assets list from the root web insted of the current web
				//https://jolera365.sharepoint.com/sites/senate
				LookupWebId = clientContext.Web.Id,
				LookupField = "ID"
			};

			var lookupFieldModel = SPMeta2Model.NewWebModel(web =>
			{
				web
					 .AddField(attachmentsFieldDefinition);
			});
			
			csomProvisionService.DeployWebModel(clientContext, lookupFieldModel);
			//error: duplicate field ParentItemID was found...
		}
	}
}
