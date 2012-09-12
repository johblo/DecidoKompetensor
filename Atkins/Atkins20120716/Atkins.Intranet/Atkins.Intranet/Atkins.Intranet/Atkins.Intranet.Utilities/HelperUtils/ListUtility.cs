using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using System.Linq;
using System.Reflection;
using System.Collections.Specialized;
using System.Xml;
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
        public static string CreateView(SPWeb currentWeb, string listName, string viewName, string[] viewFields, string query, uint rowlimit)
        {
            SPList currentList = currentWeb.Lists.TryGetList(listName);
            if (currentList != null)
            {
                if (!checkIfViewExist(currentList, viewName))
                {
                    StringCollection vf = new StringCollection();
                    vf.AddRange(viewFields);
                    SPView newView = currentList.Views.Add(viewName, vf, query, rowlimit, false, false);
                    newView.Update();
                    currentList.Update();
                    return newView.Title;
                }
            }
            return "";
        }
        public static string CreateView(SPList currentList,string viewName, string[] viewFields, string query, uint rowlimit)
        {
            StringCollection vf = new StringCollection();
            vf.AddRange(viewFields);
            SPView newView = currentList.Views.Add(viewName, vf, query, rowlimit, false, false);
            newView.Toolbar = "";
            newView.TabularView = false;
            newView.Update();
            currentList.Update();
            return newView.Title;
        }

        public static bool checkIfViewExist(SPList currentList, string name)
        {
            bool exist = false;
            foreach (SPView view in currentList.Views)
            {
                if (view.Title == name)
                {
                    exist = true;
                    break;
                }
            }
            return exist;
        }
        public static string[] returnStringArray(string input)
        {
            return input.Split(new char[] { ',' });
        }
    }
    //***************************************************************************STARTSITE**************************************************************************************
    //OFFICES LIST
    public class OfficeFields
    {
        public const string ListName = "Kontor";
        public const string ListDescription = "Contains information about Atkins multiple offices across the country.";
        public const string ListContentType = "Atkins Office Information";
        public const string AtkinsContentTypeGroup = "Atkins Content Types";

        public const string Title = "Title";
        public const string TitleDisplayName = "Title";
        public const string Address = "Address";
        public const string AddressDisplayName = "Adress";
        public const string Zip = "Zip";
        public const string ZipDisplayName = "Postnummer";
        public const string City = "City";
        public const string CityDisplayName = "Stad";
        public const string PhoneNumber = "Phone Number";
        public const string PhoneNumberDisplayName = "Telefon";
        public const string FaxNumber = "Fax Number";
        public const string FaxNumberDisplayName = "Fax";

        public static readonly SPContentTypeId OfficeContentTypeId = new SPContentTypeId("0x0100A33D9AD9805788419BDAAC2CCB37509F");
    }
    // AREA LIST
    public class AreaList
    {
        public const string ListName = "Område";
        public const string ListDescription = "Contains information about Atkins specific areas";
        public const string ListContentType = "Atkins Area Information";
        public const string AtkinsContentTypeGroup = "Atkins Content Types";
        
        public const string TitleDisplayName = "Område";
    }


    //CALENDAR LIST
    public class CalendarStartSite
    {
        public const string ListName = "Kalender";
        public const string ListDescription = "Kalender";
        public const string TitleDisplayName = "Rubrik";
        public const string webPartTitle = "Kalender";
        public const string webPartView = "webPartView";
        public const string webPartViewFields = "LinkTitle,EventDate,EndDate";
        public const string webPartQuery = "<Where>" +
                                              "<Geq><FieldRef Name='EndDate' /><Value Type='DateTime'><Today /></Value></Geq>" +
                                           "</Where>"+
                                           "<OrderBy><FieldRef Name='EventDate' Ascending='TRUE'/></OrderBy>";
        public const int webPartRowLimit = 10;
        public const string ZoneId = "Center";
        public const string webpartTitleImageUrl = "/_layouts/images/Atkins.Intranet.Portal/icons/calendar.png";
    }
    //LINKSLIST
    public class LinksStartSite
    {
        public const string ListName = "Länkar";
        public const string ListDescription = "Länkar";

        public const string TitleDisplayName = "Länk";

        public const string activeField = "linkActive";
        public const string activeFieldDisplayName = "Visa på startsidan";

        public const string areaField = "linkArea";
        public const string areaFieldDisplayName = "Område";

        public const string webPartTitle = "Nyttiga länkar";
        public const string webPartView = "webPartView";
        public const string webPartQuery = "<Where>" +
                                             "<Eq><FieldRef Name='linkActive'/><Value Type='Boolean'>1</Value></Eq>" +
                                           "</Where>";


        public const string webPartViewFields = "URL";
        public const int webPartRowLimit = 10;
        public const string ZoneId = "Right";
        public const string webpartTitleImageUrl = "/_layouts/images/Atkins.Intranet.Portal/icons/link.png";
    }

    //PERSONAL LINKSLIST
    public class PersonalLinksStartSite
    {
        public const string ListName = "Personliga Länkar";
        public const string ListDescription = "Personliga Länkar";

        
        public const string webPartTitle = "Personliga Länkar";
        public const string webPartView = "webPartView";
        public const string webPartViewFields = "URL";

        public const string webPartQuery = "<Where>" +
                                             "<Eq><FieldRef Name='Author'/><Value Type='Integer'><UserID Type='Integer'/></Value></Eq>" +
                                           "</Where>";

        public const string ZoneId = "Right";
        public const string webpartTitleImageUrl = "/_layouts/images/Atkins.Intranet.Portal/icons/link.png";
    }

   
    public class Announcements
    {
        public const string ListName = "$Resources:core,announceList;";
        public const string webPartView = "webPartView";
        public const string webPartQuery = "<Where>" +
                                             "<Geq><FieldRef Name='Expires'/><Value Type='DateTime'><Today OffsetDays='0' /></Value></Geq>" +
                                           "</Where>";

        public const string webPartViewFields = "LinkTitle,Expires";
        public const int webPartRowLimit = 1000;
        public const string webPartTitle = "Meddelanden";
        public const string ZoneId = "Left";
        public const string webpartTitleImageUrl = "/_layouts/images/Atkins.Intranet.Portal/icons/announcement.png";
    }

    public class KpiStock
    {
        public const string webPartTitle = "KPI";
        public const string contentLink = "http://ir2.flife.de/data/atkins/share-ticker.php";
        public const string ZoneId = "Center";
        public const string webpartTitleImageUrl = "/_layouts/images/Atkins.Intranet.Portal/icons/kpi.png";
    }

    public class ManualsDocuments
    {
        public const string ListName = "Manualer";
        public const string ListDescription = "Manual Documents";
        public const string ListContentType = "Atkins Manuals Documents";
        public const string AtkinsContentTypeGroup = "Atkins Content Types";

        public const string TitleDisplayName = "Titel";
        public const string ManualsDocumentCategory = "Manual Category";
        public const string ManualsDocumentCategoryDisplayName = "Manualkategori";

        public const string ManualsDocumentDescription = "ManualComments";
        public const string ManualsDocumentDescriptionDisplayName = "Kommentarer";


        public const string TermGroup = "Atkins Sweden";
        public const string TermSetCategory = "Manual Category";
        
        public static readonly SPContentTypeId ContentTypeId = new SPContentTypeId("0x010100181BDCAB4A9542E9A4BE32D6BED2AC05");
    }



    //***************************************************************************HR HR HR HR**************************************************************************************
    public class HRblogPosts
    {
        public const string webName = "Nyheter";
        public const string webPartTitle = "Rekryteringsnyheter";
        public const string webPartViewFields = "Title;Body;PublishedDate,DateTime;";
        public const string webPartView = "webPartView";
        public const string webpartItemStyle = "CQWP_Blog";
        public const int rowLimitStartPage = 10;
        public const string ListName = "Inlägg";
        public const string xslPath = "/Sites/Intranet/Style Library/XSL Style Sheets/customItem.xsl";
        public const string ZoneId = "Right";
        public const string webpartTitleImageUrl = "/_layouts/images/Atkins.Intranet.Portal/icons/news.png";
        public const string blogpostCategories = "HR,QSE,Finans,Marknad,Kontor,IT,Teknik,Rekrytering";
       
        public const string category = "Rekrytering";
    }

    public class IntroductionTemplateFields
    {
        public const string ListName = "Introduktionsmall";
        public const string ListDescription = "Contains the steps required to be taken in each introduction template.";
        public const string ListContentType = "Atkins Introduction Template";
        public const string AtkinsContentTypeGroup = "Atkins Content Types";

        public const string Title = "Title";
        public const string TitleDisplayName = "Titel";
        public const string TemplateSteps = "Introduction Steps";
        public const string TemplateStepsDisplayName = "Introduktionsaktiviteter";
        public const string TemplateIsActive = "IntroductionActive";
        public const string TemplateIsActiveDisplayName = "Aktiv";
        public const string TemplateIsActiveXML = "TemplateIsActive";
        public const string TemplateIsActiveXMLDisplayName = "TemplateIsActive";

        public static readonly SPContentTypeId TemplateStepsContentTypeId = new SPContentTypeId("0x0100A33D9AD9805788419BDAAC2CCB37509E");
    }


    public class EmployeeContactFields
    {
        public const string ListName = "Medarbetare";
        public const string ListDescription = "Contains the personnel information of new employees";
        public const string ListContentType = "Atkins Employee Contact";
        public const string AtkinsContentTypeGroup = "Atkins Content Types";

        public const string Title = "Title";
        public const string TitleDisplayName = "Titel";
        public const string PersonalNumber = "Personal Number";
        public const string PersonalNumberDisplayName = "Anställningsnummer";
        public const string Office = "Office";
        public const string OfficeDisplayName = "Kontor";
        public const string Position = "Position";
        public const string PositionDisplayName = "Position";
        public const string Manager = "Manager";
        public const string ManagerDisplayName = "Ansvarig chef";
        public const string HR_Responsible = "HR Responsible";
        public const string HR_ResponsibleDisplayName = "HR ansvarig";
        
        public const string Mentor = "Mentor";
        public const string MentorDisplayName = "Mentor";
        public const string StartDate = "StartDate";
        public const string StartDateDisplayName = "Startdatum";

        public const string IntroductionTemplate = "Template";
        public const string IntroductionTemplateDisplayName = "Introduktionsmall";
        //webPartView
        public const string webPartTitle = "Nya Medarbetare";
        public const string webPartView = "webPartView";

        public const string webPartQuery =  "<Where>" +
                                             "<Gt><FieldRef Name='StartDate'/><Value Type='DateTime'><Today OffsetDays='-30' /></Value></Gt>" +
                                           "</Where>";
            
        public const string webPartViewFields = "LinkTitle,Position,Office,StartDate";

        public const int webPartRowLimit = 10;

        public const string ZoneId = "Right";
        public const string webpartTitleImageUrl = "/_layouts/images/Atkins.Intranet.Portal/icons/group.png";

        public static readonly SPContentTypeId EmployeeContentTypeId = new SPContentTypeId("0x0100A33D9AD9805788419BDAAC2CCB37508E");

        

        public const string CustomDisplayFormUrl = "/_layouts/Atkins.Intranet.Hr/ContactDispForm.aspx";
    }

    public class IntroductionTasksFields
    {
        public const string ListName = "Malluppgifter";
        public const string ListDescription = "Contains introduction tasks created for new employees.";
        public const string ListContentType = "Atkins Introduction Task";
        public const string AtkinsContentTypeGroup = "Atkins Content Types";

        public const string TitleDisplayName = "Aktivitet";

        public const string TaskAssignee = "Assignee";
        public const string TaskAssigneeDisplayName = "Tilldelad";
        public const string DueDate = "DueDateIntroductionTask";
        public const string DueDateDisplayName = "Förfallodag";
        
        public const string Completed = "CompletedStatus";
        public const string CompletedDisplayName = "Slutförd";

        public const string CompletionDate = "CompletionDate";
        public const string CompletionDateDisplayName = "Slutdatum";
        public const string Employee = "EmployeeName";
        public const string EmployeeDisplayName = "Medarbetarnamn";

        public const string webPartTitle = "Att göra";
        public const string webPartView = "webPartView";

        public const string webPartQuery = "<Where>" +
                                              "<And>" +
                                                "<Eq><FieldRef Name='" + IntroductionTasksFields.TaskAssignee + "'/><Value Type='Integer'><UserID Type='Integer'/></Value></Eq>" +
                                                "<Eq><FieldRef Name='" + IntroductionTasksFields.Completed + "'/><Value Type='Integer'>0</Value></Eq>" +
                                              "</And>" +
                                            "</Where>";
        public const string webPartViewFields = "LinkTitle,Assignee,CompletionDate";

        public const int webPartRowLimit = 5;

        public const string ZoneId = "Right";
        public const string webpartTitleImageUrl = "/_layouts/images/Atkins.Intranet.Portal/icons/task_my.png";

        public static readonly SPContentTypeId TaskContentTypeId = new SPContentTypeId("0x0100A33D9AD9805788419BDAAC2CCB37502E");
                                                                                        
    }

    public class EmployeeDocuments
    {
        public const string ListName = "Medarbetardokument";
        public const string ListDescription = "Contains introduction documents erlated to different employees.";
        public const string ListContentType = "Atkins Introduction Document";
        public const string AtkinsContentTypeGroup = "Atkins Content Types";

        public const string EmployeeName = "Employee";
        public const string EmployeeNameDisplayName = "Medarbetare";
    }
    //PERSONAL
    public class TermStoreName
    {
        public const string TermStore = "Managed Metadata Service";
        public const string TermGroup = "Atkins Sweden";
    }
    public class EmployeeHandbook
    {
        public const string ListName = "Personalhandbok";
        public const string ListDescription = "Contains information for employees";
        public const string ListContentType = "Atkins Employee Handbook";
        public const string AtkinsContentTypeGroup = "Atkins Content Types";

        public const string Title = "Title";
        public const string TitleDisplayName = "Titel";
        public const string Description = "HandBook Description";
        public const string DescriptionDisplayName = "Beskrivning";
        public const string Category = "Employee Handbook Category";
        public const string CategoryDisplayName = "Kategori";
        
        public const string ValidFrom = "Valid from";
        public const string ValidFromDisplayName = "Giltig f.r.o.m";
        public const string ValidTo = "Valid to";
        public const string ValidToDisplayName = "Giltig t.o.m";

        public const string TermGroup = "Atkins Sweden";
        public const string TermSet = "Employee handbook categories";


        public const string webPartTitle = "Personalhandbok";
        public const string webPartView = "webPartView";

        public const string webPartQuery = "<Where>" +
                                                "<Gt><FieldRef Name='Modified'/><Value Type='DateTime'><Today OffsetDays='-30' /></Value></Gt>" +
                                            "</Where>"+
                                            "<OrderBy><FieldRef Name='Modified' Ascending='TRUE'/></OrderBy>";
        public const string webPartViewFields = "LinkTitle,EmployeeHandbookCategory";

        public const int webPartRowLimit = 1000;

        public const string ZoneId = "Left";
        public const string webpartTitleImageUrl = "/_layouts/images/Atkins.Intranet.Portal/icons/employeehandbook.png";



        public static readonly SPContentTypeId EmployeeHandBookContentTypeId = new SPContentTypeId("0x0100C4996397BE1448FB9360F07527C9F924");
    }
    public class EmployeeHandBookDocuments
    {
        public const string ListName = "Personalhandboksdokument";
        public const string ListDescription = "Contains documents related to different employee handbook items";
        public const string ListContentType = "Atkins Employee Handbook Document";
        public const string AtkinsContentTypeGroup = "Atkins Content Types";

        public const string EmployeeHandBook = "Employee HandBook";
        public const string EmployeeHandBookDisplayName = "Personalhandbok";
    }
    //***************************************************************************QSE QSE QSE**************************************************************************************
    //---------------------------CONTROLLING DOCUMENTS--------------------------
    public class ControllingDocuments
    {
        public const string ListName = "Kontrolldokument";
        public const string ListDescription = "Controlling Documents";
        public const string ListContentType = "Atkins QSE Controlling Document";
        public const string AtkinsContentTypeGroup = "Atkins Content Types";

        public const string ISO9001 = "ISO9001";
        public const string ISO9001DisplayName = "ISO9001";
        public const string ISO14001 = "ISO14001";
        public const string ISO14001DisplayName = "ISO14001";
        public const string ISO18001 = "ISO18001";
        public const string ISO18001DisplayName = "ISO18001";
        public const string Chapter = "ControlDocChapter";
        public const string ChapterDisplayname = "Kapitel";

        public const string WrittenBy = "Written By";
        public const string WrittenByDisplayName = "Skrivet av";
        public const string Auditor = "Auditor";
        public const string AuditorDisplayName = "Granskare";
        public const string Approver = "Approver";
        public const string ApproverDisplayName = "Godkännare";
        public const string ValidUntil = "Valid until";
        public const string ValidUntilDisplayName = "Giltig t.o.m";
        public const string DocumentID = "DocumentID";
        public const string DocumentIDDisplayName = "DocumentID";

        public const string TermGroup = "Atkins Sweden";
        public const string TermSetISO9001 = "ISO9001";
        public const string TermSetISO14001 = "ISO14001";
        public const string TermSetISO18001 = "ISO18001";
        public const string TermSetChapter = "Chapter";
        
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
        public const string ListName = "Resultat Dokument";
        public const string ListDescription = "Resulting Documents";
        public const string ListContentType = "Atkins QSE Resulting Document";
        public const string AtkinsContentTypeGroup = "Atkins Content Types";

        public const string ResultingDocumentCategory = "Resulting Document Category";
        public const string ResultingDocumentCategoryDisplayName = "Resultat dokument kategori";
        public const string ResultingDocumentYear = "Resulting Document Year";
        public const string ResultingDocumentYearDisplayName = "Resultat dokument år";
        public const int ResultingDocumentYearStart = 2008;
        public const int ResultingDocumentYearStop = 2030;

        public const string TermGroup = "Atkins Sweden";
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
        public const string ListName = "Process steg";
        public const string ListDescription = "The Process Step List is a SharePoint list that is designed to replace the current static html-tables that informs the user about the steps in each of the different defined Atkins process";
        public const string ListContentType = "Atkins QSE Prosess Step";
        public const string AtkinsContentTypeGroup = "Atkins Content Types";

        public const string TitleDisplayName = "Process steg";

        public const string Process = "Process";
        public const string ProcessDisplayName = "Process";
        public const string ProcessDescription = "Process Description";
        public const string ProcessDescriptionDisplayName = "Process Beskrivning";

        public const string ProcessTemplates = "ProcessTemplates";
        public const string ProcessTemplatesDisplayName = "Mallar";


        public const string TermGroup = "Atkins Sweden";
        public const string TermSetProcess = "Process";

        public const string webPartView = "webPartView";
                                                                                               
        public static readonly SPContentTypeId processStepContentTypeId = new SPContentTypeId("0x01004E6C0773E340486CBEB9D9856CEF6923");
    }

    //---------------------------DEVIATION LIST--------------------------
    public class DeviationsList
    {
        public const string ListName = "Avvikelser och förslag";
        public const string ListDescription = "Deviations and Suggestions";
        public const string ListContentTypeBase = "Atkins QSE Deviations Base";
        public const string ListContentTypeDeviations = "Atkins QSE Deviations";
        public const string ListContentTypeComplaints = "Atkins QSE Complaints";
        public const string ListContentTypeSuggestions = "Atkins QSE Suggestions";
        public const string AtkinsContentTypeGroup = "Atkins Content Types";

        public const string Title = "Title";
        public const string TitleDisplayName = "Titel";
        public const string KeyDate = "Key Date";
        public const string KeyDateDisplayName = "Datum";
        public const string DeviationStatus = "Deviation Status";
        public const string DeviationStatusDisplayName = "Avvikelsestatus";
        public const string DeviationDescription = "Deviation Description";
        public const string DeviationDescriptionDisplayName = "Avvikelsebeskrivning";
        public const string Responsible = "DeviationResponsible";
        public const string ResponsibleDisplayName = "Ansvarig";
        public const string DecisionDate = "Decision Date";
        public const string DecisionDateDisplayName = "Beslutsdatum";
        public const string DecisionComment = "Decision Comment";
        public const string DecisionCommentDisplayName = "Beslutskommentar";
        public const string ActionByDate = "Action By Date";
        public const string ActionByDateDisplayName = "Färdigställandedatum";
        public const string FollowUpDate = "Follow Up Date";
        public const string FollowUpDateDisplayName = "Uppföljningsdatum";
        public const string FollowUpComment = "Follow Up Comment";
        public const string FollowUpCommentDisplayName = "Uppföljningskommentar";

        public const string TermGroup = "Atkins Sweden";
        public const string TermSetStatus = "Deviation Status";
        //WEBPART
        public const string webPartView = "webPartView";
        public const string webPartTitle = "Avvikelser";
        public const string ZoneId = "Right";
        public const string webpartTitleImageUrl = "/_layouts/images/Atkins.Intranet.Portal/icons/deviation.png";


        public static readonly SPContentTypeId DeviationBaseContentTypeId = new SPContentTypeId("0x0100E597B736AF8F410F887223B41DF23E68");
        //DB201B0B455F46CBA862ABA9FE71071F
        //2D37B3E6024745619856983299CB97BD
        //D7241B3E34B44B05833E35CB02E99AE1
        public static readonly SPContentTypeId DeviationContentTypeId = new SPContentTypeId("0x0100E597B736AF8F410F887223B41DF23E6800DB201B0B455F46CBA862ABA9FE71071F");
        public static readonly SPContentTypeId ComplaintsContentTypeId = new SPContentTypeId("0x0100E597B736AF8F410F887223B41DF23E68002D37B3E6024745619856983299CB97BD");
        public static readonly SPContentTypeId SuggestionsContentTypeId = new SPContentTypeId("0x0100E597B736AF8F410F887223B41DF23E6800D7241B3E34B44B05833E35CB02E99AE1");

        public const string DeviationRoleDefinition = "Deviation Role Definition";
    }

    public class DeviationsSettingsList
    {
        public const string ListName = "QSEansvarig";
        public const string ListDescription = "This list contains the default user responsible for deviations";
        public const string ListContentType = "Atkins QSE Deviation Settings";
        public const string AtkinsContentTypeGroup = "Atkins Content Types";

        public const string TitleDisplayName = "Inställning";
        public const string Responsible = "DeviationSettingsResponsible";
        public const string ResponsibleDisplayName = "Responsible";

        public static readonly SPContentTypeId deviationSettingsContentTypeId = new SPContentTypeId("0x01007092430709A945E6BCD0C8E1B4D25C0D");
    }


    //---------------------------TEMPLATE DOCUMENT LIBRARY LIST--------------------------
    public class TemplateDocuments
    {
        public const string ListName = "Malldokument";
        public const string ListDescription = "Template Documents";
        public const string ListContentType = "Atkins Template Documents";
        public const string AtkinsContentTypeGroup = "Atkins Content Types";

        public const string TemplateDocumentCategory = "Template Document Category";
        public const string TemplateDocumentCategoryDisplayName = "Malldokument kategori";

        public const string TermGroup = "Atkins Sweden";
        public const string TermSetTemplateDocumentCategory = "Template Document Category";

        public static readonly SPContentTypeId templateDocumentContentTypeId = new SPContentTypeId("0x010100450A55B589644618865764033A029768");
                                                                                                            
    }
    public class TemplateDocumentAdministrators
    {
        public const string Name = "Template Document Administrators";
        public const string Description = "Contribute permission in the library.";
        public const SPRoleType role = SPRoleType.Contributor;
    }

    public class RelevantDocuments
    {
        public const string webPartTitle = "Relevanta dokument";
        public const string ZoneId = "Left";
        public const string webpartTitleImageUrl = "/_layouts/images/Atkins.Intranet.Portal/icons/docs.png";
    }
    public class LastAddedModiefiedDocuments
    {
        public const string webPartTitle = "Senast tillagda eller ändrade Dokument";
        public const string webpartItemStyle = "Announcements";
        public const string xslPath = "/Sites/Intranet/Style Library/XSL Style Sheets/customItem.xsl";
        public const string ZoneId = "Left";
        public const string webpartTitleImageUrl = "/_layouts/images/Atkins.Intranet.Portal/icons/docs.png";
    }
    public class QSELinks
    {
        public const string ListName = "$Resources:core,linksList;";
        public const string webPartTitle = "Länkar";
        public const string ZoneId = "Right";
        public const string webPartView = "webPartView";
        public const string webPartViewFields = "URL";
        public const int rowlimit = 100;
        public const string query = "";
        
        public const string webpartTitleImageUrl = "/_layouts/images/Atkins.Intranet.Portal/icons/link.png";
    }


    //***************************************************************************NYHETER BLOG**************************************************************************************

    public class BlogPosts
    {
        public const string webName = "Nyheter";
        public const string webPartTitle = "Nyheter";
        public const string webPartViewFields = "Title;Body;PublishedDate,DateTime;";
        public const string webPartView = "webPartView";
        public const string webpartItemStyle =  "CQWP_Blog";
        public const int rowLimitStartPage = 10;
        public const string ListName = "$Resources:core,BlogPost;";
        public const string xslPath = "/Sites/Intranet/Style Library/XSL Style Sheets/customItem.xsl";
        public const string ZoneId = "Left";
        public const string webpartTitleImageUrl = "/_layouts/images/Atkins.Intranet.Portal/icons/news.png";
        public const string blogpostCategories = "HR,QSE,Finans,Marknad,Kontor,IT,Teknik,Rekrytering";
        public const string categoryFilterHR = "HR";
        public const string categoryFilterQSE = "QSE";
        public const string categoryFilterFinance = "Finans";

        public const string categoryFilterMarketing = "Marknad";
        public const string categoryFilterOffices = "Kontor";
        public const string categoryFilterIT = "IT";

        public const string categoryFilterTech = "Teknik";
        public const string categoryFilterRecruit = "Rekrytering";
        
        
    }
   
    public class HideTitleBlog
    {
        public const string ZoneId = "Right";
        public const string webPartTitle = "Hide Title";
        public const string Content = "<style>.ms-webpartpagedescription {DISPLAY: none}</style>";
    }

   
    //******************************************************************FINANCE*******************************************************************************************

    public class FinanceCalendar
    {
        public const string ListName = "Kalender";
        
        public const string webPartTitle = "Kalender";
        public const string webPartView = "webPartView";
        public const string webPartViewFields = "LinkTitle,EventDate,EndDate";
        public const string webPartQuery = "<Where>" +
                                              "<Geq><FieldRef Name='EndDate' /><Value Type='DateTime'><Today /></Value></Geq>" +
                                           "</Where>" +
                                           "<OrderBy><FieldRef Name='EventDate' Ascending='TRUE'/></OrderBy>";
        public const int webPartRowLimit = 10;
        public const string ZoneId = "Right";
        public const string webpartTitleImageUrl = "/_layouts/images/Atkins.Intranet.Portal/icons/calendar.png";
    }
    public class FinanceLinks
    {
        public const string webName = "Finance";
        public const string ListName = "Länkar";
        public const string ListDescription = "Länkar";

        public const string TitleDisplayName = "Länk";

        public const string webPartTitle = "Nyttiga länkar";
        public const string webPartView = "financeWebPartView";
        public const string webPartQuery = "<Where>" +
                                             "<Eq><FieldRef Name='linkArea'/><Value Type='Lookup'>Finans</Value></Eq>" +
                                           "</Where>";

        public const string webPartViewFields = "URL";
        public const int webPartRowLimit = 10;
        public const string ZoneId = "Right";
        public const string webpartTitleImageUrl = "/_layouts/images/Atkins.Intranet.Portal/icons/link.png";
    }
    //******************************************************************COMMON COMMON COMMON***************************************************************************************
    public class CommonSettings
    {
        public const string resourceFile = "core";
        public const uint resourceLCID = 1053;
    }
}
