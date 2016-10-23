using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provision.Console.Utils
{
    public class ListsHelper
    {
		public static void RemoveList(ClientContext clientContext, string listDisplayName)
				  {
						List oList = clientContext.Web.Lists.GetByTitle(listDisplayName);
						oList.DeleteObject();
						try
						{
							 clientContext.ExecuteQuery();
						}
						catch (Exception)
						{
						 //TODO:   
						}
				  }

		public static void GenerateListCsv(ClientContext clientContext, List oList, string fileName)
				{
					FieldCollection fieldColl = oList.Fields;
					clientContext.Load(fieldColl);
					clientContext.ExecuteQuery();
					StringBuilder lines = new StringBuilder();
					string headerLine = string.Join("\t",
						"Name",
						"Type",
						"Description",
						"DisplayName",
						"StaticName",
						"Group",
						"Hidden",
						"Required",
						"Sealed",
						"ShowInDisplayForm",
						"ShowInEditForm",
						"ShowInNewForm",
						"Guid",
						"Choices"
						);
					lines.Append(headerLine + Environment.NewLine);
					foreach (Field fieldTemp in fieldColl)
					{
						if (fieldTemp.Hidden == false)
						{
							if (fieldTemp.ReadOnlyField == false)
							{
								if (fieldTemp.InternalName.ToString() != "ContentType")
								{
									if (fieldTemp.InternalName.ToString() != "Attachments")
									{
										System.Console.WriteLine(fieldTemp.InternalName.ToString());

										string choices = string.Empty;
										if (fieldTemp.TypeAsString.Contains("Choice"))
										{
											var choiceField = clientContext.CastTo<FieldChoice>(fieldTemp);
											clientContext.Load(choiceField);
											clientContext.ExecuteQuery();
											choices = string.Join("#", choiceField.Choices);
										}

										string fieldRow = string.Join("\t",
											fieldTemp.InternalName,
											fieldTemp.TypeAsString,
											fieldTemp.Description,
											fieldTemp.Title,
											fieldTemp.StaticName,
											fieldTemp.Group,
											fieldTemp.Hidden, fieldTemp.Required, fieldTemp.Sealed,
											"true",//ShowInDisplayForm
											"true",//ShowInEditForm
											"true",//ShowInNewForm
											fieldTemp.Id.ToString(),
											choices
											);

										lines.Append(fieldRow + Environment.NewLine);
									}
								}
							}
						}
					}

					System.IO.File.WriteAllText(@"Models\" + fileName, lines.ToString());
				}
	}
}
