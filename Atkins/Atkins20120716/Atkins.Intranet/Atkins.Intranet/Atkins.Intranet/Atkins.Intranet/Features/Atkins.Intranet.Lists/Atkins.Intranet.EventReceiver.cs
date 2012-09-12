using System;
using System.Runtime.InteropServices;
using Atkins.Intranet.Utilities.HelperUtils;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using System.Linq;
using Microsoft.Office.DocumentManagement.MetadataNavigation;



namespace Atkins.Intranet.Features.Atkins.Intranet.Lists
{
    [Guid("9b3d0b54-0515-4e76-aa0b-7ee53e58a641")]
    public class AtkinsIntranetEventReceiver : SPFeatureReceiver
    {
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            //Create the office list in the root
            try
            {
                SPWeb currentWeb = (SPWeb) properties.Feature.Parent;
                if (currentWeb.IsRootWeb)
                {
                    //Office Information list
                    SPList officeList = CustomListHelper.ReturnList(currentWeb, OfficeFields.ListName);
                    if (officeList == null)
                    {
                        CreateOfficeContentTypeList(currentWeb);
                    }
                    //TEMPLATE DOCUMENT LIBRARY
                    SPList templateDocumentLibrary = CustomListHelper.ReturnList(currentWeb, TemplateDocuments.ListName);
                    if (templateDocumentLibrary == null)
                    {
                        CreateTemplateDocumentLibraryContentTypeList(currentWeb);
                    }
                    SPList areaList = CustomListHelper.ReturnList(currentWeb, AreaList.ListName);
                    if (areaList == null)
                    {
                        CreateAreaList(currentWeb);
                    }
                    //LINKS LIST
                    SPList linksList = CustomListHelper.ReturnList(currentWeb, LinksStartSite.ListName);
                    if (linksList == null)
                    {
                        CreateLinksList(currentWeb);
                    }

                    #region commentCode
                    //PERSONAL LINKS LIST
                    //SPList personalLinksList = CustomListHelper.ReturnList(currentWeb, PersonalLinksStartSite.ListName);
                    //if (personalLinksList == null)
                    //{
                        //CreatePersonalLinksList(currentWeb);
                    //}
                    #endregion

                    

                    //CALENDAR LIST
                    SPList calendarList = CustomListHelper.ReturnList(currentWeb, CalendarStartSite.ListName);
                    if (calendarList == null)
                    {
                        CreateCalendarList(currentWeb);
                    }
                    //MANUALS DOCUMENTS
                    SPList manualsDocumentsList = CustomListHelper.ReturnList(currentWeb, ManualsDocuments.ListName);
                    if (manualsDocumentsList == null)
                    {
                        CreateManualsDocumentsList(currentWeb);
                    }

                }
            }
            catch (SPException exception)
            {
                throw exception;
            }
        }
        private static void CreateManualsDocumentsList(SPWeb currentWeb)
        {

            Guid listGuid = currentWeb.Lists.Add(CustomListHelper.ReturnTrimmedString(ManualsDocuments.ListName), ManualsDocuments.ListDescription, SPListTemplateType.DocumentLibrary);
            SPList manualsDocumentsList = currentWeb.Lists[listGuid];
            SPField titleField = manualsDocumentsList.Fields[SPBuiltInFieldId.Title];
            titleField.Title = ManualsDocuments.TitleDisplayName;
            titleField.Update();

            using (SPSite site = new SPSite(currentWeb.Site.ID))
            {
                SPWeb rootWeb = site.RootWeb;
                SPContentType manualsDocumentsContentType = null;
                if (CustomListHelper.SiteContainsContentType(rootWeb, ManualsDocuments.ContentTypeId))
                {
                    manualsDocumentsContentType = rootWeb.ContentTypes[ManualsDocuments.ContentTypeId];
                }
                else
                {
                    string fieldInternalName = CustomListHelper.CreateTaxonomySiteColumn(site, ManualsDocuments.ManualsDocumentCategory);
                    TaxonomyField manualsDocumentsCategoryField = rootWeb.Fields[fieldInternalName] as TaxonomyField;
                    manualsDocumentsCategoryField.AllowMultipleValues = true;
                    manualsDocumentsCategoryField.CreateValuesInEditForm = false;
                    manualsDocumentsCategoryField.Open = true;
                    manualsDocumentsCategoryField.Group = ManualsDocuments.ListName;
                    manualsDocumentsCategoryField.Title = ManualsDocuments.ManualsDocumentCategoryDisplayName;
                    TaxonomyUtility.ConnectTaxonomyField(site, manualsDocumentsCategoryField.Id, TermStoreName.TermGroup, ManualsDocuments.TermSetCategory);
                    SPFieldLink manualsDocumentsCategoryFieldLink = new SPFieldLink(manualsDocumentsCategoryField);


                    fieldInternalName = CustomListHelper.CreateSiteColumn(rootWeb, ManualsDocuments.ManualsDocumentDescription, SPFieldType.Note, false);
                    SPFieldMultiLineText descriptionField = (SPFieldMultiLineText)rootWeb.Fields.GetField(fieldInternalName);
                    descriptionField.NumberOfLines = 15;
                    descriptionField.RichText = true;
                    descriptionField.RichTextMode = SPRichTextMode.FullHtml;
                    descriptionField.Title = ManualsDocuments.ManualsDocumentDescriptionDisplayName;
                    descriptionField.Group = ManualsDocuments.ListName;
                    descriptionField.Update();
                    SPFieldLink descriptionFieldLink = new SPFieldLink(descriptionField);


                    manualsDocumentsContentType = new SPContentType(ManualsDocuments.ContentTypeId,
                                                                  rootWeb.ContentTypes,
                                                                  ManualsDocuments.ListContentType);
                    manualsDocumentsContentType.FieldLinks.Add(manualsDocumentsCategoryFieldLink);
                    manualsDocumentsContentType.FieldLinks.Add(descriptionFieldLink);
                    manualsDocumentsContentType.Group = ManualsDocuments.AtkinsContentTypeGroup;
                    rootWeb.ContentTypes.Add(manualsDocumentsContentType);
                    rootWeb.Update();
                }
                if (manualsDocumentsContentType != null &&
                !CustomListHelper.ListContainsContentType(manualsDocumentsList, ManualsDocuments.ContentTypeId))
                {
                    manualsDocumentsList.ContentTypesEnabled = true;
                    manualsDocumentsList.ContentTypes.Add(manualsDocumentsContentType);
                    manualsDocumentsList.ContentTypes[0].Delete();
                    manualsDocumentsList.Update();

                    SPView defaultView = manualsDocumentsList.DefaultView;
                    defaultView.ViewFields.Delete(CustomListHelper.ReturnListField(manualsDocumentsList, "Editor"));
                    defaultView.ViewFields.Add(CustomListHelper.ReturnListField(manualsDocumentsList, ManualsDocuments.ManualsDocumentCategory));
                    

                    SPField docId = CustomListHelper.ReturnListField(manualsDocumentsList, "_dlc_DocIdUrl");
                    if (docId != null)
                        defaultView.ViewFields.Add(docId);

                    defaultView.Update();

                    currentWeb.Update();
                    //ADD METADATA NAVIGATION TO LIST
                    MetadataNavigationSettings listNavSettings = MetadataNavigationSettings.GetMetadataNavigationSettings(manualsDocumentsList);
                    MetadataNavigationHierarchy navigationManualDocumentCategory = new MetadataNavigationHierarchy(CustomListHelper.ReturnListField(manualsDocumentsList, ManualsDocuments.ManualsDocumentCategory));
                    listNavSettings.AddConfiguredHierarchy(navigationManualDocumentCategory);
                    MetadataNavigationSettings.SetMetadataNavigationSettings(manualsDocumentsList, listNavSettings, true);
                }
            }

            currentWeb.Update();
        }


