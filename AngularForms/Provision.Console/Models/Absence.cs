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
using Provision.Console.Models;

namespace Provision.Console
{

	/// <summary>
	/// </summary>
	public static class Absence
	{
		public class Fields
		{
			public static List<FieldDefinition> GetAllFieldDefinitions()
			{
				return new List<FieldDefinition>()
				{
					Requestor,

					Manager,
					OptionalApproverUser,

					StartDate,
					EndDate,
					NumberOfDays,
					NumberOfDaysFraction,
					TypeOfLeave,
					WithPay,

					ManagerApproval,
					ApproverComments,
				
					ApproverDate,

					OptionalApproval,
					OptionalApproverComments,
					OptionalApproverText,
					OptionalApproverDate,

					AccountingApproval,
					AccountingComments,
					AccountingApprover,
					AccountingApproverDate,
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

            public static  FieldDefinition Requestor = new  FieldDefinition
            {
	            Title = "Requestor",
	            InternalName = "Requestor",
	            Id = new Guid("03375b67-01f5-4887-9e3e-2702dfb8c318"),
	            FieldType = BuiltInFieldTypes.User,
	            Group = "Angular",
	            //Required = true
            };

            public static FieldDefinition Manager = new FieldDefinition
            {
	            Title = "Manager",
	            InternalName = "Manager",
	            Id = new Guid("07733367-01f5-47d7-9e3e-2712dfb8c318"),
	            FieldType = BuiltInFieldTypes.User,
            Group = "Angular",
            };
		
            public static DateTimeFieldDefinition StartDate = new DateTimeFieldDefinition
            {
	            Title = "Start Date",
	            InternalName = "StartDateAngular",
	            Id = new Guid("07777767-01f5-47d7-9e3e-2712dfb33318"),
	            DisplayFormat = BuiltInDateTimeFieldFormatType.DateOnly,
	            Group = "Angular",
            };

            public static DateTimeFieldDefinition EndDate = new DateTimeFieldDefinition
            {
	            Title = "End Date",
	            InternalName = "EndDateAngular",
	            Id = new Guid("33777777-01f5-47d7-9e3e-2712dfb8c318"),
	            DisplayFormat = BuiltInDateTimeFieldFormatType.DateOnly,
	            Group = "Angular",
            };

            public static NumberFieldDefinition NumberOfDays = new NumberFieldDefinition
            {
	            Title = "Number of Days",
	            InternalName = "NumberOfDays",
	            Id = new Guid("06975557-33f5-47d7-9e2e-2702dfb8c218"),
	            Group = "Angular",
            };

            public static ChoiceFieldDefinition NumberOfDaysFraction = new ChoiceFieldDefinition
            {
	            Title = "Number of Days",
	            InternalName = "NumberOfDaysFraction",
	            Id = new Guid("06975557-33f5-47d7-332e-2702dfb8c218"),
	            Choices = new System.Collections.ObjectModel.Collection<string>() {"","0.25", "0.5", "0.75" },
	            DefaultValue = "",
	            Group = "Angular",
            };

            public static ChoiceFieldDefinition TypeOfLeave = new ChoiceFieldDefinition
            {
	            Title = "Type of Leave",
	            InternalName = "TypeOfLeave",
	            Id = new Guid("06975b67-0335-47d7-9e3e-2702dfb8c318"),
	            Choices = new System.Collections.ObjectModel.Collection<string>() {"Vacation", "Sick", "Education" },
	            DefaultValue = "",
	            Group = "Angular",
            };

            public static ChoiceFieldDefinition WithPay = new ChoiceFieldDefinition
            {
	            Title = "With Pay",
	            InternalName = "WithPay",
	            Id = new Guid("01175b67-0115-47d7-9e2e-2733dfb8c218"),
	            Choices = new System.Collections.ObjectModel.Collection<string>() {"Yes", "No" },
	            EditFormat = "RadioButtons",
            DefaultValue = "Yes",
            Group = "Angular",
            };

            public static ChoiceFieldDefinition ManagerApproval = new ChoiceFieldDefinition
            {
	            Title = "Manager Approval",
	            InternalName = "ManagerApproval",
	            Id = new Guid("01973367-01f5-47d7-9e2e-2702dfb8c218"),
	            Choices = new System.Collections.ObjectModel.Collection<string>() {"Approved", "Declined" },
	            EditFormat = "RadioButtons",
            DefaultValue = "",
	            Group = "Angular",
            };

            public static FieldDefinition ApproverComments = new FieldDefinition
            {
	            Title = "Approver Comments",
	            InternalName = "ApproverComments",
	            Id = new Guid("01375337-01f5-47d7-9e2e-2702dfb8c218"),
	            FieldType = BuiltInFieldTypes.Note,
	            Group = "Angular",
            };


            public static DateTimeFieldDefinition ApproverDate = new DateTimeFieldDefinition
            {
	            Title = "Approver Date",
	            InternalName = "ApproverDate",
	            Id = new Guid("06675b67-01f5-47d7-9e2e-27033fb8c218"),
	            DisplayFormat = BuiltInDateTimeFieldFormatType.DateOnly,
	            Group = "Angular",
            };

            public static ChoiceFieldDefinition OptionalApproval = new ChoiceFieldDefinition
            {
	            Title = "Optional Approval",
	            InternalName = "OptionalApproval",
	            Id = new Guid("06665b67-33f5-47d7-9e2e-2702dfb8c218"),
	            Choices = new System.Collections.ObjectModel.Collection<string>() { "Approved", "Declined" },
	            EditFormat = "RadioButtons",
	            DefaultValue = "",
	            Group = "Angular",
            };
			
            public static  FieldDefinition OptionalApproverComments = new  FieldDefinition
            {
	            Title = "Optional Approver Comments",
	            InternalName = "OptionalApproverComments",
	            Id = new Guid("06666b67-33f5-47d7-9e2e-2702dfb8c218"),
	            FieldType = BuiltInFieldTypes.Note,
	            Group = "Angular",
            };

            public static FieldDefinition OptionalApproverUser = new FieldDefinition
            {
	            Title = "Optional Approver as a User",
	            InternalName = "OptionalApproverUser",
	            Id = new Guid("03366667-01f5-47d7-9e2e-27044fb8c218"),
	            FieldType = BuiltInFieldTypes.User,
	            Group = "Angular",
            };

            public static FieldDefinition OptionalApproverText = new FieldDefinition
            {
	            Title = "Optional Approver as Text",
	            InternalName = "OptionalApproverText",
	            Id = new Guid("03366667-01f5-47d7-9e2e-4402dfb8c218"),
	            FieldType = BuiltInFieldTypes.Text,
	            Group = "Angular",
            };

            public static DateTimeFieldDefinition OptionalApproverDate = new DateTimeFieldDefinition
            {
	            Title = "Optional Approver Date",
	            InternalName = "OptionalApproverDate",
	            Id = new Guid("06675b67-01f5-47d7-9e2e-33033338c218"),
	            DisplayFormat = BuiltInDateTimeFieldFormatType.DateOnly,
	            Group = "Angular",
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
	            Group = "Angular",
            };


            public static FieldDefinition AccountingComments = new FieldDefinition
            {
	            Title = "Accounting Approver Comments",
	            InternalName = "AccountingApproverComments",
	            Id = new Guid("06666447-33f5-47d7-9e2e-2702dfb8c218"),
	            FieldType = BuiltInFieldTypes.Note,
	            Group = "Angular",
            };

            public static FieldDefinition AccountingApprover = new FieldDefinition
            {
	            Title = "Accounting Approver",
	            InternalName = "AccountingApprover",
	            Id = new Guid("03366667-01f5-47d7-442e-2702dfb8c218"),
	            FieldType = BuiltInFieldTypes.Text,
	            Group = "Angular",
            };

            public static DateTimeFieldDefinition AccountingApproverDate = new DateTimeFieldDefinition
            {
	            Title = "Accounting Approver Date",
	            InternalName = "AccountingApproverDate",
	            Id = new Guid("06675b67-01f5-47d7-9e2e-44033338c218"),
	            DisplayFormat = BuiltInDateTimeFieldFormatType.DateOnly,
	            Group = "Angular",
            };

            public static FieldDefinition Status = new FieldDefinition
            {
	            Title = "Status",
	            InternalName = "WFStatus",
	            Id = new Guid("04465b67-33f5-47d7-9323-2702dfb8c218"),
	            FieldType = BuiltInFieldTypes.Text,
	            Group = "Angular",
            };

        }// end of Fields class

        public class ContentTypes
        {
            public static string LeaveOfAbsence = "Leave of absence";
            public static ContentTypeDefinition LeaveOfAbcenceCT = new ContentTypeDefinition
            {
	            Name = LeaveOfAbsence,
	            Id = new Guid("79658c1e-3096-4c44-b335-4338d01a5b91"),
	            ParentContentTypeId = BuiltInContentTypeId.Item,
	            Group = "Angular"
            };
        }

        public class List
        {
            public static string ListName = "Leave of Absence";
            public static ListDefinition listDefinition = new ListDefinition
            {
                Title = ListName,
                CustomUrl = "LeaveofAbsence",
                Description = "Leave of absence",
                TemplateType = BuiltInListTemplateTypeId.GenericList,
                EnableVersioning = true,
                ContentTypesEnabled = true,
                OnQuickLaunch = true,
                EnableFolderCreation = false                
            };

            public static void GetM2Model(WebModelNode webDefinition)
            {
                webDefinition.AddFields(Fields.GetAllFieldDefinitions());
                webDefinition.AddContentType(ContentTypes.LeaveOfAbcenceCT, contentType =>
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
                    .AddContentTypeLink(ContentTypes.LeaveOfAbcenceCT)
                    .AddUniqueContentTypeOrder(new UniqueContentTypeOrderDefinition
                    {
                        ContentTypes = new List<ContentTypeLinkValue>
                          {
                            new ContentTypeLinkValue{ ContentTypeName = ContentTypes.LeaveOfAbcenceCT.Name},
                            new ContentTypeLinkValue{ ContentTypeName = Rooms.ContentTypes.ConferenceRoomRequestsCT.Name},
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