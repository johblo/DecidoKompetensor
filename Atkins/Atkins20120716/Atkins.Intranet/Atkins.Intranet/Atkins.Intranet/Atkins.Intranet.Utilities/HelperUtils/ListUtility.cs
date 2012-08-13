using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using System.Linq;
using System.Reflection;

namespace Atkins.Intranet.Utilities.HelperUtils
{
    public class CustomListHelper
    {

        public static void SetFieldDisplayName(SPField field, string displayName)
        {
            Type baseType = field.GetType().BaseType;
            object obj = baseType.InvokeMember("SetFieldAttributeValue", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, field, new object[] { "DisplayName", displayName });
            //field.Update();
        }

        public static string CreateSiteColumn(SPWeb currentWeb, string columnName, SPFieldType columnType, bool required)
        {
            string fieldInternalName = ReturnTrimmedString(columnName);
            if (!currentWeb.Fields.ContainsField(fieldInternalName))
            {
                fieldInternalName = currentWeb.Fields.Add(fieldInternalName, columnType, required);
                currentWeb.Update();
            }
            return fieldInternalName;
        }

        public static string CreateTaxonomySiteColumn(SPSite site, string columnName)
        {
            string fieldInternalName = ReturnTrimmedString(columnName);
            
            if (!site.RootWeb.Fields.ContainsField(fieldInternalName))
            {
                TaxonomyField field = site.RootWeb.Fields.CreateNewField("TaxonomyFieldType", fieldInternalName) as TaxonomyField;
               
                site.RootWeb.Fields.Add(field);
                site.RootWeb.Update();
            }
            return fieldInternalName;
        }
      
        public static bool ListContainsContentType(SPList list, SPContentTypeId id)
        {
            SPContentTypeId matchId = list.ContentTypes.BestMatch(id);
            return matchId.IsChildOf(id);
        }

        public static bool SiteContainsContentType(SPWeb rootWeb, SPContentTypeId id)
        {
            SPContentTypeId matchId = rootWeb.ContentTypes.BestMatch(id);
            return matchId.IsChildOf(id);
        }

        public static string ReturnTrimmedString(string input)
        {
            return input.Replace(" ", "");
        }

        public static string ReturnUserDisplayName(object userField)
        {
            string userDisplayName = string.Empty;
            if (userField != null)
            {
                int username = Int32.Parse(userField.ToString().Split(';')[0]);
                SPUser user = SPContext.Current.Web.AllUsers.GetByID(username);
                if (user != null)
                {
                    userDisplayName = user.Name;
                }
            }

            return userDisplayName;
        }

        public static SPField ReturnListField(SPList list , String fieldTitle)
        {
            SPField field=  list.Fields.TryGetFieldByStaticName(ReturnTrimmedString(fieldTitle));
            return field;
        }

        public  static SPList ReturnList(SPWeb listWeb ,string listTitle)
        {
            SPList targetList = listWeb.Lists.TryGetList(listTitle);
            if(targetList == null)
            {
                targetList = listWeb.Lists.TryGetList(ReturnTrimmedString(listTitle));
            }

            return targetList;
        }
    }

    public class OfficeFields
    {
        public const string ListName = "Offices";
        public const string ListDescription = "Contains information about Atkins multiple offices across the country.";
        public const string ListContentType = "Atkins Office Information";
        public const string AtkinsContentTypeGroup = "Atkins Content Types";

        public const string Title = "Title";
        public const string TitleDisplayName = "Title";
        public const string Address = "Address";
        public const string AddressDisplayName = "Address";
        public const string Zip = "Zip";
        public const string ZipDisplayName = "Zip";
        public const string City = "City";
        public const string CityDisplayName = "City";
        public const string PhoneNumber = "Phone Number";
        public const string PhoneNumberDisplayName = "Phone Number";
        public const string FaxNumber = "Fax Number";
        public const string FaxNumberDisplayName = "Fax Number";

        public static readonly SPContentTypeId OfficeContentTypeId = new SPContentTypeId("0x0100A33D9AD9805788419BDAAC2CCB37509F");
    }

    public class IntroductionTemplateFields
    {
        public const string ListName = "Introduction Templates";
        public const string ListDescription = "Contains the steps required to be taken in each introduction template.";
        public const string ListContentType = "Atkins Introduction Template";
        public const string AtkinsContentTypeGroup = "Atkins Content Types";

        public const string Title = "Title";
        public const string TitleDisplayName = "Title";
        public const string TemplateSteps = "Introduction Steps";
        public const string TemplateStepsDisplayName = "Introduction Steps";
        public const string TemplateIsActive = "Active";
        public const string TemplateIsActiveDisplayName = "Active";
        public const string TemplateIsActiveXML = "TemplateIsActive";
        public const string TemplateIsActiveXMLDisplayName = "TemplateIsActive";