        private static void CreateAreaList(SPWeb currentWeb)
        {
            Guid listGuid = currentWeb.Lists.Add(CustomListHelper.ReturnTrimmedString(AreaList.ListName), AreaList.ListDescription, SPListTemplateType.GenericList);
            SPList areaList = currentWeb.Lists[listGuid];
            SPField titleField = areaList.Fields[SPBuiltInFieldId.Title];
            titleField.Title = AreaList.TitleDisplayName;
            titleField.Update();

            currentWeb.Update();
        }

        private static void CreateCalendarList(SPWeb currentWeb)
        {
            Guid listGuid = currentWeb.Lists.Add(CustomListHelper.ReturnTrimmedString(CalendarStartSite.ListName), CalendarStartSite.ListDescription, SPListTemplateType.Events);
            SPList calendarList = currentWeb.Lists[listGuid];

            if (!CustomListHelper.checkIfViewExist(calendarList, CalendarStartSite.webPartView))
            {
                CustomListHelper.CreateView(calendarList, CalendarStartSite.webPartView, CustomListHelper.returnStringArray(CalendarStartSite.webPartViewFields), CalendarStartSite.webPartQuery,CalendarStartSite.webPartRowLimit);
            }
        }
        private static void CreatePersonalLinksList(SPWeb currentWeb)
        {

            Guid listGuid = currentWeb.Lists.Add(CustomListHelper.ReturnTrimmedString(PersonalLinksStartSite.ListName), PersonalLinksStartSite.ListDescription, SPListTemplateType.Links);
            SPList personalLinksList = currentWeb.Lists[listGuid];
            personalLinksList.Title = PersonalLinksStartSite.ListName;
            personalLinksList.Update();
            if (!CustomListHelper.checkIfViewExist(personalLinksList, PersonalLinksStartSite.webPartView))
            {
                CustomListHelper.CreateView(personalLinksList, PersonalLinksStartSite.webPartView, CustomListHelper.returnStringArray(PersonalLinksStartSite.webPartViewFields), PersonalLinksStartSite.webPartQuery, 5);
            }
        }

