using System;
using System.Collections.Generic;
using System.Linq;
using SPMeta2.Definitions;
using SPMeta2.Enumerations;
using SPMeta2.Definitions.Fields;
using SPMeta2.Definitions.ContentTypes;
using SPMeta2.Syntax.Default;

namespace Provision.Console.Models
{

	/// <summary>
	/// </summary>
	public static class Rooms
	{
		public class Fields
		{
			public static List<FieldDefinition> GetAllFieldDefinitions()
			{
				return new List<FieldDefinition>()
				{
					Office,
					DateRequested,
					ContactPerson,
					PhoneNumber,
					DateOfUse,
					StartTime,
					EndTime,
					UseOfRoomOrChamber,
					NumberOfAttendees,
					Purpose, 
					AcFrom,
					AcTo,
					OptionalApproverRoom,

					OptionalApproval,
					OptionalApproverComments,
					OptionalRoomApprover,
					OptionalApproverDate,
					Status
				};
			}

			public static List<string> GetAllFieldNames()
			{
				var fieldNames = GetAllFieldDefinitions().Select(x => x.InternalName).ToList();
				return fieldNames;
			}

			public static List<Guid> GetAllFieldIds()
			{
				var fieldIds = GetAllFieldDefinitions().Select(x => x.Id).ToList();
				return fieldIds;
			}
			public static List<FieldLinkValue> FieldLinkValues()
			{
				var fieldIds = GetAllFieldDefinitions().Select(x => new FieldLinkValue { Id = x.Id }).ToList();
				return fieldIds;
			}

			public static FieldDefinition Office = new FieldDefinition
			{
				Title = "Office",
				InternalName = "OfficeRoom",
				Id = new Guid("03445b67-01f5-4887-9e3e-2702dfb8c318"),
				FieldType = BuiltInFieldTypes.Text,
				Group = "Senate",
				//Required = true
			};

			public static DateTimeFieldDefinition DateRequested = new DateTimeFieldDefinition
			{
				Title = "Date Requested",
				InternalName = "DateRequested",
				Id = new Guid("07744367-01f5-47d7-9e3e-2712dfb8c318"),
				//DisplayFormat = BuiltInDateTimeFieldFormatType.DateOnly,
				Group = "Senate",
				
			};

			public static FieldDefinition ContactPerson = new FieldDefinition
			{
				Title = "Contact Person",
				InternalName = "ContactPerson",
				Id = new Guid("07777b67-0445-3337-9e3e-2712dfb8c318"),
				FieldType = BuiltInFieldTypes.Text,
				//Required = true
				Group = "Senate",
			};

			public static FieldDefinition PhoneNumber = new FieldDefinition
			{
				Title = "Phone Number",
				InternalName = "PhoneNumber",
				Id = new Guid("07777767-01f5-47d7-9e3e-27144fb33318"),
				FieldType = BuiltInFieldTypes.Text,
				Group = "Senate",
			};

			public static DateTimeFieldDefinition DateOfUse = new DateTimeFieldDefinition
			{
				Title = "Date of Use",
				InternalName = "DateOfUse",
				Id = new Guid("33774477-01f5-47d7-9e3e-2712dfb8c318"),
				//DisplayFormat = BuiltInDateTimeFieldFormatType.DateOnly,
				Group = "Senate",

			};

			public static DateTimeFieldDefinition StartTime = new DateTimeFieldDefinition
			{
				Title = "Start Time",
				InternalName = "StartTime",
				Id = new Guid("06445557-33f5-47d7-9e2e-2702dfb8c218"),
				//DisplayFormat = BuiltInDateTimeFieldFormatType.DateTime,
				Group = "Senate",
			};

			public static DateTimeFieldDefinition EndTime = new DateTimeFieldDefinition
			{
				Title = "EndT ime",
				InternalName = "EndTime",
				Id = new Guid("06975557-33f5-47d7-3344-2744dfb8c218"),
				//DisplayFormat = BuiltInDateTimeFieldFormatType.DateTime,
				Group = "Senate",
			};

