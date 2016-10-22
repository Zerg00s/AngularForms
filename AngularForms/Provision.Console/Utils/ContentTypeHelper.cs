using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provision.Console.Utils
{
    public class ContentTypeHelper
    {
        public static void SetDocumentTemplateForConentType(ClientContext clientContext, string DeploymentContentTypeId, string wordTemplateUrl)
        {
            clientContext.Load(clientContext.Web.ContentTypes);
            ContentType ct = clientContext.Web.ContentTypes.GetById(DeploymentContentTypeId);
            clientContext.Load(ct);
            clientContext.ExecuteQuery();
            ct.DocumentTemplate = wordTemplateUrl;
            ct.Update(true);

			clientContext.ExecuteQuery();
        }

        public static void RemoveContentType(ClientContext clientContext, string listDisplayName, string contentTypeName)
        {
            List list = clientContext.Web.Lists.GetByTitle(listDisplayName);
            clientContext.Load(list.ContentTypes);
            clientContext.ExecuteQuery();
            ContentType contentType = list.ContentTypes.FirstOrDefault(ct => ct.Name == contentTypeName);

            if (contentType != null)
	        {
		        contentType.DeleteObject();
                clientContext.ExecuteQuery();
	        }
        }

        public static void RemoveContentType(ClientContext clientContext, Web web, string contentTypeName)
        {
            clientContext.Load(web.ContentTypes);
            clientContext.ExecuteQuery();
            ContentType contentType = web.ContentTypes.FirstOrDefault(ct => ct.Name == contentTypeName);

            if (contentType != null)
            {
                contentType.DeleteObject();
                clientContext.ExecuteQuery();
            }
        }
    }
}