        private static void CreateLinksList(SPWeb currentWeb)
        {

            using (SPSite site = new SPSite(currentWeb.Site.ID))
            {
                SPWeb rootWeb = site.RootWeb;
                Guid listGuid = currentWeb.Lists.Add(CustomListHelper.ReturnTrimmedString(LinksStartSite.ListName), LinksStartSite.ListDescription, SPListTemplateType.Links);
                SPList linksList = currentWeb.Lists[listGuid];
                linksList.Title = LinksStartSite.ListName;
                string fieldInternalName = CustomListHelper.CreateSiteColumn(currentWeb, LinksStartSite.activeField, SPFieldType.Boolean, false);


               
                SPFieldBoolean activeField = (SPFieldBoolean)currentWeb.Fields.GetField(fieldInternalName);
                activeField.Group = LinksStartSite.ListName;
                activeField.Title = LinksStartSite.activeFieldDisplayName;
                activeField.Update();
                linksList.Fields.Add(activeField);

                //ADD AREA FIELD LOOKUP

                SPList areaList = rootWeb.Lists.TryGetList(AreaList.ListName);
                if (areaList != null)
                {
                    
                    string internalName = linksList.Fields.AddLookup(LinksStartSite.areaField, areaList.ID, rootWeb.ID,false);
                    SPFieldLookup areaField = (SPFieldLookup)linksList.Fields[internalName];
                    areaField.LookupField = areaList.Fields[SPBuiltInFieldId.Title].InternalName;
                    areaField.Title = LinksStartSite.areaFieldDisplayName;
                    areaField.AllowMultipleValues = true;
                    areaField.Update();
                }


                linksList.Update();
                SPView defaultView = linksList.DefaultView;
                defaultView.ViewFields.Add(activeField);
                defaultView.Update();

                if (!CustomListHelper.checkIfViewExist(linksList, LinksStartSite.webPartView))
                {
                    CustomListHelper.CreateView(linksList, LinksStartSite.webPartView, CustomListHelper.returnStringArray(LinksStartSite.webPartViewFields), LinksStartSite.webPartQuery, 5);
                }
            }
        }

