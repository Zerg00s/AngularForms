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
		public const string MinimumDownloadFeatureGuid = "87294c72-f260-42f3-a41b-981a2ffce37a";
		public const string OpenDocumentsinClientApplicationsByDefailtFeatureGuid = "8a4b8de2-6fd8-41e9-923c-c7c3c00f8295";

		public const string SourceFolder = "App";
		public const string AppDestinationLibraryTitle = "Assets";
		public const string AssetsLibraryInternalname = "Assets";
		#endregion

		static void Main(string[] args)
		{
			ClientContext clientContext = null;
			clientContext = ContextHelper.GetClientContext("http://contoso/forms/conf4/", @"user@domain");

			//ProvisionSharePointArtifacts(clientContext);
			FileHelper.UploadFoldersRecursively(clientContext, SourceFolder, AppDestinationLibraryTitle);
			//WebPartsHelper.AddWebPart(clientContext);
			//WebPartsHelper.FooWebPart(clientContext, @"/teams/LCBO/delete/SitePages/Home.aspx", "Remotely Provisioned", "Header", @"/teams/LCBO/delete/Assets/App/provisioned.html", false);
			//AddWebParts(clientContext.Web.Url, clientContext);
			//FileHelper.DownloadLibrary(clientContext, "Assets", @"C:\temp\results\subfolder\");
			System.Console.WriteLine("PRESS ANY KEY TO EXIT");
			System.Console.ReadKey(true);
		}

		private static void ProvisionSharePointArtifacts(ClientContext clientContext)
		{
			var webModel = SPMeta2Model.NewWebModel();
			webModel.AddList(Assets.listDefinition);

			//ExcelBasedListModel travelModel = new ExcelBasedListModel(@"Models\Travel.csv", "Travel Requests", "Travel", new Guid("79658c1e-3096-5544-5535-4335501a5b91"), "Travel Requests", "Senate");
			//travelModel.InjectModelsToDefinition(webDefinition);
			//travelModel.GenerateAngularView();
			
			//ListHelper.GenerateListCsv(clientContext, "/teams/LCBO/LeaveRequest/LeaveofAbsence/", "Absence.csv");
			
			var csomProvisionService = new CSOMProvisionService();
			csomProvisionService.DeployWebModel(clientContext, webModel);
		}


		private static void AddWebParts(string SITE_URL, ClientContext clientContext)
		{
			System.Console.WriteLine("adding web parts to the forms");

			//Travel:
			WebPartsHelper.AddCEWP(clientContext, new Uri(SITE_URL).AbsolutePath + "/Travel/NewForm.aspx", "Senate CEWP", "Main", new Uri(SITE_URL).AbsolutePath + "/" + AssetsLibraryInternalname + "/App/Forms/Travel/travelForm.html", true);

			WebPartsHelper.AddCEWP(clientContext, new Uri(SITE_URL).AbsolutePath + "/Travel/EditForm.aspx", "Senate CEWP", "Main", new Uri(SITE_URL).AbsolutePath + "/" + AssetsLibraryInternalname + "/App/Forms/Travel/travelForm.html", true);

			WebPartsHelper.AddCEWP(clientContext, new Uri(SITE_URL).AbsolutePath + "/Travel/DispForm.aspx", "Senate CEWP", "Main", new Uri(SITE_URL).AbsolutePath + "/" + AssetsLibraryInternalname + "/App/Forms/Travel/travelForm.html", true);
		}

	}
}
