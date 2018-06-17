using Microsoft.SharePoint.Client;
using Provision.Console.Models;
using Provision.Console.Utils;
using SPMeta2.CSOM.Services;
using SPMeta2.Definitions.Fields;
using SPMeta2.Syntax.Default;
using System;

namespace Provision.Console
{
	class Program
	{
		public const string SourceFolder = "App";
		public const string AppDestinationLibraryTitle = "Assets";
		public const string AssetsLibraryInternalname = "Assets";

		static void Main(string[] args) 
		{
            Introduction();
            Messenger.Attention("Enter target site URL:");
            Messenger.Info("Example: https://contoso.sharepoint.com/portal or http://SP2013Portal/subsite/");
            System.Console.Write("URL: ");
            string targetSiteUrl = System.Console.ReadLine();

            Messenger.Attention("Enter login:");
            Messenger.InfoBold("Examples:");
            Messenger.Info("SharePoint Online: JohnP@portal.onmicrosoft.com");
            Messenger.Info("SharePoint Online: JohnPy@contoso.com");
            Messenger.Info("SharePoint 2013/2016: contoso\\johnP");
            System.Console.Write("Login: ");
            string userLogin  = System.Console.ReadLine();

            var clientContext = ContextHelper.GetClientContext(targetSiteUrl, userLogin);

            Messenger.Attention("Enter target list Title:");
            System.Console.Write("List title: ");
            string targetListTitle = System.Console.ReadLine();

            List targetList = clientContext.Web.Lists.GetByTitle(targetListTitle);
			clientContext.Load(targetList);
			clientContext.ExecuteQuery();

			DeployAttachmentsLibrary(clientContext, targetList);

			AngularHelper.GenerateView(clientContext, targetList);

            FileHelper.UploadFoldersRecursively(clientContext, SourceFolder, AppDestinationLibraryTitle);

			WebPartsHelper.AddCEWPToList(clientContext, targetList);

            //Open freshly generated Form in the browser:
            System.Diagnostics.Process.Start(new Uri(clientContext.Web.Url).GetLeftPart(UriPartial.Authority) + "/" + targetList.DefaultNewFormUrl);
        }

		private static void DeployAttachmentsLibrary(ClientContext clientContext, List targetList)
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


        private static void Introduction()
        {
            Messenger.Attention("*****************************************************************************");
            Messenger.Success("Angular Forms Generator");
            Messenger.InfoBold("Automatically Generates an Angular.js form for a specified SharePoint list");
            Messenger.Attention("*****************************************************************************");
        }

	}
}