        private static void CreateTemplateDocumentLibraryContentTypeList(SPWeb currentWeb)
        {

            Guid documentLibraryGuid = currentWeb.Lists.Add(CustomListHelper.ReturnTrimmedString(TemplateDocuments.ListName), TemplateDocuments.ListDescription, SPListTemplateType.DocumentLibrary);
            SPList templateDocumentsList = currentWeb.Lists[documentLibraryGuid];
            templateDocumentsList.Title = TemplateDocuments.ListName;
            templateDocumentsList.EnableVersioning = true;
            templateDocumentsList.BreakRoleInheritance(false);
            templateDocumentsList.Update();
            //QSEResultingDocumentsAdministrators
            SecurityUtility.CreateListGroup(currentWeb, templateDocumentsList, TemplateDocumentAdministrators.Name, TemplateDocumentAdministrators.Description, TemplateDocumentAdministrators.role);
            //Site Members
            SecurityUtility.AddExistingGroup(currentWeb, templateDocumentsList, currentWeb.AssociatedMemberGroup, SPRoleType.Reader);
            using (SPSite site = new SPSite(currentWeb.Site.ID))
            {
                SPWeb rootWeb = site.RootWeb;
                SPContentType templateDocumentsContentType = null;
                if (CustomListHelper.SiteContainsContentType(rootWeb, TemplateDocuments.templateDocumentContentTypeId))
                {
                    templateDocumentsContentType = rootWeb.ContentTypes[TemplateDocuments.templateDocumentContentTypeId];
                }
                else
                {
                    //TEMPLATE DOCUMENT CATEGORY   -   CREATE QSE TERMSET and Group
                    
                    string fieldInternalName = CustomListHelper.CreateTaxonomySiteColumn(site, TemplateDocuments.TemplateDocumentCategory);
                    TaxonomyField templateDocumentCategoryField = rootWeb.Fields[fieldInternalName] as TaxonomyField;
                    templateDocumentCategoryField.AllowMultipleValues = false;
                    templateDocumentCategoryField.CreateValuesInEditForm = false;
                    templateDocumentCategoryField.Open = true;
                    templateDocumentCategoryField.Group = TemplateDocuments.ListName;
                    templateDocumentCategoryField.Title = TemplateDocuments.TemplateDocumentCategoryDisplayName;
                    TaxonomyUtility.ConnectTaxonomyField(site, templateDocumentCategoryField.Id, TermStoreName.TermGroup, TemplateDocuments.TermSetTemplateDocumentCategory);
                    SPFieldLink templateDocumentCategoryFieldLink = new SPFieldLink(templateDocumentCategoryField);

                    templateDocumentsContentType = new SPContentType(TemplateDocuments.templateDocumentContentTypeId,
                                                                   rootWeb.ContentTypes,
                                                                   TemplateDocuments.ListContentType);
                    templateDocumentsContentType.FieldLinks.Add(templateDocumentCategoryFieldLink);
                    templateDocumentsContentType.Group = TemplateDocuments.AtkinsContentTypeGroup;
                    rootWeb.ContentTypes.Add(templateDocumentsContentType);
                    rootWeb.Update();


                }
                if (templateDocumentsContentType != null &&
                !CustomListHelper.ListContainsContentType(templateDocumentsList, TemplateDocuments.templateDocumentContentTypeId))
                {
                    templateDocumentsList.ContentTypesEnabled = true;
                    templateDocumentsList.ContentTypes.Add(templateDocumentsContentType);
                    templateDocumentsList.ContentTypes[0].Delete();
                    templateDocumentsList.Update();

                    SPView defaultView = templateDocumentsList.DefaultView;
                    defaultView.ViewFields.Delete(CustomListHelper.ReturnListField(templateDocumentsList, "Modified"));
                    
                    defaultView.ViewFields.Delete(CustomListHelper.ReturnListField(templateDocumentsList, "Editor"));
                    defaultView.ViewFields.Add(CustomListHelper.ReturnListField(templateDocumentsList, TemplateDocuments.TemplateDocumentCategory));
                    defaultView.ViewFields.Add("Modified");

                    SPField docId = CustomListHelper.ReturnListField(templateDocumentsList, "_dlc_DocIdUrl");
                    if (docId != null)
                        defaultView.ViewFields.Add(docId);


                    defaultView.Update();


                    //WebPartView
                    //System.Collections.Specialized.StringCollection viewFields = new System.Collections.Specialized.StringCollection();
                    //viewFields.Add("LinkTitle");
                    //SPView webPartView = templateDocumentsList.Views.Add(TemplateDocuments.webPartView, viewFields, "", 5, false, false);
                    //webPartView.TabularView = false;
                    //webPartView.Update();


                    currentWeb.Update();
                    //ADD METADATA NAVIGATION TO LIST
                    MetadataNavigationSettings listNavSettings = MetadataNavigationSettings.GetMetadataNavigationSettings(templateDocumentsList);
                    MetadataNavigationHierarchy navigationTemplateDocumentCategory = new MetadataNavigationHierarchy(CustomListHelper.ReturnListField(templateDocumentsList, TemplateDocuments.TemplateDocumentCategory));
                    listNavSettings.AddConfiguredHierarchy(navigationTemplateDocumentCategory);
                    MetadataNavigationSettings.SetMetadataNavigationSettings(templateDocumentsList, listNavSettings, true);
                }
            }
            

        }



