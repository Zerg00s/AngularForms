using SPMeta2.Definitions;
using SPMeta2.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provision.Console.Models
{
	public class Assets
	{
		public static string ListName = "Assets";
		public static ListDefinition listDefinition = new ListDefinition
		{
			Title = ListName,
			CustomUrl = "Assets",
			Description = "Assets",
			TemplateType = BuiltInListTemplateTypeId.DocumentLibrary,
			EnableVersioning = true,
			ContentTypesEnabled = true,
			OnQuickLaunch = true,
			EnableFolderCreation = true
		};
	}
}
