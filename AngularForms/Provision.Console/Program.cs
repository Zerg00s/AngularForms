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
			string targetSiteUrl = "https://jolera365.sharepoint.com/sites/demo/subsite";
			string userLogin = @"dmolodtsov@jolera.com";
			string targetListTitle = "SampleList2";

            var clientContext = ContextHelper.GetClientContext(targetSiteUrl, userLogin);
			List targetList = clientContext.Web.Lists.GetByTitle(targetListTitle);
			clientContext.Load(targetList);
			clientContext.ExecuteQuery();

			DeployListsAndLibraries(clientContext, targetList);

			AngularHelper.GenerateView(clientContext, targetList);

            FileHelper.UploadFoldersRecursively(clientContext, SourceFolder, AppDestinationLibraryTitle);

			WebPartsHelper.AddCEWPToList(clientContext, targetList);

            //Open the Form in the browser:
            System.Diagnostics.Process.Start(new Uri(clientContext.Web.Url).GetLeftPart(UriPartial.Authority) + "/" + targetList.DefaultNewFormUrl);
        }

		private static void DeployListsAndLibraries(ClientContext clientContext, List targetList)
		{
			var csomProvisionService = new CSOMProvisionService();

			var webModel = SPMeta2Model.NewWebModel();
			webModel.AddList(Assets.listDefinition);
			
			csomProvisionService.DeployWebModel(clientContext, webModel);

			var attachmentsFieldDefinition = new LookupFieldDefinition
			{
				Title = "ParentItemID",
				InternalName = "ParentItemID",
				Group = "Angular",
				Id = new Guid("FEF440A7-3228-4034-BB2A-FFD538D46A62"),
				LookupListTitle = targetList.Title,
				LookupWebId = clientContext.Web.Id,
				LookupField = "ID",
                RelationshipDeleteBehavior = "Cascade",
                Indexed = true
			};

			var lookupFieldModel = SPMeta2Model.NewWebModel(web =>
			{
				web.AddField(attachmentsFieldDefinition);
			});

            webModel.AddList(Attachments.listDefinition, list => list.AddField(attachmentsFieldDefinition));
            csomProvisionService.DeployWebModel(clientContext, webModel);
		}
	}
}