        public static readonly SPContentTypeId TemplateStepsContentTypeId = new SPContentTypeId("0x0100A33D9AD9805788419BDAAC2CCB37509E");
    }


    public class EmployeeContactFields
    {
        public const string ListName = "Employees";
        public const string ListDescription = "Contains the personnel information of new employees";
        public const string ListContentType = "Atkins Employee Contact";
        public const string AtkinsContentTypeGroup = "Atkins Content Types";

        public const string Title = "Title";
        public const string TitleDisplayName = "Title";
        public const string PersonalNumber = "Personal Number";
        public const string PersonalNumberDisplayName = "Personal Number";
        public const string Office = "Office";
        public const string OfficeDisplayName = "Office";
        public const string Position = "Position";
        public const string PositionDisplayName = "Position";
        public const string Manager = "Manager";
        public const string ManagerDisplayName = "Manager";
        public const string HR_Responsible = "HR Responsible";
        public const string HR_ResponsibleDisplayName = "HR Responsible";
        public const string Mentor = "Mentor";
        public const string MentorDisplayName = "Mentor";
        public const string IntroductionTemplate = "Template";
        public const string IntroductionTemplateDisplayName = "Template";
        //webPartView
        public const string webPartTitle = "Anställda";
        public const string webPartView = "webPartView";

        public static readonly SPContentTypeId EmployeeContentTypeId = new SPContentTypeId("0x0100A33D9AD9805788419BDAAC2CCB37508E");

        

        public const string CustomDisplayFormUrl = "/_layouts/Atkins.Intranet.Hr/ContactDispForm.aspx";
    }

    public class IntroductionTasksFields
    {
        public const string ListName = "Template Tasks";
        public const string ListDescription = "Contains introduction tasks created for new employees.";
        public const string ListContentType = "Atkins Introduction Task";
        public const string AtkinsContentTypeGroup = "Atkins Content Types";

        public const string TaskAssignee = "Assignee";
        public const string TaskAssigneeDisplayName = "Assignee";
        public const string DueDate = "DueDateIntroductionTask";
        public const string DueDateDisplayName = "Due Date";
        
        //public const string DueDateInternalName = "Deadline";

        public const string Completed = "CompletedStatus";
        public const string CompletedDisplayName = "Completed";
        //public const string CompletedInternalName = "CompletedStatus";

        public const string CompletionDate = "CompletionDate";
        public const string CompletionDateDisplayName = "Completion Date";
        public const string Employee = "EmployeeName";
        public const string EmployeeDisplayName = "Employee Name";
        public const string webPartTitle = "Mina uppgifter";
        public const string webPartView = "webPartView";
        public const string webPartViewDisplayName = "webPartView";


        public static readonly SPContentTypeId TaskContentTypeId = new SPContentTypeId("0x0100A33D9AD9805788419BDAAC2CCB37502E");
                                                                                        
    }

    public class EmployeeDocuments
    {
        public const string ListName = "Employee Documents";
        public const string ListDescription = "Contains introduction documents erlated to different employees.";
        public const string ListContentType = "Atkins Introduction Document";
        public const string AtkinsContentTypeGroup = "Atkins Content Types";

        public const string EmployeeName = "Employee";
        public const string EmployeeNameDisplayName = "Employee";
    }
    //PERSONAL
    public class TermStoreName
    {
        public const string TermStore = "Managed Metadata Service";
    }
    public class EmployeeHandbook
    {
        public const string ListName = "Employee Handbook";
        public const string ListDescription = "Contains information for employees";
        public const string ListContentType = "Atkins Employee Handbook";
        public const string AtkinsContentTypeGroup = "Atkins Content Types";

        public const string Title = "Title";
        public const string TitleDisplayName = "Title";
        public const string Description = "HandBook Description";
        public const string DescriptionDisplayName = "HandBook Description";
        public const string Category = "Employee Handbook Category";
        public const string CategoryDisplayName = "Category";
        public const string SubCategory = "Sub category";
        public const string SubCategoryDisplayName = "Sub category";
        public const string ValidFrom = "Valid from";
        public const string ValidFromDisplayName = "Valid from";
        public const string ValidTo = "Valid to";
        public const string ValidToDisplayName = "Valid to";

        public const string TermGroup = "HR";
        public const string TermSet = "EmployeeHandbook Categories";


        public static readonly SPContentTypeId EmployeeHandBookContentTypeId = new SPContentTypeId("0x0100C4996397BE1448FB9360F07527C9F924");
    }
    public class EmployeeHandBookDocuments
    {
        public const string ListName = "EmployeeHandBookDocuments";
        public const string ListDescription = "Contains documents related to different employee handbook items";
        public const string ListContentType = "Atkins Employee Handbook Document";
        public const string AtkinsContentTypeGroup = "Atkins Content Types";

