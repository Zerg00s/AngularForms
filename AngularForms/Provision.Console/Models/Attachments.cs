using SPMeta2.Definitions;
using SPMeta2.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provision.Console.Models
{
	public class Attachments
	{
		public static string ListName = "Attachments";
		public static ListDefinition listDefinition = new ListDefinition
		{
			Title = ListName,
			CustomUrl = "Attachments",
			Description = "Attachments for the Angular forms",
			TemplateType = BuiltInListTemplateTypeId.DocumentLibrary,
			EnableVersioning = false,
			ContentTypesEnabled = false,
			OnQuickLaunch = false,
			EnableFolderCreation = true
		};
	}
}
