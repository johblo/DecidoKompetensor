using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Atkins.Intranet.Utilities.HelperUtils;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.Security;
using System.Linq;
using Microsoft.Office.DocumentManagement.MetadataNavigation;
using System.Globalization;
using System.Threading;

namespace Atkins.Intranet.QSE.Features.Atkins.Intranet.QSE.Lists
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("e858e7bc-37e9-47b9-aa27-254f57b25ed4")]
    public class AtkinsIntranetQSEEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            try
            {
                SPWeb currentWeb = (SPWeb)properties.Feature.Parent;
                //Controlling Document
                SPList controllingList = CustomListHelper.ReturnList(currentWeb, ControllingDocuments.ListName);
                if (controllingList == null)
                {
                    CreateControllingDocumentContentTypeList(currentWeb);
                }
                //Resulting Document
                SPList resultingList = CustomListHelper.ReturnList(currentWeb, ResultingDocuments.ListName);
                if (resultingList == null)
                {
                    CreateResultingDocumentContentTypeList(currentWeb);
                }
                //Process Step
                SPList processStepList = CustomListHelper.ReturnList(currentWeb, ProcessStepList.ListName);
                if (processStepList == null)
                {
                    CreateProcessStepContentTypeList(currentWeb);
                }
                //Deviations Step
                SPList deviationList = CustomListHelper.ReturnList(currentWeb, DeviationsList.ListName);
                if (deviationList == null)
                {
                    CreateDeviationsContentTypeList(currentWeb);
                }
                //Create view in LinkList
                string listName = SPUtility.GetLocalizedString(QSELinks.ListName, QSELinks.resourceFile, QSELinks.resourceLCID);
                SPList linkList = CustomListHelper.ReturnList(currentWeb, listName);
                if (!CustomListHelper.checkIfViewExist(linkList, QSELinks.webPartView))
                {
                    CustomListHelper.CreateView(linkList, QSELinks.webPartView, CustomListHelper.returnStringArray(QSELinks.webPartViewFields), QSELinks.query,QSELinks.rowlimit);
                }
            }
            catch (SPException exception)
            {
                //Log error message
                throw exception;
            }
           
        }
        private static void CreateDeviationsContentTypeList(SPWeb currentWeb)
        {

            Guid listGuid = currentWeb.Lists.Add(CustomListHelper.ReturnTrimmedString(DeviationsList.ListName), DeviationsList.ListDescription, SPListTemplateType.GenericList);
            SPList deviationList = currentWeb.Lists[listGuid];
            deviationList.Title = DeviationsList.ListName;
            deviationList.BreakRoleInheritance(false);
            deviationList.WriteSecurity = 2;
            deviationList.EnableVersioning = true;
            deviationList.OnQuickLaunch = true;
            deviationList.Update();
            SecurityUtility.CreateListGroup(currentWeb, deviationList, QSEAdministratorsGroup.Name, QSEAdministratorsGroup.Description, QSEAdministratorsGroup.role);
            SecurityUtility.AddExistingGroupCustomDefinition(currentWeb, deviationList, currentWeb.AssociatedMemberGroup, currentWeb.Site.RootWeb.RoleDefinitions[DeviationsList.DeviationRoleDefinition]);
            //SecurityUtility.AddExistingGroupCustomDefinition(currentWeb,deviationList, currentWeb.AssociatedOwnerGroup,currentWeb.Site.RootWeb.RoleDefinitions[
            SecurityUtility.AddExistingGroup(currentWeb, deviationList, currentWeb.AssociatedOwnerGroup, SPRoleType.Administrator);
            using (SPSite site = new SPSite(currentWeb.Site.ID))
            {
                SPWeb rootWeb = site.RootWeb;
                SPContentType deviationBaseContentType = null;
                SPContentType deviationContentType = null;
                SPContentType complaintsContentType = null;
                SPContentType suggestionsContentType = null;
                
                if (CustomListHelper.SiteContainsContentType(rootWeb, DeviationsList.DeviationBaseContentTypeId))
                {
                    deviationBaseContentType = rootWeb.ContentTypes[DeviationsList.DeviationBaseContentTypeId];
                    if (CustomListHelper.SiteContainsContentType(rootWeb, DeviationsList.DeviationContentTypeId))
                    {
                        deviationContentType = rootWeb.ContentTypes[DeviationsList.DeviationContentTypeId];
                    }
                    if (CustomListHelper.SiteContainsContentType(rootWeb, DeviationsList.ComplaintsContentTypeId))
                    {
                        complaintsContentType = rootWeb.ContentTypes[DeviationsList.ComplaintsContentTypeId];
                    }
                    if (CustomListHelper.SiteContainsContentType(rootWeb, DeviationsList.SuggestionsContentTypeId))
                    {
                        suggestionsContentType = rootWeb.ContentTypes[DeviationsList.SuggestionsContentTypeId];
                    }
                }
                else
                {
                    //KEY DATE
                    string fieldInternalName = CustomListHelper.CreateSiteColumn(rootWeb, DeviationsList.KeyDate, SPFieldType.DateTime, true);
                    SPFieldDateTime keyDateField = (SPFieldDateTime)rootWeb.Fields.GetField(fieldInternalName);
                    keyDateField.DefaultValue = SPUtility.CreateISO8601DateTimeFromSystemDateTime(DateTime.Now);
                    keyDateField.Title = DeviationsList.KeyDateDisplayName;
                    keyDateField.Group = DeviationsList.ListName;
                    keyDateField.DisplayFormat = SPDateTimeFieldFormatType.DateOnly;
                    keyDateField.Update();
                    SPFieldLink keyDateFieldLink = new SPFieldLink(keyDateField);

                    //Description Field
                    fieldInternalName = CustomListHelper.CreateSiteColumn(rootWeb, DeviationsList.DeviationDescription, SPFieldType.Note, false);
                    SPFieldMultiLineText descriptionField = (SPFieldMultiLineText)rootWeb.Fields.GetField(fieldInternalName);
                    descriptionField.Title = DeviationsList.DeviationDescriptionDisplayName;
                    descriptionField.Group = DeviationsList.ListName;
                    descriptionField.NumberOfLines = 15;
                    descriptionField.RichText = true;
                    descriptionField.RichTextMode = SPRichTextMode.FullHtml;
                    descriptionField.Update();
                    SPFieldLink descriptionFieldLink = new SPFieldLink(descriptionField);

                    //STATUS
                    TaxonomyUtility.CreateTermSet(currentWeb, DeviationsList.TermGroup, DeviationsList.TermSetStatus);
                    fieldInternalName = CustomListHelper.CreateTaxonomySiteColumn(site, DeviationsList.DeviationStatus);
                    TaxonomyField statusField = rootWeb.Fields[fieldInternalName] as TaxonomyField;
                    TaxonomySession session = new TaxonomySession(site);
                    var termStore = session.TermStores[TermStoreName.TermStore];
                    var group = from g in termStore.Groups where g.Name == DeviationsList.TermGroup select g;
                    var termSet = group.FirstOrDefault().TermSets[DeviationsList.TermSetStatus];
                    statusField.SspId = termSet.TermStore.Id;
                    statusField.TermSetId = termSet.Id;
                    statusField.TargetTemplate = string.Empty;
                    statusField.AllowMultipleValues = false;
                    statusField.CreateValuesInEditForm = false;
                    statusField.Open = true;
                    statusField.AnchorId = Guid.Empty;
                    statusField.Group = DeviationsList.ListName;
                    statusField.Title = DeviationsList.DeviationStatusDisplayName;
                    statusField.Update();
                    SPFieldLink statusFieldLink = new SPFieldLink(statusField);

                    //RESPONSIBLE
                    fieldInternalName = CustomListHelper.CreateSiteColumn(rootWeb, DeviationsList.Responsible, SPFieldType.User, false);
                    SPFieldUser responsibleField = (SPFieldUser)rootWeb.Fields.GetField(fieldInternalName);
                    responsibleField.Title = DeviationsList.ResponsibleDisplayName;
                    responsibleField.AllowMultipleValues = false;
                    responsibleField.Group = DeviationsList.ListName;
                    responsibleField.Update();
                    SPFieldLink responsibleFieldLink = new SPFieldLink(responsibleField);

                    //DECISION DATE
                    fieldInternalName = CustomListHelper.CreateSiteColumn(rootWeb, DeviationsList.DecisionDate, SPFieldType.DateTime, false);
                    SPFieldDateTime decisionDateField = (SPFieldDateTime)rootWeb.Fields.GetField(fieldInternalName);
                    decisionDateField.Title = DeviationsList.DecisionDateDisplayName;
                    decisionDateField.Group = DeviationsList.ListName;
                    decisionDateField.DisplayFormat = SPDateTimeFieldFormatType.DateOnly;
                    decisionDateField.Update();
                    SPFieldLink decisionDateFieldLink = new SPFieldLink(decisionDateField);

                    //DECISION COMMENT
                    fieldInternalName = CustomListHelper.CreateSiteColumn(rootWeb, DeviationsList.DecisionComment, SPFieldType.Note, false);
                    SPFieldMultiLineText decisionCommentField = (SPFieldMultiLineText)rootWeb.Fields.GetField(fieldInternalName);
                    decisionCommentField.Title = DeviationsList.DecisionCommentDisplayName;
                    decisionCommentField.Group = DeviationsList.ListName;
                    decisionCommentField.AppendOnly = true;
                    decisionCommentField.NumberOfLines = 15;
                    decisionCommentField.RichText = true;
                    decisionCommentField.RichTextMode = SPRichTextMode.FullHtml;
                    decisionCommentField.Update();
                    SPFieldLink decisionCommentFieldLink = new SPFieldLink(decisionCommentField);

                    //ACTION BY DATE
                    fieldInternalName = CustomListHelper.CreateSiteColumn(rootWeb, DeviationsList.ActionByDate, SPFieldType.DateTime, false);
                    SPFieldDateTime actionDateField = (SPFieldDateTime)rootWeb.Fields.GetField(fieldInternalName);
                    actionDateField.Title = DeviationsList.ActionByDateDisplayName;
                    actionDateField.Group = DeviationsList.ListName;
                    actionDateField.DisplayFormat = SPDateTimeFieldFormatType.DateOnly;
                    actionDateField.Update();
                    SPFieldLink actionDateFieldLink = new SPFieldLink(actionDateField);

                    //FOLLOWUP DATE
                    fieldInternalName = CustomListHelper.CreateSiteColumn(rootWeb, DeviationsList.FollowUpDate, SPFieldType.DateTime, false);
                    SPFieldDateTime followUpDateField = (SPFieldDateTime)rootWeb.Fields.GetField(fieldInternalName);
                    followUpDateField.Title = DeviationsList.FollowUpDateDisplayName;
                    followUpDateField.Group = DeviationsList.ListName;
                    followUpDateField.DisplayFormat = SPDateTimeFieldFormatType.DateOnly;
                    followUpDateField.Update();
                    SPFieldLink followupDateFieldLink = new SPFieldLink(followUpDateField);

                    //FOLLOWUP COMMENT
                    fieldInternalName = CustomListHelper.CreateSiteColumn(rootWeb, DeviationsList.FollowUpComment, SPFieldType.Note, false);
                    SPFieldMultiLineText followUpCommentField = (SPFieldMultiLineText)rootWeb.Fields.GetField(fieldInternalName);
                    followUpCommentField.Title = DeviationsList.FollowUpCommentDisplayName;
                    followUpCommentField.Group = DeviationsList.ListName;
                    followUpCommentField.NumberOfLines = 15;
                    followUpCommentField.RichText = true;
                    followUpCommentField.RichTextMode = SPRichTextMode.FullHtml;
                    followUpCommentField.Update();
                    SPFieldLink followUpCommentFieldLink = new SPFieldLink(followUpCommentField);

                    //----CONTENT TYPE------
                    deviationBaseContentType = new SPContentType(DeviationsList.DeviationBaseContentTypeId,
                                                                    rootWeb.ContentTypes,
                                                                    DeviationsList.ListContentTypeBase);

                    deviationBaseContentType.FieldLinks.Add(keyDateFieldLink);
                    deviationBaseContentType.FieldLinks.Add(statusFieldLink);
                    deviationBaseContentType.FieldLinks.Add(descriptionFieldLink);
                    deviationBaseContentType.FieldLinks.Add(responsibleFieldLink);
                    deviationBaseContentType.FieldLinks.Add(decisionDateFieldLink);
                    deviationBaseContentType.FieldLinks.Add(decisionCommentFieldLink);
                    deviationBaseContentType.FieldLinks.Add(actionDateFieldLink);
                    deviationBaseContentType.FieldLinks.Add(followupDateFieldLink);
                    deviationBaseContentType.FieldLinks.Add(followUpCommentFieldLink);

                    deviationBaseContentType.Group = DeviationsList.AtkinsContentTypeGroup;
                    rootWeb.ContentTypes.Add(deviationBaseContentType);

                    //DEFINE AND ADD CHILD CONTENTTYPES
                    deviationContentType = new SPContentType(DeviationsList.DeviationContentTypeId, rootWeb.ContentTypes, DeviationsList.ListContentTypeDeviations);
                    deviationContentType.Group = DeviationsList.AtkinsContentTypeGroup;
                    rootWeb.ContentTypes.Add(deviationContentType);
                    
                    complaintsContentType = new SPContentType(DeviationsList.ComplaintsContentTypeId, rootWeb.ContentTypes, DeviationsList.ListContentTypeComplaints);
                    complaintsContentType.Group = DeviationsList.AtkinsContentTypeGroup;
                    rootWeb.ContentTypes.Add(complaintsContentType);

                    suggestionsContentType = new SPContentType(DeviationsList.SuggestionsContentTypeId, rootWeb.ContentTypes, DeviationsList.ListContentTypeSuggestions);
                    suggestionsContentType.Group = DeviationsList.AtkinsContentTypeGroup;
                    rootWeb.ContentTypes.Add(suggestionsContentType);

                    rootWeb.Update();
                }
                if (deviationBaseContentType != null &&
                !CustomListHelper.ListContainsContentType(deviationList,
                                                            DeviationsList.DeviationBaseContentTypeId))
                {
                    deviationList.ContentTypesEnabled = true;
                    deviationList.ContentTypes.Add(deviationBaseContentType);
                    deviationList.ContentTypes[0].Delete();
                    deviationList.ContentTypes.Add(deviationContentType);
                    deviationList.ContentTypes.Add(complaintsContentType);
                    deviationList.ContentTypes.Add(suggestionsContentType);

                    deviationList.Update();

                    SPView defaultView = deviationList.DefaultView;

                    SPField keyDate = CustomListHelper.ReturnListField(deviationList, DeviationsList.KeyDate);
                    if (keyDate != null)
                        defaultView.ViewFields.Add(keyDate);

                    SPField author = CustomListHelper.ReturnListField(deviationList, "Author");
                    if (author != null)
                        defaultView.ViewFields.Add(author);

                    SPField responsible = CustomListHelper.ReturnListField(deviationList, DeviationsList.Responsible);
                    if (responsible != null)
                        defaultView.ViewFields.Add(responsible);

                    SPField status = CustomListHelper.ReturnListField(deviationList, DeviationsList.DeviationStatus);
                    if (status != null)
                        defaultView.ViewFields.Add(status);

                    defaultView.Query = "<OrderBy><FieldRef Name='"+CustomListHelper.ReturnTrimmedString(DeviationsList.KeyDate)+"' Ascending='TRUE'/></OrderBy>";

                    defaultView.Update();


                    //WebPartView
                    System.Collections.Specialized.StringCollection viewFields = new System.Collections.Specialized.StringCollection();
                    viewFields.Add("LinkTitle");
                    viewFields.Add(CustomListHelper.ReturnTrimmedString(DeviationsList.KeyDate));
                    SPView webPartView = deviationList.Views.Add(DeviationsList.webPartView, viewFields, "", 5, false, false);
                    webPartView.TabularView = false;
                    webPartView.Update();


                    currentWeb.Update();
                    //ADD METADATA NAVIGATION TO LIST
                    MetadataNavigationSettings listNavSettings = MetadataNavigationSettings.GetMetadataNavigationSettings(deviationList);
                    MetadataNavigationHierarchy navigationStatus = new MetadataNavigationHierarchy(status);
                    listNavSettings.AddConfiguredHierarchy(navigationStatus);
                    

                    MetadataNavigationKeyFilter keyfilterAuthor = new MetadataNavigationKeyFilter(author);
                    listNavSettings.AddConfiguredKeyFilter(keyfilterAuthor);

                    MetadataNavigationKeyFilter keyfilterAuditor = new MetadataNavigationKeyFilter(responsible);
                    listNavSettings.AddConfiguredKeyFilter(keyfilterAuditor);

                    MetadataNavigationKeyFilter keyfilterApprover = new MetadataNavigationKeyFilter(keyDate);
                    listNavSettings.AddConfiguredKeyFilter(keyfilterApprover);

                    MetadataNavigationSettings.SetMetadataNavigationSettings(deviationList, listNavSettings, true);

                }
            }
        }

        private static void CreateProcessStepContentTypeList(SPWeb currentWeb)
        {
            Guid documentLibraryGuid = currentWeb.Lists.Add(CustomListHelper.ReturnTrimmedString(ProcessStepList.ListName), ProcessStepList.ListDescription, SPListTemplateType.GenericList);
            SPList processStepList = currentWeb.Lists[documentLibraryGuid];
            processStepList.Title = ProcessStepList.ListName;
            processStepList.BreakRoleInheritance(false);
            processStepList.OnQuickLaunch = true;
            processStepList.Update();
            //QSEResultingDocumentsAdministrators
            SecurityUtility.CreateListGroup(currentWeb, processStepList, QSEAdministratorsGroup.Name, QSEAdministratorsGroup.Description, QSEAdministratorsGroup.role);
            //Site Members
            SecurityUtility.AddExistingGroup(currentWeb, processStepList,currentWeb.AssociatedMemberGroup,SPRoleType.Reader);
            //administrators
            SecurityUtility.AddExistingGroup(currentWeb, processStepList, currentWeb.AssociatedOwnerGroup, SPRoleType.Administrator);
            using (SPSite site = new SPSite(currentWeb.Site.ID))
            {
                SPWeb rootWeb = site.RootWeb;
                SPContentType processStepContentType = null;
                if (CustomListHelper.SiteContainsContentType(rootWeb, ProcessStepList.processStepContentTypeId))
                {
                    processStepContentType = rootWeb.ContentTypes[ProcessStepList.processStepContentTypeId];
                }
                else
                {
                    //RESULTING DOCUMENT CATEGORY   -   CREATE QSE TERMSET and Group
                    TaxonomyUtility.CreateTermSet(currentWeb, ProcessStepList.TermGroup, ProcessStepList.TermSetProcess);
                    string fieldInternalName = CustomListHelper.CreateTaxonomySiteColumn(site, ProcessStepList.Process);
                    TaxonomyField processStepField = rootWeb.Fields[fieldInternalName] as TaxonomyField;
                    TaxonomySession session = new TaxonomySession(site);
                    var termStore = session.TermStores[TermStoreName.TermStore];
                    var group = from g in termStore.Groups where g.Name == ProcessStepList.TermGroup select g;
                    var termSet = group.FirstOrDefault().TermSets[ProcessStepList.TermSetProcess];
                    processStepField.SspId = termSet.TermStore.Id;
                    processStepField.TermSetId = termSet.Id;
                    processStepField.TargetTemplate = string.Empty;
                    processStepField.AllowMultipleValues = false;
                    processStepField.CreateValuesInEditForm = false;
                    processStepField.Open = true;
                    processStepField.AnchorId = Guid.Empty;
                    processStepField.Group = ProcessStepList.ListName;
                    processStepField.Title = ProcessStepList.ProcessDisplayName;
                    processStepField.Update();
                    SPFieldLink processStepFieldLink = new SPFieldLink(processStepField);

                    //Process Description Field
                    fieldInternalName = CustomListHelper.CreateSiteColumn(rootWeb, ProcessStepList.ProcessDescription, SPFieldType.Note, true);
                    SPFieldMultiLineText descriptionField = (SPFieldMultiLineText)rootWeb.Fields.GetField(fieldInternalName);
                    descriptionField.NumberOfLines = 15;
                    descriptionField.Title = ProcessStepList.ProcessDescriptionDisplayName;
                    descriptionField.Group = ProcessStepList.ListName;
                    descriptionField.Update();
                    SPFieldLink descriptionFieldLink = new SPFieldLink(descriptionField);

                    processStepContentType = new SPContentType(ProcessStepList.processStepContentTypeId,
                                                                   rootWeb.ContentTypes,
                                                                   ProcessStepList.ListContentType);
                    processStepContentType.FieldLinks.Add(processStepFieldLink);
                    processStepContentType.FieldLinks.Add(descriptionFieldLink);
                    processStepContentType.Group = ProcessStepList.AtkinsContentTypeGroup;
                    rootWeb.ContentTypes.Add(processStepContentType);
                    rootWeb.Update();


                }
                if (processStepContentType != null &&
                !CustomListHelper.ListContainsContentType(processStepList, ProcessStepList.processStepContentTypeId))
                {
                    processStepList.ContentTypesEnabled = true;
                    processStepList.ContentTypes.Add(processStepContentType);
                    processStepList.ContentTypes[0].Delete();
                    processStepList.Update();

                    SPView defaultView = processStepList.DefaultView;
                    //defaultView.ViewFields.Delete(CustomListHelper.ReturnListField(processStepList, "Modified"));
                    //defaultView.ViewFields.Delete(CustomListHelper.ReturnListField(processStepList, "Editor"));
                    defaultView.ViewFields.Add(CustomListHelper.ReturnListField(processStepList, ProcessStepList.Process));
                    defaultView.ViewFields.Add(CustomListHelper.ReturnListField(processStepList, ProcessStepList.ProcessDescription));

                    defaultView.Update();


                    //WebPartView
                    System.Collections.Specialized.StringCollection viewFields = new System.Collections.Specialized.StringCollection();
                    viewFields.Add("LinkTitle");
                    SPView webPartView = processStepList.Views.Add(ProcessStepList.webPartView, viewFields, "", 5, false, false);
                    webPartView.TabularView = false;
                    webPartView.Update();


                    currentWeb.Update();
                    //ADD METADATA NAVIGATION TO LIST
                    MetadataNavigationSettings listNavSettings = MetadataNavigationSettings.GetMetadataNavigationSettings(processStepList);
                    MetadataNavigationHierarchy navigationResultingDocumentCategory = new MetadataNavigationHierarchy(CustomListHelper.ReturnListField(processStepList, ProcessStepList.Process));
                    listNavSettings.AddConfiguredHierarchy(navigationResultingDocumentCategory);
                    MetadataNavigationSettings.SetMetadataNavigationSettings(processStepList, listNavSettings, true); 
                }
            }
            
        }

        private static void CreateResultingDocumentContentTypeList(SPWeb currentWeb)
        {
            Guid documentLibraryGuid = currentWeb.Lists.Add(CustomListHelper.ReturnTrimmedString(ResultingDocuments.ListName), ResultingDocuments.ListDescription, SPListTemplateType.DocumentLibrary);
            SPList resultingDocumentsList = currentWeb.Lists[documentLibraryGuid];
            resultingDocumentsList.Title = ResultingDocuments.ListName;
            resultingDocumentsList.BreakRoleInheritance(false);
            resultingDocumentsList.OnQuickLaunch = true;
            resultingDocumentsList.Update();
            //QSEResultingDocumentsAdministrators
            SecurityUtility.CreateListGroup(currentWeb, resultingDocumentsList, QSEResultingDocumentsAdministrators.Name, QSEResultingDocumentsAdministrators.Description, QSEResultingDocumentsAdministrators.role);
            //QSEAdministrators
            SecurityUtility.CreateListGroup(currentWeb, resultingDocumentsList, QSEAdministratorsGroup.Name, QSEAdministratorsGroup.Description, QSEAdministratorsGroup.role);
            //administrators
            SecurityUtility.AddExistingGroup(currentWeb, resultingDocumentsList, currentWeb.AssociatedOwnerGroup, SPRoleType.Administrator);
            using (SPSite site = new SPSite(currentWeb.Site.ID))
            {
                SPWeb rootWeb = site.RootWeb;
                SPContentType resultingDocumentsContentType = null;
                if (CustomListHelper.SiteContainsContentType(rootWeb, ResultingDocuments.resultingDocumentsContentTypeId))
                {
                    resultingDocumentsContentType = rootWeb.ContentTypes[ResultingDocuments.resultingDocumentsContentTypeId];
                }
                else
                {
                    //RESULTING DOCUMENT CATEGORY   -   CREATE QSE TERMSET and Group
                    TaxonomyUtility.CreateTermSet(currentWeb, ResultingDocuments.TermGroup, ResultingDocuments.TermSetResultingDocumentCategory);
                    string fieldInternalName = CustomListHelper.CreateTaxonomySiteColumn(site, ResultingDocuments.ResultingDocumentCategory);
                    TaxonomyField resultingDocumentCategoryField = rootWeb.Fields[fieldInternalName] as TaxonomyField;
                    TaxonomySession session = new TaxonomySession(site);
                    var termStore = session.TermStores[TermStoreName.TermStore];
                    var group = from g in termStore.Groups where g.Name == ResultingDocuments.TermGroup select g;
                    var termSet = group.FirstOrDefault().TermSets[ResultingDocuments.TermSetResultingDocumentCategory];
                    resultingDocumentCategoryField.SspId = termSet.TermStore.Id;
                    resultingDocumentCategoryField.TermSetId = termSet.Id;
                    resultingDocumentCategoryField.TargetTemplate = string.Empty;
                    resultingDocumentCategoryField.AllowMultipleValues = false;
                    resultingDocumentCategoryField.CreateValuesInEditForm = false;
                    resultingDocumentCategoryField.Open = true;
                    resultingDocumentCategoryField.AnchorId = Guid.Empty;
                    resultingDocumentCategoryField.Group = ResultingDocuments.ListName;
                    resultingDocumentCategoryField.Title = ResultingDocuments.ResultingDocumentCategoryDisplayName;
                    resultingDocumentCategoryField.Update();
                    SPFieldLink resultingDocumentCategoryFieldLink = new SPFieldLink(resultingDocumentCategoryField);

                    //RESULTING DOCUMENT YEAR
                    fieldInternalName = CustomListHelper.CreateSiteColumn(rootWeb, ResultingDocuments.ResultingDocumentYear, SPFieldType.Choice, false);
                    SPFieldChoice resultingDocumentYearField = (SPFieldChoice)rootWeb.Fields.GetField(fieldInternalName);
                    resultingDocumentYearField.Title = ResultingDocuments.ResultingDocumentYearDisplayName;
                    resultingDocumentYearField.Group = ResultingDocuments.ListName;
                    for (int year = ResultingDocuments.ResultingDocumentYearStart; year <= ResultingDocuments.ResultingDocumentYearStop; year++)
                        resultingDocumentYearField.Choices.Add(year.ToString());
                    resultingDocumentYearField.Update();
                    SPFieldLink resultingDocumentYearFieldLink = new SPFieldLink(resultingDocumentYearField);

                    resultingDocumentsContentType = new SPContentType(ResultingDocuments.resultingDocumentsContentTypeId,
                                                                    rootWeb.ContentTypes,
                                                                    ResultingDocuments.ListContentType);
                    resultingDocumentsContentType.FieldLinks.Add(resultingDocumentCategoryFieldLink);
                    resultingDocumentsContentType.FieldLinks.Add(resultingDocumentYearFieldLink);
                    resultingDocumentsContentType.Group = ResultingDocuments.AtkinsContentTypeGroup;
                    rootWeb.ContentTypes.Add(resultingDocumentsContentType);
                    rootWeb.Update();
                }
                if (resultingDocumentsContentType != null &&
                !CustomListHelper.ListContainsContentType(resultingDocumentsList,ResultingDocuments.resultingDocumentsContentTypeId))
                {
                    resultingDocumentsList.ContentTypesEnabled = true;
                    resultingDocumentsList.ContentTypes.Add(resultingDocumentsContentType);
                    resultingDocumentsList.ContentTypes[0].Delete();
                    resultingDocumentsList.Update();

                    SPView defaultView = resultingDocumentsList.DefaultView;
                    defaultView.ViewFields.Delete(CustomListHelper.ReturnListField(resultingDocumentsList, "Modified"));
                    defaultView.ViewFields.Delete(CustomListHelper.ReturnListField(resultingDocumentsList, "Editor"));
                    SPField resultingDocumentCategory = CustomListHelper.ReturnListField(resultingDocumentsList, ResultingDocuments.ResultingDocumentCategory);
                    if (resultingDocumentCategory != null)
                        defaultView.ViewFields.Add(resultingDocumentCategory);
                    SPField resultingDocumentYear = CustomListHelper.ReturnListField(resultingDocumentsList, ResultingDocuments.ResultingDocumentYear);
                    if (resultingDocumentYear != null)
                        defaultView.ViewFields.Add(resultingDocumentYear);

                    SPField docId = CustomListHelper.ReturnListField(resultingDocumentsList, "_dlc_DocIdUrl");
                    if (docId != null)
                        defaultView.ViewFields.Add(docId);
                  

                    defaultView.Update();
                    currentWeb.Update();
                    //ADD METADATA NAVIGATION TO LIST
                    MetadataNavigationSettings listNavSettings = MetadataNavigationSettings.GetMetadataNavigationSettings(resultingDocumentsList);
                    MetadataNavigationHierarchy navigationResultingDocumentCategory = new MetadataNavigationHierarchy(resultingDocumentCategory);
                    listNavSettings.AddConfiguredHierarchy(navigationResultingDocumentCategory);

                    MetadataNavigationKeyFilter keyfilterResultingDocumentYear = new MetadataNavigationKeyFilter(resultingDocumentYear);
                    listNavSettings.AddConfiguredKeyFilter(keyfilterResultingDocumentYear);

                    MetadataNavigationSettings.SetMetadataNavigationSettings(resultingDocumentsList, listNavSettings, true);  
                }
            }

        }

        private static void CreateControllingDocumentContentTypeList(SPWeb currentWeb)
        {
            Guid documentLibraryGuid = currentWeb.Lists.Add(CustomListHelper.ReturnTrimmedString(ControllingDocuments.ListName), ControllingDocuments.ListDescription, SPListTemplateType.DocumentLibrary);
            SPList controllingDocumentsList = currentWeb.Lists[documentLibraryGuid];
            controllingDocumentsList.Title = ControllingDocuments.ListName;
            controllingDocumentsList.OnQuickLaunch = true;
            controllingDocumentsList.EnableVersioning = true;
            controllingDocumentsList.EnableMinorVersions = true;
            controllingDocumentsList.MajorWithMinorVersionsLimit = 5;
            controllingDocumentsList.BreakRoleInheritance(false);
            controllingDocumentsList.Update();

            //QSEAUTHORS
            SecurityUtility.CreateListGroup(currentWeb, controllingDocumentsList, QSEAuthorsGroup.Name, QSEAuthorsGroup.Description, QSEAuthorsGroup.role);
            //QSEAUDITORS
            SecurityUtility.CreateListGroup(currentWeb, controllingDocumentsList, QSEAuditorssGroup.Name, QSEAuditorssGroup.Description, QSEAuditorssGroup.role);
            //QSEAPPROVERS
            SecurityUtility.CreateListGroup(currentWeb, controllingDocumentsList, QSEApproverssGroup.Name, QSEApproverssGroup.Description, QSEApproverssGroup.role);
            //QSEADMINISTRATORS
            SecurityUtility.CreateListGroup(currentWeb, controllingDocumentsList, QSEAdministratorsGroup.Name, QSEAdministratorsGroup.Description, QSEAdministratorsGroup.role);
            //Membersgroup
            SecurityUtility.AddExistingGroup(currentWeb, controllingDocumentsList, currentWeb.AssociatedMemberGroup, SPRoleType.Reader);
            //administrators
            SecurityUtility.AddExistingGroup(currentWeb, controllingDocumentsList, currentWeb.AssociatedOwnerGroup, SPRoleType.Administrator);

            using (SPSite site = new SPSite(currentWeb.Site.ID))
            {
                SPWeb rootWeb = site.RootWeb;
                SPContentType controllingDocumentsContentType = null;
                if (CustomListHelper.SiteContainsContentType(rootWeb, ControllingDocuments.controllingDocumentsContentTypeId))
                {
                    controllingDocumentsContentType = rootWeb.ContentTypes[ControllingDocuments.controllingDocumentsContentTypeId];
                }
                else
                {
                    //ISO9001   -   CREATE QSE TERMSET and Group
                    TaxonomyUtility.CreateTermSet(currentWeb, ControllingDocuments.TermGroup, ControllingDocuments.TermSetISO9001);
                    string fieldInternalName = CustomListHelper.CreateTaxonomySiteColumn(site, ControllingDocuments.ISO9001);
                    TaxonomyField iso9001Field = rootWeb.Fields[fieldInternalName] as TaxonomyField;
                    TaxonomySession session = new TaxonomySession(site);
                    var termStore = session.TermStores[TermStoreName.TermStore];
                    var group = from g in termStore.Groups where g.Name == ControllingDocuments.TermGroup select g;
                    var termSet = group.FirstOrDefault().TermSets[ControllingDocuments.TermSetISO9001];
                    iso9001Field.SspId = termSet.TermStore.Id;
                    iso9001Field.TermSetId = termSet.Id;
                    iso9001Field.TargetTemplate = string.Empty;
                    iso9001Field.AllowMultipleValues = false;
                    iso9001Field.CreateValuesInEditForm = false;
                    iso9001Field.Open = true;
                    iso9001Field.AnchorId = Guid.Empty;
                    iso9001Field.Group = ControllingDocuments.ListName;
                    iso9001Field.Title = ControllingDocuments.ISO9001DisplayName;
                    iso9001Field.Update();
                    SPFieldLink iso9001FieldLink = new SPFieldLink(iso9001Field);

                    //ISO14001   -   CREATE QSE TERM GROUP AND TERMSET 
                    TaxonomyUtility.CreateTermSet(currentWeb, ControllingDocuments.TermGroup, ControllingDocuments.TermSetISO14001);
                    fieldInternalName = CustomListHelper.CreateTaxonomySiteColumn(site, ControllingDocuments.ISO14001);
                    TaxonomyField iso14001Field = rootWeb.Fields[fieldInternalName] as TaxonomyField;
                    group = from g in termStore.Groups where g.Name == ControllingDocuments.TermGroup select g;
                    termSet = group.FirstOrDefault().TermSets[ControllingDocuments.TermSetISO14001];
                    iso14001Field.SspId = termSet.TermStore.Id;
                    iso14001Field.TermSetId = termSet.Id;
                    iso14001Field.TargetTemplate = string.Empty;
                    iso14001Field.AllowMultipleValues = false;
                    iso14001Field.CreateValuesInEditForm = false;
                    iso14001Field.Open = true;
                    iso14001Field.AnchorId = Guid.Empty;
                    iso14001Field.Group = ControllingDocuments.ListName;
                    iso14001Field.Title = ControllingDocuments.ISO14001DisplayName;
                    iso14001Field.Update();
                    SPFieldLink iso14001FieldLink = new SPFieldLink(iso14001Field);

                    //ISO18001   -   CREATE QSE TERM GROUP AND TERMSET 
                    TaxonomyUtility.CreateTermSet(currentWeb, ControllingDocuments.TermGroup, ControllingDocuments.TermSetISO18001);
                    fieldInternalName = CustomListHelper.CreateTaxonomySiteColumn(site, ControllingDocuments.ISO18001);
                    TaxonomyField iso18001Field = rootWeb.Fields[fieldInternalName] as TaxonomyField;
                    group = from g in termStore.Groups where g.Name == ControllingDocuments.TermGroup select g;
                    termSet = group.FirstOrDefault().TermSets[ControllingDocuments.TermSetISO18001];
                    iso18001Field.SspId = termSet.TermStore.Id;
                    iso18001Field.TermSetId = termSet.Id;
                    iso18001Field.TargetTemplate = string.Empty;
                    iso18001Field.AllowMultipleValues = false;
                    iso18001Field.CreateValuesInEditForm = false;
                    iso18001Field.Open = true;
                    iso18001Field.AnchorId = Guid.Empty;
                    iso18001Field.Group = ControllingDocuments.ListName;
                    iso18001Field.Title = ControllingDocuments.ISO18001DisplayName;
                    iso18001Field.Update();
                    SPFieldLink iso18001FieldLink = new SPFieldLink(iso18001Field);

                    //Written By Field
                    fieldInternalName = CustomListHelper.CreateSiteColumn(rootWeb, ControllingDocuments.WrittenBy, SPFieldType.User,false);
                    SPFieldUser writtenByField = (SPFieldUser)rootWeb.Fields.GetField(fieldInternalName);
                    writtenByField.Title = ControllingDocuments.WrittenByDisplayName;
                    writtenByField.AllowMultipleValues = false;
                    writtenByField.Group = ControllingDocuments.ListName;
                    writtenByField.SelectionGroup = currentWeb.SiteGroups[QSEAuthorsGroup.Name].ID;
                    writtenByField.Update();
                    SPFieldLink writtenByFieldLink = new SPFieldLink(writtenByField);

                    //Auditor Field
                    fieldInternalName = CustomListHelper.CreateSiteColumn(rootWeb, ControllingDocuments.Auditor, SPFieldType.User, false);
                    SPFieldUser auditorField = (SPFieldUser)rootWeb.Fields.GetField(fieldInternalName);
                    auditorField.Title = ControllingDocuments.AuditorDisplayName;
                    auditorField.Group = ControllingDocuments.ListName;
                    auditorField.SelectionGroup = currentWeb.SiteGroups[QSEAuditorssGroup.Name].ID;
                    auditorField.Update();
                    SPFieldLink auditorFieldLink = new SPFieldLink(auditorField);

                    //Approver Field
                    fieldInternalName = CustomListHelper.CreateSiteColumn(rootWeb, ControllingDocuments.Approver, SPFieldType.User, false);
                    SPFieldUser approverField = (SPFieldUser)rootWeb.Fields.GetField(fieldInternalName);
                    approverField.Title = ControllingDocuments.ApproverDisplayName;
                    approverField.Group = ControllingDocuments.ListName;
                    approverField.SelectionGroup = currentWeb.SiteGroups[QSEApproverssGroup.Name].ID;
                    approverField.Update();
                    SPFieldLink approverFieldLink = new SPFieldLink(approverField);

                    fieldInternalName = CustomListHelper.CreateSiteColumn(rootWeb, ControllingDocuments.ValidUntil, SPFieldType.DateTime, false);
                    SPFieldDateTime validUntilField = (SPFieldDateTime)rootWeb.Fields.GetField(fieldInternalName);
                    validUntilField.Title = ControllingDocuments.ValidUntilDisplayName;
                    validUntilField.Group = ControllingDocuments.ListName;
                    validUntilField.DisplayFormat = SPDateTimeFieldFormatType.DateOnly;
                    validUntilField.Update();
                    SPFieldLink validUntilFieldLink = new SPFieldLink(validUntilField);


                    //----CONTENT TYPE------
                    controllingDocumentsContentType = new SPContentType(ControllingDocuments.controllingDocumentsContentTypeId,
                                                                    rootWeb.ContentTypes,
                                                                    ControllingDocuments.ListContentType);

                    controllingDocumentsContentType.FieldLinks.Add(iso9001FieldLink);
                    controllingDocumentsContentType.FieldLinks.Add(iso14001FieldLink);
                    controllingDocumentsContentType.FieldLinks.Add(iso18001FieldLink);
                    controllingDocumentsContentType.FieldLinks.Add(writtenByFieldLink);
                    controllingDocumentsContentType.FieldLinks.Add(auditorFieldLink);
                    controllingDocumentsContentType.FieldLinks.Add(approverFieldLink);
                    controllingDocumentsContentType.FieldLinks.Add(validUntilFieldLink);

                    controllingDocumentsContentType.Group = ControllingDocuments.AtkinsContentTypeGroup;
                    rootWeb.ContentTypes.Add(controllingDocumentsContentType);
                    rootWeb.Update();
                }
                if (controllingDocumentsContentType != null &&
                !CustomListHelper.ListContainsContentType(controllingDocumentsList,
                                                            ControllingDocuments.controllingDocumentsContentTypeId))
                {
                    controllingDocumentsList.ContentTypesEnabled = true;
                    controllingDocumentsList.ContentTypes.Add(controllingDocumentsContentType);
                    controllingDocumentsList.ContentTypes[0].Delete();
                    controllingDocumentsList.Update();

                    SPView defaultView = controllingDocumentsList.DefaultView;
                    defaultView.ViewFields.Delete(CustomListHelper.ReturnListField(controllingDocumentsList, "Modified"));
                    defaultView.ViewFields.Delete(CustomListHelper.ReturnListField(controllingDocumentsList, "Editor"));
                    SPField iso9001 = CustomListHelper.ReturnListField(controllingDocumentsList, ControllingDocuments.ISO9001);
                    if (iso9001 != null)
                        defaultView.ViewFields.Add(iso9001);
                    SPField iso14001 = CustomListHelper.ReturnListField(controllingDocumentsList, ControllingDocuments.ISO14001);
                    if (iso14001 != null)
                        defaultView.ViewFields.Add(iso14001);
                    SPField iso18001 = CustomListHelper.ReturnListField(controllingDocumentsList, ControllingDocuments.ISO18001);
                    if (iso18001 != null)
                        defaultView.ViewFields.Add(iso18001);
                    SPField writtenBy = CustomListHelper.ReturnListField(controllingDocumentsList, ControllingDocuments.WrittenBy);
                    if (writtenBy != null)
                        defaultView.ViewFields.Add(writtenBy);
                    SPField validUntil = CustomListHelper.ReturnListField(controllingDocumentsList, ControllingDocuments.ValidUntil);
                    if (validUntil != null)
                        defaultView.ViewFields.Add(validUntil);

                    SPField docId = CustomListHelper.ReturnListField(controllingDocumentsList, "_dlc_DocIdUrl");
                    if (docId != null)
                        defaultView.ViewFields.Add(docId);
                    

                    SPField auditor = CustomListHelper.ReturnListField(controllingDocumentsList, ControllingDocuments.Auditor);
                    SPField approver = CustomListHelper.ReturnListField(controllingDocumentsList, ControllingDocuments.Approver);

                    defaultView.Update();
                    currentWeb.Update();
                    //ADD METADATA NAVIGATION TO LIST
                    MetadataNavigationSettings listNavSettings = MetadataNavigationSettings.GetMetadataNavigationSettings(controllingDocumentsList);
                    MetadataNavigationHierarchy navigationIso9001 = new MetadataNavigationHierarchy(iso9001);
                    listNavSettings.AddConfiguredHierarchy(navigationIso9001);
                    MetadataNavigationHierarchy navigationIso14001 = new MetadataNavigationHierarchy(iso14001);
                    listNavSettings.AddConfiguredHierarchy(navigationIso14001);
                    MetadataNavigationHierarchy navigationIso18001 = new MetadataNavigationHierarchy(iso18001);
                    listNavSettings.AddConfiguredHierarchy(navigationIso18001);

                    MetadataNavigationKeyFilter keyfilterAuthor = new MetadataNavigationKeyFilter(writtenBy);
                    listNavSettings.AddConfiguredKeyFilter(keyfilterAuthor);

                    MetadataNavigationKeyFilter keyfilterAuditor = new MetadataNavigationKeyFilter(auditor);
                    listNavSettings.AddConfiguredKeyFilter(keyfilterAuditor);

                    MetadataNavigationKeyFilter keyfilterApprover = new MetadataNavigationKeyFilter(approver);
                    listNavSettings.AddConfiguredKeyFilter(keyfilterApprover);

                    MetadataNavigationKeyFilter keyfilterValidUntil = new MetadataNavigationKeyFilter(validUntil);
                    listNavSettings.AddConfiguredKeyFilter(keyfilterValidUntil);

                    MetadataNavigationSettings.SetMetadataNavigationSettings(controllingDocumentsList, listNavSettings, true);  

                }
            }

        }

        // Uncomment the method below to handle the event raised before a feature is deactivated.

        //public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