        public const string EmployeeHandBook = "Employee HandBook";
        public const string EmployeeHandBookDisplayName = "Employee HandBook";
    }
  
//---------------------------CONTROLLING DOCUMENTS--------------------------
    public class ControllingDocuments
    {
        public const string ListName = "Controlling Documents";
        public const string ListDescription = "Controlling Documents";
        public const string ListContentType = "Atkins QSE Controlling Document";
        public const string AtkinsContentTypeGroup = "Atkins Content Types";

        public const string ISO9001 = "ISO9001";
        public const string ISO9001DisplayName = "ISO9001";
        public const string ISO14001 = "ISO14001";
        public const string ISO14001DisplayName = "ISO14001";
        public const string ISO18001 = "ISO18001";
        public const string ISO18001DisplayName = "ISO18001";
        public const string WrittenBy = "Written By";
        public const string WrittenByDisplayName = "Written By";
        public const string Auditor = "Auditor";
        public const string AuditorDisplayName = "Auditor";
        public const string Approver = "Approver";
        public const string ApproverDisplayName = "Approver";
        public const string ValidUntil = "Valid until";
        public const string ValidUntilDisplayName = "Valid until";
        public const string DocumentID = "DocumentID";
        public const string DocumentIDDisplayName = "DocumentID";

        public const string TermGroup = "QSE";
        public const string TermSetISO9001 = "ISO9001";
        public const string TermSetISO14001 = "ISO14001";
        public const string TermSetISO18001 = "ISO18001";
        
        public static readonly SPContentTypeId controllingDocumentsContentTypeId = new SPContentTypeId("0x010100D38A7116AC8D488FB894DD2ED97EA4BB");

    }
    public class QSEAuthorsGroup
    {
        public const string Name = "QSE Authors";
        public const string Description = "Contribute permission in the library. Can publish major versions.";
        public const SPRoleType role = SPRoleType.Contributor;
    }
    public class QSEAuditorssGroup
    {
        public const string Name = "QSE Auditors";
        public const string Description = "Contribute permission in the library. Can publish major versions.";
        public const SPRoleType role = SPRoleType.Contributor;
    }
    public class QSEApproverssGroup
    {
        public const string Name = "QSE Approvers";
        public const string Description = "Contribute permission in the library. Can publish major versions. ";
        public const SPRoleType role = SPRoleType.Contributor;
    }
    public class QSEAdministratorsGroup
    {
        public const string Name = "QSE Administrators";
        public const string Description = "Contribute permission in the library. Option to set higher permissions to this group.";
        public const SPRoleType role = SPRoleType.Contributor;
    }

    //---------------------------RESULTING DOCUMENTS--------------------------

    public class ResultingDocuments
    {
        public const string ListName = "Resulting Documents";
        public const string ListDescription = "Resulting Documents";
        public const string ListContentType = "Atkins QSE Resulting Document";
        public const string AtkinsContentTypeGroup = "Atkins Content Types";

        public const string ResultingDocumentCategory = "Resulting Document Category";
        public const string ResultingDocumentCategoryDisplayName = "Resulting Document Category";
        public const string ResultingDocumentYear = "Resulting Document Year";
        public const string ResultingDocumentYearDisplayName = "Resulting Document Year";
        public const int ResultingDocumentYearStart = 2008;
        public const int ResultingDocumentYearStop = 2030;

        public const string TermGroup = "QSE";
        public const string TermSetResultingDocumentCategory = "Resulting Document Category";

        public static readonly SPContentTypeId resultingDocumentsContentTypeId = new SPContentTypeId("0x0101003A2789604B6E4EAC9B33A49132E26BF1");
    }
    public class QSEResultingDocumentsAdministrators 
    {
        public const string Name = "QSE Resulting Documents Administrators";
        public const string Description = "Contribute permission in the library.";
        public const SPRoleType role = SPRoleType.Contributor;
    }
    //---------------------------PROCESS STEP LIST--------------------------
    public class ProcessStepList
    {
        public const string ListName = "Process Steps";
        public const string ListDescription = "The Process Step List is a SharePoint list that is designed to replace the current static html-tables that informs the user about the steps in each of the different defined Atkins process";
        public const string ListContentType = "Atkins QSE Prosess Step";
        public const string AtkinsContentTypeGroup = "Atkins Content Types";

        public const string Process = "Process";
        public const string ProcessDisplayName = "Process";
        public const string ProcessDescription = "Process Description";
        public const string ProcessDescriptionDisplayName = "Process Description";

        public const string TermGroup = "QSE";
        public const string TermSetProcess = "Process";

