using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provision.Console.Utils
{
	public class FeaturesHelper
	{

		public static void ActivateFeature(ClientContext clientContext, Site site, Guid featureId)
		{
            var features = site.Features;
            features.Add(featureId, true, FeatureDefinitionScope.None);
			
			try
			{
                clientContext.ExecuteQuery();
			}
			catch (Exception)
			{
				//TODO: 
			}
		}

        public static void ActivateFeature(ClientContext clientContext, Web web, Guid featureId)
        {
            var features = web.Features;
            features.Add(featureId, true, FeatureDefinitionScope.None);

            try
            {
                clientContext.ExecuteQuery();
            }
            catch (Exception)
            {
                //TODO: 
            }
        }

		public static void DeactivateFeature(ClientContext clientContext, Guid featureId)
		{
			Web web = clientContext.Web;
			var features = web.Features;
			clientContext.Load(features);
			clientContext.ExecuteQuery();

			web.Features.Remove(featureId, true);
			try
			{
				clientContext.ExecuteQuery();
			}
			catch (Exception)
			{
				//TODO: avoid try/catch
			}
		}
	}
}
