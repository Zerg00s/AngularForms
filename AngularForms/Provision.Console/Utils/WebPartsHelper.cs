using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provision.Console.Utils
{
	public class WebPartsHelper
	{

		public static void AddCEWP(ClientContext clientContext, string formUrl, string WebPartTitle, string Zone, string contentLink, bool closeDefaultWebPart)
		{
			formUrl = formUrl.Replace("//", "/");
			contentLink = contentLink.Replace("//", "/");
			System.Console.WriteLine(contentLink);
         Microsoft.SharePoint.Client.File oFile = clientContext.Web.GetFileByServerRelativeUrl(formUrl);
			LimitedWebPartManager limitedWebPartManager = oFile.GetLimitedWebPartManager(PersonalizationScope.Shared);
			string webpartxml = @"<?xml version='1.0' encoding='utf-8'?>
<WebPart xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns='http://schemas.microsoft.com/WebPart/v2'>
  <Title>"+ WebPartTitle + @"</Title>
  <FrameType>Default</FrameType>
  <Description />
  <IsIncluded>true</IsIncluded>
  <ZoneID>"+ Zone + @"</ZoneID>
  <PartOrder>0</PartOrder>
  <FrameState>Normal</FrameState>
  <Height />
  <Width />
  <AllowRemove>true</AllowRemove>
  <AllowZoneChange>true</AllowZoneChange>
  <AllowMinimize>true</AllowMinimize>
  <AllowConnect>true</AllowConnect>
  <AllowEdit>true</AllowEdit>
  <AllowHide>true</AllowHide>
  <IsVisible>true</IsVisible>
  <DetailLink />
  <HelpLink />
  <HelpMode>Modeless</HelpMode>
  <Dir>Default</Dir>
  <PartImageSmall />
  <MissingAssembly>Cannot import this Web Part.</MissingAssembly>
  <PartImageLarge>/_layouts/15/images/mscontl.gif</PartImageLarge>
  <IsIncludedFilter />
  <ContentLink>" + contentLink + @"</ContentLink>
  <Assembly>Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c</Assembly>
  <TypeName>Microsoft.SharePoint.WebPartPages.ContentEditorWebPart</TypeName>
  <ContentLink xmlns='http://schemas.microsoft.com/WebPart/v2/ContentEditor'>"+ contentLink + @"</ContentLink>
  <Content xmlns='http://schemas.microsoft.com/WebPart/v2/ContentEditor' />
  <PartStorage xmlns='http://schemas.microsoft.com/WebPart/v2/ContentEditor' />
</WebPart>" /*The webpart XML */;
			try
			{
				clientContext.Load(limitedWebPartManager.WebParts);
				clientContext.ExecuteQuery();

				if (closeDefaultWebPart)
				{
					CloseDefaultWebPart(clientContext, limitedWebPartManager.WebParts);
				}

				DeleteWebPart(clientContext, formUrl, WebPartTitle);

				var wpd = limitedWebPartManager.ImportWebPart(webpartxml);
				limitedWebPartManager.AddWebPart(wpd.WebPart, Zone, 0);
				clientContext.ExecuteQuery();
			}
			catch (Exception ex)
			{
				System.Console.WriteLine("error while adding the web part to the {0} form", formUrl);
				System.Console.WriteLine(ex.Message);
			}
		}

      public static void CloseDefaultWebPart(ClientContext clientContext, WebPartDefinitionCollection webPartDefinitionCollection)
      {
         foreach (var WebPart in webPartDefinitionCollection)
         {
            clientContext.Load(WebPart, w => w.WebPart.Title);

            clientContext.ExecuteQuery();

            if (WebPart.WebPart.Title == "")
            {
               WebPart.CloseWebPart();
               WebPart.SaveWebPartChanges();
               clientContext.ExecuteQuery();
            }
         }
      } 

		public static void DeleteWebPart(ClientContext clientContext, string serverRelativeFormUrl, string WebPartTitle)
		{
			File oFile = clientContext.Web.GetFileByServerRelativeUrl(serverRelativeFormUrl);
			LimitedWebPartManager limitedWebPartManager = oFile.GetLimitedWebPartManager(PersonalizationScope.Shared);
			try
			{
				clientContext.Load(limitedWebPartManager.WebParts);
				clientContext.ExecuteQuery();

				foreach (var webPart in limitedWebPartManager.WebParts)
				{
					clientContext.Load(webPart.WebPart);
				}
				clientContext.ExecuteQuery();

				foreach (var WebPart in limitedWebPartManager.WebParts)
				{
					clientContext.Load(WebPart, w => w.WebPart.Title);

					clientContext.ExecuteQuery();

					if (WebPart.WebPart.Title == WebPartTitle)
					{
						WebPart.DeleteWebPart();
						clientContext.ExecuteQuery();
					}
				}
			}
			catch (Exception ex)
			{
				System.Console.WriteLine("error while deleting the web part {0} from the form {1}", WebPartTitle, serverRelativeFormUrl);
				System.Console.WriteLine(ex.Message);
			}
		}

		public static void AddWebPart(ClientContext clientContext)
		{

			File oFile = clientContext.Web.GetFileByServerRelativeUrl(@"/teams/LCBO/delete/SitePages/NewHome.aspx");
			LimitedWebPartManager limitedWebPartManager = oFile.GetLimitedWebPartManager(PersonalizationScope.Shared);
			string webpartxml = @"<?xml version='1.0' encoding='utf-8'?>
<WebPart xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns='http://schemas.microsoft.com/WebPart/v2'>
  <Title>" + "Content Editor" + @"</Title>
  <FrameType>Default</FrameType>
  <Description />
  <IsIncluded>true</IsIncluded>
  <ZoneID>g_B52A77AA960F43A9B85DE9D8D20CD89F</ZoneID>
  <PartOrder>10</PartOrder>
  <FrameState>Normal</FrameState>
  <Height />
  <Width />
  <AllowRemove>true</AllowRemove>
  <AllowZoneChange>true</AllowZoneChange>
  <AllowMinimize>true</AllowMinimize>
  <AllowConnect>true</AllowConnect>
  <AllowEdit>true</AllowEdit>
  <AllowHide>true</AllowHide>
  <IsVisible>true</IsVisible>
  <DetailLink />
  <HelpLink />
  <HelpMode>Modeless</HelpMode>
  <Dir>Default</Dir>
  <PartImageSmall />
  <MissingAssembly>Cannot import this Web Part.</MissingAssembly>
  <PartImageLarge>/_layouts/15/images/mscontl.gif</PartImageLarge>
  <IsIncludedFilter />
  <ContentLink>" + @"/teams/LCBO/delete/Assets/App/provisioned.html" + @"</ContentLink>
  <Assembly>Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c</Assembly>
  <TypeName>Microsoft.SharePoint.WebPartPages.ContentEditorWebPart</TypeName>
  <ContentLink xmlns='http://schemas.microsoft.com/WebPart/v2/ContentEditor'>" + "/teams/LCBO/delete/Assets/App/provisioned.html" + @"</ContentLink>
  <Content xmlns='http://schemas.microsoft.com/WebPart/v2/ContentEditor' />
  <PartStorage xmlns='http://schemas.microsoft.com/WebPart/v2/ContentEditor' />
</WebPart>" /*The webpart XML */;
			try
			{
				clientContext.Load(limitedWebPartManager.WebParts);
				clientContext.ExecuteQuery();

				foreach (var webPart in limitedWebPartManager.WebParts)
				{
					clientContext.Load(webPart.WebPart);
				}
				clientContext.ExecuteQuery();

				DeleteWebPart(clientContext, "/teams/LCBO/delete/SitePages/NewHome.aspx", "Content Editor");

				var wpd = limitedWebPartManager.ImportWebPart(webpartxml);
				limitedWebPartManager.AddWebPart(wpd.WebPart, "Header", 0);
				clientContext.ExecuteQuery();
			}
			catch (Exception ex)
			{
				System.Console.WriteLine("error while adding the web part to the form");
				System.Console.WriteLine(ex.Message);
			}

		}
	}


}