        public const string webPartView = "webPartView";
                                                                                               
        public static readonly SPContentTypeId processStepContentTypeId = new SPContentTypeId("0x01004E6C0773E340486CBEB9D9856CEF6923");
    }

    //---------------------------DEVIATION LIST--------------------------
    public class DeviationsList
    {
        public const string ListName = "Deviations and Suggestions";
        public const string ListDescription = "Deviations and Suggestions";
        public const string ListContentTypeBase = "Atkins QSE Deviations Base";
        public const string ListContentTypeDeviations = "Atkins QSE Deviations";
        public const string ListContentTypeComplaints = "Atkins QSE Complaints";
        public const string ListContentTypeSuggestions = "Atkins QSE Suggestions";
        public const string AtkinsContentTypeGroup = "Atkins Content Types";

        public const string Title = "Title";
        public const string TitleDisplayName = "Title";
        public const string KeyDate = "Key Date";
        public const string KeyDateDisplayName = "Key Date";
        public const string DeviationStatus = "Deviation Status";
        public const string DeviationStatusDisplayName = "Deviation Status";
        public const string DeviationDescription = "Deviation Description";
        public const string DeviationDescriptionDisplayName = "Deviation Description";
        public const string Responsible = "Responsible";
        public const string ResponsibleDisplayName = "Responsible";
        public const string DecisionDate = "Decision Date";
        public const string DecisionDateDisplayName = "Decision Date";
        public const string DecisionComment = "Decision Comment";
        public const string DecisionCommentDisplayName = "Decision Comment";
        public const string ActionByDate = "Action By Date";
        public const string ActionByDateDisplayName = "Action By Date";
        public const string FollowUpDate = "Follow Up Date";
        public const string FollowUpDateDisplayName = "Follow Up Date";
        public const string FollowUpComment = "Follow Up Comment";
        public const string FollowUpCommentDisplayName = "Follow Up Comment";
        
        public const string TermGroup = "QSE";
        public const string TermSetStatus = "Deviation Status";
        //WEBPART
        public const string webPartView = "webPartView";
        public const string webPartTitle = "Avvikelser";
        public static readonly SPContentTypeId DeviationBaseContentTypeId = new SPContentTypeId("0x0100E597B736AF8F410F887223B41DF23E68");
        //DB201B0B455F46CBA862ABA9FE71071F
        //2D37B3E6024745619856983299CB97BD
        //D7241B3E34B44B05833E35CB02E99AE1
        public static readonly SPContentTypeId DeviationContentTypeId = new SPContentTypeId("0x0100E597B736AF8F410F887223B41DF23E6800DB201B0B455F46CBA862ABA9FE71071F");
        public static readonly SPContentTypeId ComplaintsContentTypeId = new SPContentTypeId("0x0100E597B736AF8F410F887223B41DF23E68002D37B3E6024745619856983299CB97BD");
        public static readonly SPContentTypeId SuggestionsContentTypeId = new SPContentTypeId("0x0100E597B736AF8F410F887223B41DF23E6800D7241B3E34B44B05833E35CB02E99AE1");

        public const string DeviationRoleDefinition = "Deviation Role Definition";
    }
    //---------------------------TEMPLATE DOCUMENT LIBRARY LIST--------------------------
    public class TemplateDocuments
    {
        public const string ListName = "Template Documents";
        public const string ListDescription = "Template Documents";
        public const string ListContentType = "Atkins Template Documents";
        public const string AtkinsContentTypeGroup = "Atkins Content Types";

        public const string TemplateDocumentCategory = "Template Document Category";
        public const string TemplateDocumentCategoryDisplayName = "Template Document Category";

        public const string TermGroup = "PORTAL";
        public const string TermSetTemplateDocumentCategory = "Template Document Category";

        public static readonly SPContentTypeId templateDocumentContentTypeId = new SPContentTypeId("0x010100450A55B589644618865764033A029768");
                                                                                                            
    }
    public class TemplateDocumentAdministrators
    {
        public const string Name = "Template Document Administrators";
        public const string Description = "Contribute permission in the library.";
        public const SPRoleType role = SPRoleType.Contributor;
    }

    public class BlogPosts
    {
        public const string webPartTitle = "Blog Posts";
        public const string webPartViewFields = "Title;Body";
        public const string webPartView = "webPartView";
        public const string webpartItemStyle =  "CQWP_Blog";
        public const string ListName = "Inlägg";
        public const string xslPath = "/Sites/Intranet/Style Library/XSL Style Sheets/customItem.xsl";
        public const string BlogZoneLeft = "Vänster";
        public const string HideTitle = "Hide Title";
        public const string HideTitleContent = "<style>.s4-titletext {DISPLAY: none}</style>";

        
    }

}
