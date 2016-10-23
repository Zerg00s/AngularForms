using Microsoft.SharePoint.Client;
using Provision.Console.Models;
using Provision.Console.Utils;
using SPMeta2.CSOM.Services;
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
			string targetSiteUrl = "https://site.sharepoint.com/sites/senate/subsite/subsite2";
			string userLogin = @"login@domain.com";
         var clientContext = ContextHelper.GetClientContext(targetSiteUrl, userLogin);
			List targetList = clientContext.Web.Lists.GetByTitle("TargetListTitle");
			clientContext.Load(targetList);
			clientContext.ExecuteQuery();

			DeployListsAndLibraries(clientContext);

			AngularHelper.GenerateView(clientContext, targetList);

         FileHelper.UploadFoldersRecursively(clientContext, SourceFolder, AppDestinationLibraryTitle);

			WebPartsHelper.AddCEWPToList(clientContext, targetList);

			System.Console.WriteLine("PRESS ANY KEY TO EXIT");
			System.Console.ReadKey(true);
		}

		private static void DeployListsAndLibraries(ClientContext clientContext)
		{
			var webModel = SPMeta2Model.NewWebModel();
			webModel.AddList(Assets.listDefinition);
			
			var csomProvisionService = new CSOMProvisionService();
			csomProvisionService.DeployWebModel(clientContext, webModel);
		}
	}
}
