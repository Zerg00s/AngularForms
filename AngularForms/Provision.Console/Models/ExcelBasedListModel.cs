using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using SPMeta2.Models;
using SPMeta2.Syntax.Default;
using SPMeta2.Syntax.Default.Modern;
using SPMeta2.Syntax.Default.Utils;
using SPMeta2.CSOM.Services;
using SPMeta2.Definitions;
using SPMeta2.Enumerations;
using System.Net;
using Provision.Console;
using SPMeta2.CSOM.ModelHosts;
using SPMeta2.Definitions.Fields;
using SPMeta2.Definitions.ContentTypes;
using Provision.Console.Utils;
using System.Xml.Linq;

namespace Provision.Console.Models
{
	public class ExcelBasedListModel
	{
		public string ContentTypeName { get; set; }
		public Guid ContentGuid { get; set; }
		public string ListName { get; set; }
		public string ListUrl { get; set; }
		public string Group { get; set; }
		public string CsvFilePath { get; set; }
		public string ListInternalName{ get; set; }


		public ExcelBasedListModel(string csvFilePath, string listDisplayName, string listUrl, Guid contentTypeId, string contentTypeName, string group)
		{
			CsvFilePath = csvFilePath;
			ListName = listDisplayName;
			ListUrl = listUrl;
			ContentGuid = contentTypeId;
			ContentTypeName = contentTypeName;
			Group = group;
		}

		public ListDefinition getListDefinition()
		{
			var listDefinition = new ListDefinition();
			listDefinition.Title = ListName;
			listDefinition.CustomUrl = ListUrl;
			listDefinition.Description = "";
			listDefinition.TemplateType = BuiltInListTemplateTypeId.GenericList;
			listDefinition.EnableVersioning = true;
			listDefinition.ContentTypesEnabled = true;
			listDefinition.OnQuickLaunch = true;
			listDefinition.EnableFolderCreation = false;

			return listDefinition;
		}

		public ContentTypeDefinition ContentTypeDefinition
		{
			get
			{
				ContentTypeDefinition contentTypeDefinition = new ContentTypeDefinition
				{
					Name = ContentTypeName,
					Id = ContentGuid,
					ParentContentTypeId = BuiltInContentTypeId.Item,
					Group = Group
				};

            return contentTypeDefinition;
         }

		}

		List<FieldDefinition> fieldDefinitions = null;
		List<xField> xFields = null;

		public List<FieldDefinition> FieldDefnitions
		{
			get
			{
				if (fieldDefinitions != null)
				{
					return fieldDefinitions;
				}
				List<FieldDefinition> fields = new List<FieldDefinition>();
				xFields = new List<xField>();

				string[] records = System.IO.File.ReadAllLines(CsvFilePath);

				records = records.Skip(1).ToArray();
				foreach (string rec in records)
				{
					string[] field = rec.Split('\t');
					xField xField = new xField(field);
					xFields.Add(xField);
               fields.Add(xField.definition);
				}
				fieldDefinitions = fields;
				return fields;
			}
		}

		public string GenerateAngularView()
		{
			string templateFolder = @"App\Forms\SampleForm";
			string destinationFolder = string.Format("App\\Forms\\{0}", ListInternalName);
			System.IO.Directory.CreateDirectory(destinationFolder);
			var k = FieldDefnitions;
			StringBuilder builder = new StringBuilder();

			foreach (xField field in xFields)
			{
            builder.Append(field.AngularView);
         }

			System.IO.File.Delete(string.Format("App\\Forms\\{0}\\{0}Fields.html", ListInternalName));
			
         System.IO.File.WriteAllText(string.Format("App\\Forms\\{0}\\{0}Fields.html", ListInternalName), builder.ToString());

			string destinationControlerFile = destinationFolder + "\\" + ListInternalName + "Controller.js";
			string destinationViewFile = destinationFolder + "\\" + ListInternalName + "Form.html";
			System.IO.File.Delete(destinationControlerFile);
			System.IO.File.Delete(destinationViewFile);
         System.IO.File.Copy(templateFolder + @"\sampleController.js", destinationControlerFile);
			System.IO.File.Copy(templateFolder + @"\sampleForm.html", destinationViewFile);

			string controlerText = System.IO.File.ReadAllText(destinationControlerFile);
			controlerText = controlerText.Replace("LIST_TITLE", ListName);
			System.IO.File.WriteAllText(destinationControlerFile, controlerText);

			string viewText = System.IO.File.ReadAllText(destinationViewFile);
			viewText = viewText.Replace("LIST_NAME", ListInternalName);
			System.IO.File.WriteAllText(destinationViewFile, viewText);

			return builder.ToString();
      }

		public List<string> GetAllFieldNames()
		{
			var fieldNames = FieldDefnitions.Select(x => x.InternalName).ToList();
			return fieldNames;
		}

		public List<Guid> GetAllFieldIds()
		{
			var fieldIds = FieldDefnitions.Select(x => x.Id).ToList();
			return fieldIds;
		}
		public List<FieldLinkValue> FieldLinkValues()
		{
			var fieldIds = FieldDefnitions.Select(x => new FieldLinkValue { Id = x.Id }).ToList();
			return fieldIds;
		}

		public void InjectModelsToDefinition(WebModelNode webDefinition)
		{
			webDefinition.AddFields(FieldDefnitions);
			webDefinition.AddContentType(ContentTypeDefinition, contentType =>
			{
				contentType
				.AddContentTypeFieldLinks(FieldDefnitions)
				.AddUniqueContentTypeFieldsOrder(new UniqueContentTypeFieldsOrderDefinition
				{
					Fields = FieldLinkValues()
				});
			});
			webDefinition.AddList(this.getListDefinition(), list =>
			{
				list
				.AddContentTypeLink(ContentTypeDefinition)
				.AddUniqueContentTypeOrder(new UniqueContentTypeOrderDefinition
				{
					ContentTypes = new List<ContentTypeLinkValue>
						{
									new ContentTypeLinkValue{ ContentTypeName = ContentTypeDefinition.Name},
						}
				})
			//.AddListView(deploymentDefaultView)
			//.AddListView(deploymentFullView)
			;
			});
		}
 
	}
}



 
