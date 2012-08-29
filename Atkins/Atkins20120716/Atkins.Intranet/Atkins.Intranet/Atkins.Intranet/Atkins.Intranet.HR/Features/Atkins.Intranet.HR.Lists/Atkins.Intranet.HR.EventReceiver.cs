using System;
using System.Runtime.InteropServices;
using Atkins.Intranet.Utilities.HelperUtils;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using System.Linq;
using Microsoft.Office.DocumentManagement.MetadataNavigation;
using System.Threading;
using System.Globalization;



namespace Atkins.Intranet.HR.Features.Lists
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("6c41d377-e77b-49cb-bbba-7e521abefef1")]
    public class AtkinsIntranetHREventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPWeb currentWeb = (SPWeb)properties.Feature.Parent;
            try
            {

                //Introduction Templates list
                SPList templateList = CustomListHelper.ReturnList(currentWeb, IntroductionTemplateFields.ListName);
                if (templateList == null)
                {
                   CreateIntroductionContentTypeList(currentWeb);
                }

                //Employee Contact List
                SPList contactList = CustomListHelper.ReturnList(currentWeb, EmployeeContactFields.ListName);
                if (contactList == null)
                {
                    CreateEmployeeContactContentTypeList(currentWeb);
                }
                
                //Employee - Introduction Tasks list
                SPList taskList = CustomListHelper.ReturnList(currentWeb, IntroductionTasksFields.ListName);
                if (taskList == null)
                {
                    CreateTaskContentTypeList(currentWeb);
                }

                //Introduction Document Library
                SPList documentList = CustomListHelper.ReturnList(currentWeb, EmployeeDocuments.ListName);
                if (documentList == null)
                {
                    CreateDocumentContentTypeList(currentWeb);
                }

                //Employee Handbook List 
                SPList employeeList = CustomListHelper.ReturnList(currentWeb, EmployeeHandbook.ListName);
                if (employeeList == null)
                {
                    CreateEmployeeHandbookContentTypeList(currentWeb);
                }
                //Employee Handbook Documents 
                SPList EmployeeHandBookDocumentsList = CustomListHelper.ReturnList(currentWeb, EmployeeHandBookDocuments.ListName);
                if (employeeList == null)
                {
                    CreateEmployeeHandbookDocumentsContentTypeList(currentWeb);
                }
            }
            catch(SPException exception)
            {
                //Log error message
                throw exception;
            }
        }

        private static void CreateEmployeeHandbookDocumentsContentTypeList(SPWeb currentWeb)
        {

            Guid documentLibraryGuid = currentWeb.Lists.Add(CustomListHelper.ReturnTrimmedString(EmployeeHandBookDocuments.ListName), EmployeeHandBookDocuments.ListDescription, SPListTemplateType.DocumentLibrary);
            SPList employeeHandBookDocuments = currentWeb.Lists[documentLibraryGuid];
            employeeHandBookDocuments.Title = EmployeeHandBookDocuments.ListName;
            employeeHandBookDocuments.OnQuickLaunch = true;
            employeeHandBookDocuments.Update();

            using (SPSite site = new SPSite(currentWeb.Site.ID))
            {
                SPWeb rootWeb = site.RootWeb;

                //Employee handbook ID look up 

                SPList employeeHandbookList = CustomListHelper.ReturnList(currentWeb, EmployeeHandbook.ListName);
                if (employeeHandbookList != null)
                {
                    string internalName = employeeHandBookDocuments.Fields.AddLookup(CustomListHelper.ReturnTrimmedString(EmployeeHandBookDocuments.EmployeeHandBook), employeeHandbookList.ID, currentWeb.ID, true);
                    SPFieldLookup employeeLookUp = (SPFieldLookup)employeeHandBookDocuments.Fields[internalName];
                    employeeLookUp.LookupField = employeeHandbookList.Fields[SPBuiltInFieldId.Title].InternalName;
                    employeeLookUp.Title = EmployeeHandBookDocuments.EmployeeHandBookDisplayName;
                    employeeLookUp.Update();
                }

                employeeHandBookDocuments.Update();

                SPView defaultView = employeeHandBookDocuments.DefaultView;
                defaultView.ViewFields.Add(CustomListHelper.ReturnTrimmedString(EmployeeHandBookDocuments.EmployeeHandBook));
                defaultView.Update();
            }

        }


        private static void CreateEmployeeHandbookContentTypeList(SPWeb currentWeb)
        {
            Guid employeeHandbookListGuid = currentWeb.Lists.Add(CustomListHelper.ReturnTrimmedString(EmployeeHandbook.ListName), EmployeeHandbook.ListDescription, SPListTemplateType.GenericList);
            SPList employeeHandbookList = currentWeb.Lists[employeeHandbookListGuid];
            employeeHandbookList.Title = EmployeeHandbook.ListName;
            employeeHandbookList.OnQuickLaunch = true;
            employeeHandbookList.EnableVersioning = true;
            employeeHandbookList.Update();

            using (SPSite site = new SPSite(currentWeb.Site.ID))
            {
                SPWeb rootWeb = site.RootWeb;
                SPContentType employeeHandbookContentType = null;
                if (CustomListHelper.SiteContainsContentType(rootWeb, EmployeeHandbook.EmployeeHandBookContentTypeId))
                {
                    employeeHandbookContentType = rootWeb.ContentTypes[EmployeeHandbook.EmployeeHandBookContentTypeId];
                }
                else
                {
                    //Description Field
                    string fieldInternalName = CustomListHelper.CreateSiteColumn(rootWeb, EmployeeHandbook.Description, SPFieldType.Note, true);
                    SPFieldMultiLineText descriptionField = (SPFieldMultiLineText)rootWeb.Fields.GetField(fieldInternalName);
                    descriptionField.Title = EmployeeHandbook.DescriptionDisplayName;
                    descriptionField.Group = EmployeeHandbook.ListName;
                    descriptionField.NumberOfLines = 15;
                    descriptionField.RichText = true;
                    descriptionField.RichTextMode = SPRichTextMode.FullHtml;
                    descriptionField.Update();
                    SPFieldLink descriptionLink = new SPFieldLink(descriptionField);

                    //Creates the TermSet and or group
                    TaxonomyUtility.CreateTermSet(currentWeb,EmployeeHandbook.TermGroup,EmployeeHandbook.TermSet);
                    //Category TaxonomyField

                    fieldInternalName = CustomListHelper.CreateTaxonomySiteColumn(site, EmployeeHandbook.Category);
                    TaxonomyField categoryField = rootWeb.Fields[fieldInternalName] as TaxonomyField;
                    TaxonomySession session = new TaxonomySession(site);
                    var termStore = session.TermStores[TermStoreName.TermStore];
                    var group = from g in termStore.Groups where g.Name == EmployeeHandbook.TermGroup select g;
                    var termSet = group.FirstOrDefault().TermSets[EmployeeHandbook.TermSet];
                    categoryField.SspId = termSet.TermStore.Id;
                    categoryField.TermSetId = termSet.Id;
                    categoryField.TargetTemplate = string.Empty;
                    categoryField.AllowMultipleValues = false;
                    categoryField.CreateValuesInEditForm = false;
                    categoryField.Open = true;
                    categoryField.AnchorId = Guid.Empty;
                    categoryField.Group = EmployeeHandbook.ListName;
                    categoryField.Title = EmployeeHandbook.CategoryDisplayName;
                    categoryField.Update();
                    SPFieldLink categoryFieldLink = new SPFieldLink(categoryField);
                    
                    //Valid from Field
                    fieldInternalName = CustomListHelper.CreateSiteColumn(rootWeb, EmployeeHandbook.ValidFrom, SPFieldType.DateTime,true);
                    SPFieldDateTime validFromField = (SPFieldDateTime)rootWeb.Fields.GetField(fieldInternalName);
                    validFromField.Title = EmployeeHandbook.ValidFromDisplayName;
                    validFromField.Group = EmployeeHandbook.ListName;
                    validFromField.DisplayFormat = SPDateTimeFieldFormatType.DateOnly;
                    validFromField.Update();
                    SPFieldLink validFromFieldLink = new SPFieldLink(validFromField);

                    //Valid to Field
                    fieldInternalName = CustomListHelper.CreateSiteColumn(rootWeb, EmployeeHandbook.ValidTo, SPFieldType.DateTime, true);
                    SPFieldDateTime validToField = (SPFieldDateTime)rootWeb.Fields.GetField(fieldInternalName);
                    validToField.Title = EmployeeHandbook.ValidToDisplayName;
                    validToField.Group = EmployeeHandbook.ListName;
                    validToField.DisplayFormat = SPDateTimeFieldFormatType.DateOnly;
                    validToField.Update();
                    SPFieldLink validToFieldLink = new SPFieldLink(validToField);


                    employeeHandbookContentType = new SPContentType(EmployeeHandbook.EmployeeHandBookContentTypeId,
                                                                rootWeb.ContentTypes,
                                                                EmployeeHandbook.ListContentType);

                    employeeHandbookContentType.FieldLinks.Add(descriptionLink);
                    employeeHandbookContentType.FieldLinks.Add(categoryFieldLink);
                    employeeHandbookContentType.FieldLinks.Add(validFromFieldLink);
                    employeeHandbookContentType.FieldLinks.Add(validToFieldLink);
                    employeeHandbookContentType.Group = EmployeeHandbook.AtkinsContentTypeGroup;
                    rootWeb.ContentTypes.Add(employeeHandbookContentType);
                    rootWeb.Update();
                }
                if (employeeHandbookContentType != null &&
                !CustomListHelper.ListContainsContentType(employeeHandbookList,
                                                            EmployeeHandbook.EmployeeHandBookContentTypeId))
                {
                    employeeHandbookList.ContentTypesEnabled = true;
                    employeeHandbookList.ContentTypes.Add(employeeHandbookContentType);
                    employeeHandbookList.ContentTypes[0].Delete();
                    employeeHandbookList.Update();

                    SPView defaultView = employeeHandbookList.DefaultView;
                    SPField description = CustomListHelper.ReturnListField(employeeHandbookList, EmployeeHandbook.Description);
                    if (description != null)
                        defaultView.ViewFields.Add(description);

                    SPField category = CustomListHelper.ReturnListField(employeeHandbookList, EmployeeHandbook.Category);
                    if (category != null)
                        defaultView.ViewFields.Add(category);


                    SPField validFrom = CustomListHelper.ReturnListField(employeeHandbookList, EmployeeHandbook.ValidFrom);
                    if (validFrom != null)
                        defaultView.ViewFields.Add(validFrom);

                    SPField validTo = CustomListHelper.ReturnListField(employeeHandbookList, EmployeeHandbook.ValidTo);
                    if (validTo != null)
                        defaultView.ViewFields.Add(validTo);

                    defaultView.Update();
                    currentWeb.Update();

                    //ADD METADATA NAVIGATION TO LIST
                    MetadataNavigationSettings listNavSettings = MetadataNavigationSettings.GetMetadataNavigationSettings(employeeHandbookList);
                    MetadataNavigationHierarchy mdnNavHierarchy1 = new MetadataNavigationHierarchy(category);
                    listNavSettings.AddConfiguredHierarchy(mdnNavHierarchy1);

                    MetadataNavigationSettings.SetMetadataNavigationSettings(employeeHandbookList, listNavSettings, true);  

                }
            }
        }

        private static void CreateIntroductionContentTypeList(SPWeb currentWeb)
        {
            Guid templateListGuid = currentWeb.Lists.Add(CustomListHelper.ReturnTrimmedString(IntroductionTemplateFields.ListName), IntroductionTemplateFields.ListDescription, SPListTemplateType.GenericList);
            SPList templateList = currentWeb.Lists[templateListGuid];
            templateList.Title = IntroductionTemplateFields.ListName;
            templateList.OnQuickLaunch = true;
            templateList.Update();
            
            using (SPSite site = new SPSite(currentWeb.Site.ID))
            {
                SPWeb rootWeb = site.RootWeb;
               
                //Create Introduction Template Content Type
                SPContentType templateListContentType = null;
                if (CustomListHelper.SiteContainsContentType(rootWeb, IntroductionTemplateFields.TemplateStepsContentTypeId))
                {
                    templateListContentType = rootWeb.ContentTypes[IntroductionTemplateFields.TemplateStepsContentTypeId];
                }
                else
                {
                    //Steps Field
                    string fieldInternalName = CustomListHelper.CreateSiteColumn(rootWeb, IntroductionTemplateFields.TemplateSteps, SPFieldType.Note, true);
                    SPFieldMultiLineText stepsField = (SPFieldMultiLineText)rootWeb.Fields.GetField(fieldInternalName);
                    stepsField.Title = IntroductionTemplateFields.TemplateStepsDisplayName;
                    stepsField.Group = IntroductionTemplateFields.ListName;
                    stepsField.Update();
                    SPFieldLink stepsLink = new SPFieldLink(stepsField);

                    //TemplateIsActive Field
                    fieldInternalName = CustomListHelper.CreateSiteColumn(rootWeb, IntroductionTemplateFields.TemplateIsActive, SPFieldType.Boolean, true);
                    SPFieldBoolean activeField = (SPFieldBoolean)rootWeb.Fields.GetField(fieldInternalName);
                    activeField.Title = IntroductionTemplateFields.TemplateIsActiveDisplayName;
                    activeField.Group = IntroductionTemplateFields.ListName;
                    activeField.Update();
                    SPFieldLink activeLink = new SPFieldLink(activeField);

                    templateListContentType = new SPContentType(IntroductionTemplateFields.TemplateStepsContentTypeId,
                                                                rootWeb.ContentTypes,
                                                                IntroductionTemplateFields.ListContentType);

                    templateListContentType.FieldLinks.Add(stepsLink);
                    templateListContentType.FieldLinks.Add(activeLink);
                    templateListContentType.Group = IntroductionTemplateFields.AtkinsContentTypeGroup;

                    rootWeb.ContentTypes.Add(templateListContentType);
                    rootWeb.Update();
                }

                //Add Introduction Template Content type to Template list
                if (templateListContentType != null &&
                    !CustomListHelper.ListContainsContentType(templateList,
                                                              IntroductionTemplateFields.TemplateStepsContentTypeId))
                {
                    templateList.ContentTypesEnabled = true;
                    templateList.ContentTypes.Add(templateListContentType);
                    templateList.ContentTypes[0].Delete();
                    templateList.Update();

                    SPView defaultView = templateList.DefaultView;
                    SPField templateSteps = CustomListHelper.ReturnListField(templateList, IntroductionTemplateFields.TemplateSteps);
                    if(templateSteps != null)
                    defaultView.ViewFields.Add(templateSteps);

                    SPField activeTemplate = CustomListHelper.ReturnListField(templateList, IntroductionTemplateFields.TemplateIsActive);
                    if(activeTemplate != null)
                    defaultView.ViewFields.Add(activeTemplate);
                    defaultView.Update();

                    currentWeb.Update();
                }
            }
        }

        private static void CreateEmployeeContactContentTypeList(SPWeb currentWeb)
        {
            SPList employeeList;
            Guid contactListGuid = currentWeb.Lists.Add(CustomListHelper.ReturnTrimmedString(EmployeeContactFields.ListName), EmployeeContactFields.ListDescription, SPListTemplateType.GenericList); //SPListTemplateType.Contacts);
            employeeList = currentWeb.Lists[contactListGuid];
            employeeList.Title = EmployeeContactFields.ListName;
            employeeList.OnQuickLaunch = true;
            employeeList.NavigateForFormsPages = true;
            employeeList.Update();
            
            using (SPSite site = new SPSite(currentWeb.Site.ID))
            {
                SPWeb rootWeb = site.RootWeb;
               
                //Create Employee Contact Content Type
                SPContentType employeeListContentType = null;
                if (CustomListHelper.SiteContainsContentType(rootWeb, EmployeeContactFields.EmployeeContentTypeId))
                {
                    employeeListContentType = rootWeb.ContentTypes[EmployeeContactFields.EmployeeContentTypeId];
                }
                else
                {
                    //Personal Code Field
                    string fieldInternalName = CustomListHelper.CreateSiteColumn(rootWeb, EmployeeContactFields.PersonalNumber, SPFieldType.Text, true);
                    SPFieldText personalNumberField = (SPFieldText)rootWeb.Fields.GetField(fieldInternalName);
                    personalNumberField.Title = EmployeeContactFields.PersonalNumberDisplayName;
                    personalNumberField.Group = EmployeeContactFields.ListName;
                    personalNumberField.Update();
                    SPFieldLink codeLink = new SPFieldLink(personalNumberField);

                    //Position Field
                    fieldInternalName = CustomListHelper.CreateSiteColumn(rootWeb, EmployeeContactFields.Position, SPFieldType.Text, true);
                    SPFieldText positionField = (SPFieldText)rootWeb.Fields.GetField(fieldInternalName);
                    positionField.Title = EmployeeContactFields.PositionDisplayName;
                    positionField.Group = EmployeeContactFields.ListName;
                    positionField.Update();
                    SPFieldLink positionLink = new SPFieldLink(positionField);

                    //Manager Field
                    fieldInternalName = CustomListHelper.CreateSiteColumn(rootWeb, EmployeeContactFields.Manager, SPFieldType.User, true);
                    SPFieldUser managerField = (SPFieldUser)rootWeb.Fields.GetField(fieldInternalName);
                    managerField.Title = EmployeeContactFields.ManagerDisplayName;
                    managerField.Group = EmployeeContactFields.ListName;
                    managerField.Update();
                    SPFieldLink managerLink = new SPFieldLink(managerField);

                    //HR Responsible Field
                    fieldInternalName = CustomListHelper.CreateSiteColumn(rootWeb, EmployeeContactFields.HR_Responsible, SPFieldType.User, true);
                    SPFieldUser hrResponsibleField = (SPFieldUser)rootWeb.Fields.GetField(fieldInternalName);
                    hrResponsibleField.Title = EmployeeContactFields.HR_ResponsibleDisplayName;
                    hrResponsibleField.Group = EmployeeContactFields.ListName;
                    hrResponsibleField.Update();
                    SPFieldLink hrResponsibleLink = new SPFieldLink(hrResponsibleField);

                    //Mentor Field
                    fieldInternalName = CustomListHelper.CreateSiteColumn(rootWeb, EmployeeContactFields.Mentor, SPFieldType.User, false);
                    SPFieldUser mentorField = (SPFieldUser)rootWeb.Fields.GetField(fieldInternalName);
                    mentorField.Title = EmployeeContactFields.MentorDisplayName;
                    mentorField.Group = EmployeeContactFields.ListName;
                    mentorField.Update();
                    SPFieldLink mentorLink = new SPFieldLink(mentorField);


                    employeeListContentType = new SPContentType(EmployeeContactFields.EmployeeContentTypeId,
                                                                rootWeb.ContentTypes,
                                                                EmployeeContactFields.ListContentType);

                    employeeListContentType.FieldLinks.Add(codeLink);
                    employeeListContentType.FieldLinks.Add(positionLink);
                    employeeListContentType.FieldLinks.Add(mentorLink);
                    employeeListContentType.FieldLinks.Add(managerLink);
                    employeeListContentType.FieldLinks.Add(hrResponsibleLink);
                    
                    employeeListContentType.Group = EmployeeContactFields.AtkinsContentTypeGroup;
                    employeeListContentType.DisplayFormUrl = currentWeb.ServerRelativeUrl + EmployeeContactFields.CustomDisplayFormUrl;
                    
                    rootWeb.ContentTypes.Add(employeeListContentType);
                    rootWeb.Update();
                }

                //Add Employee Contact Content type to Office Information list
                if (employeeListContentType != null &&
                    !CustomListHelper.ListContainsContentType(employeeList,
                                                              EmployeeContactFields.EmployeeContentTypeId))
                {
                    employeeList.ContentTypesEnabled = true;
                    employeeList.ContentTypes.Add(employeeListContentType);
                    employeeList.ContentTypes[0].Delete();
                    employeeList.Update();

                    currentWeb.Update();
                }

                //Add look ups directly to the list instance instead of the content type (ease of maintenance in the long run)
                //Office Lookup field
                SPList officeList = CustomListHelper.ReturnList(rootWeb, OfficeFields.ListName);
                if (officeList != null)
                {
                    string internalName= employeeList.Fields.AddLookup(EmployeeContactFields.Office, officeList.ID, rootWeb.ID, true);
                    SPFieldLookup officeField = (SPFieldLookup)employeeList.Fields[internalName];
                    officeField.LookupField = officeList.Fields[SPBuiltInFieldId.Title].InternalName;
                    officeField.Update();
                }

                //Template LookUp Field
                SPList templateList = CustomListHelper.ReturnList(currentWeb, IntroductionTemplateFields.ListName);
                if (templateList != null)
                {
                    string fieldInternalName = employeeList.Fields.AddLookup(CustomListHelper.ReturnTrimmedString(EmployeeContactFields.IntroductionTemplate), templateList.ID, true);
                    SPFieldLookup templateField = (SPFieldLookup) employeeList.Fields[fieldInternalName];
                    templateField.LookupField = templateList.Fields[SPBuiltInFieldId.Title].InternalName;
                    templateField.Title = EmployeeContactFields.IntroductionTemplate;
                    templateField.Update();
                }

                SPView defaultView = employeeList.DefaultView;
                defaultView.ViewFields.Add(CustomListHelper.ReturnTrimmedString(EmployeeContactFields.PersonalNumber));
                defaultView.ViewFields.Add(CustomListHelper.ReturnTrimmedString(EmployeeContactFields.Position));
                defaultView.ViewFields.Add(CustomListHelper.ReturnTrimmedString(EmployeeContactFields.Manager));
                defaultView.ViewFields.Add(CustomListHelper.ReturnTrimmedString(EmployeeContactFields.Mentor));
                defaultView.ViewFields.Add(CustomListHelper.ReturnTrimmedString(EmployeeContactFields.HR_Responsible));
                defaultView.ViewFields.Add(CustomListHelper.ReturnTrimmedString(EmployeeContactFields.Office));
                defaultView.ViewFields.Add(CustomListHelper.ReturnTrimmedString(EmployeeContactFields.IntroductionTemplate));
                defaultView.Update();

                //WebPartView
                System.Collections.Specialized.StringCollection viewFields = new System.Collections.Specialized.StringCollection();
                viewFields.Add("LinkTitle");
                SPView webPartView = employeeList.Views.Add(EmployeeContactFields.webPartView, viewFields, "", 5, false, false);
                webPartView.TabularView = false;
                webPartView.Update();
            }
        }

        private static void CreateTaskContentTypeList(SPWeb currentWeb)
        {
            Guid taskListGuid = currentWeb.Lists.Add(CustomListHelper.ReturnTrimmedString(IntroductionTasksFields.ListName), IntroductionTasksFields.ListDescription, SPListTemplateType.GenericList); 
            SPList taskList = currentWeb.Lists[taskListGuid];
            taskList.Title = IntroductionTasksFields.ListName;
            taskList.OnQuickLaunch = true;
            taskList.Update();

            using (SPSite site = new SPSite(currentWeb.Site.ID))
            {
                SPWeb rootWeb = site.RootWeb;

                //Create Introduction Task Content Type
                SPContentType taskListContentType = null;
                if (CustomListHelper.SiteContainsContentType(rootWeb, IntroductionTasksFields.TaskContentTypeId))
                {
                    taskListContentType = rootWeb.ContentTypes[IntroductionTasksFields.TaskContentTypeId];
                }
                else
                {
                    //AssignedTo Field
                    string fieldInternalName = CustomListHelper.CreateSiteColumn(rootWeb, IntroductionTasksFields.TaskAssignee, SPFieldType.User, false);
                    SPFieldUser assigneeField = (SPFieldUser)rootWeb.Fields.GetField(fieldInternalName);
                    assigneeField.Title = IntroductionTasksFields.TaskAssigneeDisplayName;
                    assigneeField.Group = IntroductionTasksFields.ListName;
                    assigneeField.Update();
                    SPFieldLink assigneeLink = new SPFieldLink(assigneeField);

                    //Due Date Field
                    fieldInternalName = CustomListHelper.CreateSiteColumn(rootWeb, CustomListHelper.ReturnTrimmedString(IntroductionTasksFields.DueDate), SPFieldType.DateTime, false);
                    SPFieldDateTime dueDateField = (SPFieldDateTime)rootWeb.Fields.GetField(fieldInternalName);
                    dueDateField.Title = IntroductionTasksFields.DueDateDisplayName;
                    dueDateField.Group = IntroductionTasksFields.ListName;
                    dueDateField.DisplayFormat = SPDateTimeFieldFormatType.DateOnly;
                    dueDateField.Update();
                    SPFieldLink dueDateLink = new SPFieldLink(dueDateField);

                    //Completed Status Field
                    fieldInternalName = CustomListHelper.CreateSiteColumn(rootWeb, IntroductionTasksFields.Completed, SPFieldType.Boolean, false);
                    SPFieldBoolean completeStatusField = (SPFieldBoolean)rootWeb.Fields.GetField(fieldInternalName);
                    completeStatusField.Title = IntroductionTasksFields.CompletedDisplayName;
                    completeStatusField.Group = IntroductionTasksFields.ListName;
                    completeStatusField.Update();
                    SPFieldLink completeStatusLink = new SPFieldLink(completeStatusField);

                    //Completion Date Field
                    fieldInternalName = CustomListHelper.CreateSiteColumn(rootWeb, CustomListHelper.ReturnTrimmedString(IntroductionTasksFields.CompletionDate), SPFieldType.DateTime, false);
                    SPFieldDateTime completionDateField = (SPFieldDateTime)rootWeb.Fields.GetField(fieldInternalName);
                    completionDateField.Title = IntroductionTasksFields.CompletionDateDisplayName;
                    completionDateField.Group = IntroductionTasksFields.ListName;
                    completionDateField.DisplayFormat = SPDateTimeFieldFormatType.DateOnly;
                    completionDateField.Update();
                    SPFieldLink completionDateLink = new SPFieldLink(completionDateField);

                    taskListContentType = new SPContentType(IntroductionTasksFields.TaskContentTypeId, rootWeb.ContentTypes, IntroductionTasksFields.ListContentType);

                    taskListContentType.FieldLinks.Add(assigneeLink);
                    taskListContentType.FieldLinks.Add(dueDateLink);
                    taskListContentType.FieldLinks.Add(completeStatusLink);
                    taskListContentType.FieldLinks.Add(completionDateLink);
                    taskListContentType.Group = IntroductionTasksFields.AtkinsContentTypeGroup;

                    rootWeb.ContentTypes.Add(taskListContentType);
                    rootWeb.Update();
                }

                //Add Introduction Task Content type to task list
                if (taskListContentType != null && !CustomListHelper.ListContainsContentType(taskList, IntroductionTasksFields.TaskContentTypeId))
                {
                    taskList.ContentTypesEnabled = true;
                    taskList.ContentTypes.Add(taskListContentType);
                    taskList.ContentTypes[0].Delete();
                    taskList.Update();

                    currentWeb.Update();
                }

                //Employee ID look up 
                SPList contactList = CustomListHelper.ReturnList(currentWeb, EmployeeContactFields.ListName);
                if (contactList != null)
                {
                    string internalName = taskList.Fields.AddLookup(CustomListHelper.ReturnTrimmedString(IntroductionTasksFields.Employee), contactList.ID, currentWeb.ID, true);
                    SPFieldLookup employeeLookUp = (SPFieldLookup)taskList.Fields[internalName];
                    employeeLookUp.LookupField = contactList.Fields[SPBuiltInFieldId.Title].InternalName;
                    employeeLookUp.Title = IntroductionTasksFields.Employee;
                    employeeLookUp.Update();
                }
                
                taskList.Update();

                SPView defaultView = taskList.DefaultView;
                defaultView.ViewFields.Add(IntroductionTasksFields.Completed);
                defaultView.ViewFields.Add(CustomListHelper.ReturnTrimmedString(IntroductionTasksFields.CompletionDate));
                defaultView.ViewFields.Add(CustomListHelper.ReturnTrimmedString(IntroductionTasksFields.Employee));
                defaultView.ViewFields.Add(CustomListHelper.ReturnTrimmedString(IntroductionTasksFields.TaskAssignee));
                defaultView.ViewFields.Add(IntroductionTasksFields.DueDate);
                defaultView.Update();

                //WebPartView show items assigned to [ME] and are not completed
                System.Collections.Specialized.StringCollection viewFields = new System.Collections.Specialized.StringCollection();
                viewFields.Add("LinkTitle");
                string query = "<Where>"+
                                    "<And>"+
                                        "<Eq><FieldRef Name='"+IntroductionTasksFields.TaskAssignee+"'/><Value Type='Integer'><UserID Type='Integer'/></Value></Eq>"+
                                        "<Eq><FieldRef Name='" + IntroductionTasksFields.Completed + "'/><Value Type='Integer'>0</Value></Eq>" +
                                    "</And>" +
                                "</Where>";
                SPView webPartView = taskList.Views.Add(IntroductionTasksFields.webPartView, viewFields, query, 5, false, false);
                webPartView.TabularView = false;
                webPartView.Update();

            }

        }

        private static void CreateDocumentContentTypeList(SPWeb currentWeb)
        {
            Guid documentLibraryGuid = currentWeb.Lists.Add(CustomListHelper.ReturnTrimmedString(EmployeeDocuments.ListName), EmployeeDocuments.ListDescription, SPListTemplateType.DocumentLibrary);
            SPList employeeDocuments = currentWeb.Lists[documentLibraryGuid];
            employeeDocuments.Title = EmployeeDocuments.ListName;
            employeeDocuments.OnQuickLaunch = true;
            employeeDocuments.Update();

            using (SPSite site = new SPSite(currentWeb.Site.ID))
            {
                SPWeb rootWeb = site.RootWeb;
                
                //Employee ID look up 
                SPList contactList = CustomListHelper.ReturnList(currentWeb, EmployeeContactFields.ListName);
                if (contactList != null)
                {
                    string internalName = employeeDocuments.Fields.AddLookup(CustomListHelper.ReturnTrimmedString(EmployeeDocuments.EmployeeName), contactList.ID, currentWeb.ID, true);
                    SPFieldLookup employeeLookUp = (SPFieldLookup)employeeDocuments.Fields[internalName];
                    employeeLookUp.LookupField = contactList.Fields[SPBuiltInFieldId.Title].InternalName;
                    employeeLookUp.Title = EmployeeDocuments.EmployeeNameDisplayName;
                    employeeLookUp.Group = EmployeeDocuments.ListName;
                    employeeLookUp.Update();
                }

                employeeDocuments.Update();

                SPView defaultView = employeeDocuments.DefaultView;
                defaultView.ViewFields.Add(CustomListHelper.ReturnTrimmedString(EmployeeDocuments.EmployeeName));
                defaultView.Update();
            }

        }

        
    }
}
