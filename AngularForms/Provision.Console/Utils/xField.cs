using SPMeta2.Definitions;
using SPMeta2.Definitions.Fields;
using SPMeta2.Enumerations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provision.Console.Utils
{
	public class xField
	{
		public string Type { get; set; }
		public string Name { get; set; }
		public string DisplayName { get; set; }
		public List<string> Choices { get; set; }
		public string Description { get; set; }
		public string Group { get; set; }
		public Guid id { get; set; }
		public bool Hidden { get; set; }
		public bool Required { get; set; }
		private string AngularStringTemplate = "<div class='col-md-4'><!--{0} | Type: {1} --><h4>{{{{f.{2}.FieldDisplayName}}}}</h4>{3}</div>";

		private string AngularTextField = "<input type='text' id='{0}' ng-model='f.{0}.Value' class='full-width' />";
		private string AngularNoteField = "<textarea ng-model='f.{0}.Value'></textarea>";
		private string AngularNumberField = "<input type='number' ng-model='f.{0}.Value' min='0' id='{0}' />";
		private string AngularTimeField = "<input type='time' ng-model='f.{0}.Value' id='{0}' />";
		private string AngularDateTimeField = "<datetime-picker format='calendarFormat' model='f.{0}.Value' />";
		private string AngularChoiceField = "<choice-field field='f.{0}' id='{0}' id='{0}' class='choice-field'> </choice-field>";
		private string AngularRadioField = "<radio-field field='f.{0}' id='{0}' id='{0}'> </choice-field>";
		private string AngularUserField = "<div ng-model='f.{0}.user' id='{0}' ui-people pp-is-multiuser='{{false}}' pp-width='220px' pp-account-type='User,DL,SecGroup,SPGroup'> </div>";
		private string AngularBooleanField = "<input type='checkbox' ng-model='f.{0}.Value' id='{0}' />";

		public string AngularView { get; set; }

		public FieldDefinition definition
		{
			get
			{
				FieldDefinition fieldDefinition = null;

				switch (Type)
				{
					case "Text":
						fieldDefinition = new FieldDefinition();
                  fieldDefinition.FieldType = BuiltInFieldTypes.Text;

						AngularView = string.Format(AngularTextField,Name);
                  break;
					case "Note":
						fieldDefinition = new FieldDefinition();
						fieldDefinition.FieldType = BuiltInFieldTypes.Note;
						AngularView = string.Format(AngularNoteField, Name);
						break;

					case "Number":
						fieldDefinition = new FieldDefinition();
						fieldDefinition.FieldType = BuiltInFieldTypes.Number;
						AngularView = string.Format(AngularNumberField, Name);
						break;

					case "Choice":
						ChoiceFieldDefinition choiceFieldDefinition = new ChoiceFieldDefinition();
						choiceFieldDefinition.Choices = new Collection<string>(Choices);
						//choiceFieldDefinition.EditFormat = "RadioButtons",
						choiceFieldDefinition.DefaultValue = "";
						fieldDefinition = choiceFieldDefinition;
						AngularView = string.Format(AngularChoiceField, Name);
						break;

					case "Radio":
						ChoiceFieldDefinition radioFieldDefinition = new ChoiceFieldDefinition();
						radioFieldDefinition.Choices = new Collection<string>(Choices);
						radioFieldDefinition.EditFormat = "RadioButtons";
						radioFieldDefinition.DefaultValue = "";
						fieldDefinition = radioFieldDefinition;
						AngularView = string.Format(AngularRadioField, Name);
						break;

					case "DateTime":
						DateTimeFieldDefinition dateTimeFieldDefinition = new DateTimeFieldDefinition();
						//dateTimeFieldDefinition.DisplayFormat = BuiltInDateTimeFieldFormatType.DateOnly,
						fieldDefinition = dateTimeFieldDefinition;
						AngularView = string.Format(AngularDateTimeField, Name);
						break;

					case "Time":
						DateTimeFieldDefinition TimeFieldDefinition = new DateTimeFieldDefinition();
						//dateTimeFieldDefinition.DisplayFormat = BuiltInDateTimeFieldFormatType.DateOnly,
						fieldDefinition = TimeFieldDefinition;
						AngularView = string.Format(AngularTimeField, Name);
						break;

					case "User":
						fieldDefinition = new FieldDefinition();
						fieldDefinition.FieldType = BuiltInFieldTypes.User;
						AngularView = string.Format(AngularUserField, Name);
						break;

					case "Boolean":
						fieldDefinition = new FieldDefinition();
						fieldDefinition.FieldType = BuiltInFieldTypes.Boolean;
						AngularView = string.Format(AngularBooleanField, Name);
						break;

					default:

						break;
				}

				AngularView = string.Format(AngularStringTemplate, DisplayName, Type, Name, AngularView);

				fieldDefinition.Title = DisplayName;
				fieldDefinition.InternalName = Name;
				fieldDefinition.Id = id;
				fieldDefinition.Group = Group;
				fieldDefinition.Required = Required;

				return fieldDefinition;
			}
		}

		public xField(string[] csvColumns)
		{
			Name = csvColumns[0];
			Type = csvColumns[1];
			Description = csvColumns[2];
			DisplayName = csvColumns[3];
			Group = csvColumns[5];
			Hidden = bool.Parse(csvColumns[6]);
			Required = bool.Parse(csvColumns[7]);
			id = Guid.Parse(csvColumns[12]);
			Choices = csvColumns[13].Split('#').ToList();
		}


	}
}