			public static ChoiceFieldDefinition UseOfRoomOrChamber = new ChoiceFieldDefinition
			{
				Title = "Use of Room/Chamber",
				InternalName = "UseOfRoomOrChamber",
				Id = new Guid("06975b67-0335-47d7-9e3e-2442dfb8c318"),
				Choices = new System.Collections.ObjectModel.Collection<string>()
				{ "Room 016",
				  "Room 224",
				  "Room 225",
				  "Room 229",
				  "Room 414",
				  "Room 211 (requires approval from WAM)",
				  "Chamber (requires Sen. President's approval)",
				  "Majority Caucus Room (requires Majority Caucus leader's approval)",
				  "Minority Caucus Room (requires Minority Caucus leader's approval)"
				},
			 
				Group = "Senate",
			};

			public static FieldDefinition NumberOfAttendees = new FieldDefinition
			{
				Title = "Number of Attendees",
				InternalName = "NumberOfAttendees",
				Id = new Guid("01175b67-0115-47d7-9e2e-2733dfb8c448"),
				FieldType = BuiltInFieldTypes.Number,
				Group = "Senate",
			};

			public static DateTimeFieldDefinition AcFrom = new DateTimeFieldDefinition
			{
				Title = "AC From",
				InternalName = "AcFrom",
				Id = new Guid("01973447-01f5-47d7-9e2e-2702dfb8c218"),
				//DisplayFormat = BuiltInDateTimeFieldFormatType.DateTime,
				Group = "Senate",
			};

			public static DateTimeFieldDefinition AcTo = new DateTimeFieldDefinition
			{
				Title = "AC To",
				InternalName = "AcTo",
				Id = new Guid("01374437-01f5-47d7-9e2e-2702dfb8c218"),
				//DisplayFormat = BuiltInDateTimeFieldFormatType.DateTime,
				Group = "Senate",
			};

			public static FieldDefinition OptionalApproverRoom = new FieldDefinition
			{
				Title = "Optional Approver",
				InternalName = "OptionalApproverRoom",
				Id = new Guid("03366667-01f5-47d7-9e2e-27044f44c218"),
				FieldType = BuiltInFieldTypes.User,
				Group = "Senate",
			};

			public static FieldDefinition Purpose = new FieldDefinition
			{
				Title = "Purpose",
				InternalName = "PurposeRoom",
				Id = new Guid("01374437-01f5-47d7-9e2e-2702dfb8c218"),
				FieldType = BuiltInFieldTypes.Note,
				Group = "Senate",
			};

			public static DateTimeFieldDefinition ApproverDate = new DateTimeFieldDefinition
			{
				Title = "Approver Date",
				InternalName = "ApproverDate",
				Id = new Guid("06675b67-01f5-47d7-9e2e-27033fb8c218"),
				//DisplayFormat = BuiltInDateTimeFieldFormatType.DateOnly,
				Group = "Senate",
			};

			public static ChoiceFieldDefinition OptionalApproval = new ChoiceFieldDefinition
			{
				Title = "Optional Approval",
				InternalName = "OptionalApproval",
				Id = new Guid("06665b67-33f5-47d7-9e2e-2702dfb8c218"),
				Choices = new System.Collections.ObjectModel.Collection<string>() { "Approved", "Declined" },
				EditFormat = "RadioButtons",
				DefaultValue = "",
				Group = "Senate",
			};

			public static FieldDefinition OptionalApproverComments = new FieldDefinition
			{
				Title = "Optional Approver Comments",
				InternalName = "OptionalApproverComments",
				Id = new Guid("06666b67-33f5-47d7-9e2e-2702dfb8c218"),
				FieldType = BuiltInFieldTypes.Note,
				Group = "Senate",
			};

			public static FieldDefinition OptionalRoomApprover = new FieldDefinition
			{
				Title = "Optional Approver",
				InternalName = "OptionalRoomApprover",
				Id = new Guid("03366667-01f5-47d7-9e2e-2702dfb8c218"),
				FieldType = BuiltInFieldTypes.Text,
				Group = "Senate",
			};

			public static DateTimeFieldDefinition OptionalApproverDate = new DateTimeFieldDefinition
			{
				Title = "Optional Approver Date",
				InternalName = "OptionalApproverDate",
				Id = new Guid("06675b67-01f5-47d7-9e2e-33033338c218"),
				//DisplayFormat = BuiltInDateTimeFieldFormatType.DateOnly,
				Group = "Senate",
			};