        private static void CreateOfficeContentTypeList(SPWeb currentWeb)
        {
            Guid officeListGuid = currentWeb.Lists.Add(CustomListHelper.ReturnTrimmedString(OfficeFields.ListName), OfficeFields.ListDescription,
                                                       SPListTemplateType.GenericList);
            
            SPList officeList = currentWeb.Lists[officeListGuid];
            officeList.Title = OfficeFields.ListName;
            officeList.Update();
            currentWeb.Update();

            //Create Office Content Type
            SPContentType officeListContentType = null;
            if (CustomListHelper.SiteContainsContentType(currentWeb, OfficeFields.OfficeContentTypeId))
            {
                officeListContentType = currentWeb.ContentTypes[OfficeFields.OfficeContentTypeId];
            }
            else
            {
                //Address Field
                string fieldInternalName = CustomListHelper.CreateSiteColumn(currentWeb, OfficeFields.Address, SPFieldType.Note, false);
                SPFieldMultiLineText addressField = (SPFieldMultiLineText)currentWeb.Fields.GetField(fieldInternalName);
                addressField.Group = OfficeFields.ListName;
                addressField.Title = OfficeFields.AddressDisplayName;
                addressField.Update();
                SPFieldLink addressLink = new SPFieldLink(addressField);

                //Zip Field
                fieldInternalName = CustomListHelper.CreateSiteColumn(currentWeb, OfficeFields.Zip, SPFieldType.Text, false);
                SPFieldText zipField = (SPFieldText)currentWeb.Fields.GetField(fieldInternalName);
                zipField.Group = OfficeFields.ListName;
                zipField.Group = OfficeFields.ZipDisplayName;
                zipField.Update();
                SPFieldLink zipLink = new SPFieldLink(zipField);

                //City Field
                fieldInternalName = CustomListHelper.CreateSiteColumn(currentWeb, OfficeFields.City, SPFieldType.Text, false);
                SPFieldText cityField = (SPFieldText)currentWeb.Fields.GetField(fieldInternalName);
                cityField.Group = OfficeFields.ListName;
                cityField.Title = OfficeFields.CityDisplayName;
                cityField.Update();
                SPFieldLink cityLink = new SPFieldLink(cityField);

                //Phone number Field
                fieldInternalName = CustomListHelper.CreateSiteColumn(currentWeb, OfficeFields.PhoneNumber, SPFieldType.Text, false);
                SPFieldText phoneField = (SPFieldText)currentWeb.Fields.GetField(fieldInternalName);
                phoneField.Title = OfficeFields.PhoneNumberDisplayName;
                phoneField.Group = OfficeFields.ListName;
                phoneField.Update();
                SPFieldLink phoneLink = new SPFieldLink(phoneField);


                //Fax number Field
                fieldInternalName = CustomListHelper.CreateSiteColumn(currentWeb, OfficeFields.FaxNumber, SPFieldType.Text, false);
                SPFieldText faxField = (SPFieldText)currentWeb.Fields.GetField(fieldInternalName);
                faxField.Title = OfficeFields.FaxNumberDisplayName;
                faxField.Group = OfficeFields.ListName;
                faxField.Update();
                SPFieldLink faxLink = new SPFieldLink(faxField);

                officeListContentType = new SPContentType(OfficeFields.OfficeContentTypeId,
                                                          currentWeb.ContentTypes,
                                                          OfficeFields.ListContentType);
                officeListContentType.FieldLinks.Add(addressLink);
                officeListContentType.FieldLinks.Add(zipLink);
                officeListContentType.FieldLinks.Add(cityLink);
                officeListContentType.FieldLinks.Add(phoneLink);
                officeListContentType.FieldLinks.Add(faxLink);
                officeListContentType.Group = OfficeFields.AtkinsContentTypeGroup;

                currentWeb.ContentTypes.Add(officeListContentType);
                currentWeb.Update();
            }

            //Add Office Content type to Office Information list
            if (officeListContentType != null &&
                !CustomListHelper.ListContainsContentType(officeList, OfficeFields.OfficeContentTypeId))
            {
                officeList.ContentTypesEnabled = true;
                officeList.ContentTypes.Add(officeListContentType);
                officeList.ContentTypes[0].Delete();
                officeList.Update();

                SPField titleField = officeList.Fields[SPBuiltInFieldId.Title];
                titleField.Title = OfficeFields.TitleDisplayName;
                titleField.Update();


                SPView defaultView = officeList.DefaultView;
                defaultView.ViewFields.Add(CustomListHelper.ReturnTrimmedString(OfficeFields.Address));
                defaultView.ViewFields.Add(CustomListHelper.ReturnTrimmedString(OfficeFields.City));
                defaultView.ViewFields.Add(CustomListHelper.ReturnTrimmedString(CustomListHelper.ReturnTrimmedString(OfficeFields.FaxNumber)));
                defaultView.ViewFields.Add(CustomListHelper.ReturnTrimmedString(CustomListHelper.ReturnTrimmedString(OfficeFields.PhoneNumber)));
                defaultView.ViewFields.Add(CustomListHelper.ReturnTrimmedString(OfficeFields.Zip));
                defaultView.Update();

                currentWeb.Update();
            }
        }

      
    }
}