			public static ChoiceFieldDefinition RoomApproval = new ChoiceFieldDefinition
			{
				Title = "Room Approval",
				InternalName = "RoomApproval",
				Id = new Guid("05565b67-33f5-47d7-9e2e-2702dfb8c218"),
				Choices = new System.Collections.ObjectModel.Collection<string>() { "Approved", "Declined" },
				EditFormat = "RadioButtons",
				DefaultValue = "",
				Group = "Senate",
			};

			public static FieldDefinition RoomApproverComments = new FieldDefinition
			{
				Title = "Room Approver Comments",
				InternalName = "RoomApproverComments",
				Id = new Guid("06665567-33f5-47d7-9e2e-2702dfb8c218"),
 				FieldType = BuiltInFieldTypes.Note,
				Group = "Senate",
			};

			public static FieldDefinition RoomApprover = new FieldDefinition
			{
				Title = "Room Approver",
				InternalName = "RoomApprover",
				Id = new Guid("03366655-01f5-47d7-9e2e-2702dfb8c218"),
				FieldType = BuiltInFieldTypes.Text,
				Group = "Senate",
			};

			public static DateTimeFieldDefinition RoomApproverDate = new DateTimeFieldDefinition
			{
				Title = "Room Approver Date",
				InternalName = "RoomApproverDate",
				Id = new Guid("06675567-01f5-47d7-9e2e-33033338c218"),
				//DisplayFormat = BuiltInDateTimeFieldFormatType.DateOnly,
				Group = "Senate",
			};

			//--------------------
			public static ChoiceFieldDefinition AccountingApproval = new ChoiceFieldDefinition
			{
				Title = "Accounting Approval",
				InternalName = "AccountingApproval",
				Id = new Guid("04465b67-33f5-47d7-9e2e-2702dfb8c218"),
				Choices = new System.Collections.ObjectModel.Collection<string>() { "Approved", "Declined" },
				EditFormat = "RadioButtons",
				DefaultValue = "",
				Group = "Senate",
			};


	   

			public static FieldDefinition Status = new FieldDefinition
			{
				Title = "Status",
				InternalName = "WFStatus",
				Id = new Guid("04465b67-33f5-47d7-9323-2702dfb8c218"),
				FieldType = BuiltInFieldTypes.Text,
				Group = "Senate",
			};

		}
		public class ContentTypes
		{
			public static string ConferenceRoomRequests = "Conference Room Request";
			public static ContentTypeDefinition ConferenceRoomRequestsCT = new ContentTypeDefinition
			{
				Name = ConferenceRoomRequests,
				Id = new Guid("79658c1e-3096-4c44-b335-4338d01a5b93"),
				ParentContentTypeId = BuiltInContentTypeId.Item,
				Group = "Senate"
			};
		}

		public class List
		{
			public static string ListName = "Conference Room Requests";
			public static ListDefinition listDefinition = new ListDefinition
			{
				Title = ListName,
                CustomUrl = "ConferenceRoomRequests",
				Description = "Conference Room Requests",
				TemplateType = BuiltInListTemplateTypeId.GenericList,
				EnableVersioning = true,
				ContentTypesEnabled = true,
				OnQuickLaunch = true,
				EnableFolderCreation = false
			};
            public static void GetM2Model(WebModelNode webDefinition)
            {
                webDefinition.AddFields(Fields.GetAllFieldDefinitions());
                webDefinition.AddContentType(ContentTypes.ConferenceRoomRequestsCT, contentType =>
                {
                    contentType
                     .AddContentTypeFieldLinks(Fields.GetAllFieldDefinitions())
                     .AddUniqueContentTypeFieldsOrder(new UniqueContentTypeFieldsOrderDefinition
                     {
                         Fields = Fields.FieldLinkValues()
                     });
                });
                webDefinition.AddList(listDefinition, list =>
                {
                    list
                    .AddContentTypeLink(ContentTypes.ConferenceRoomRequestsCT)
                    .AddUniqueContentTypeOrder(new UniqueContentTypeOrderDefinition
                    {
                        ContentTypes = new List<ContentTypeLinkValue>
                          {
                            new ContentTypeLinkValue{ ContentTypeName = ContentTypes.ConferenceRoomRequestsCT.Name},
                          }
                    })
                //.AddListView(deploymentDefaultView)
                //.AddListView(deploymentFullView)
                ;
                });
            }

        }
	}
}