using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint.WebPartPages;
using System.Collections.Specialized;
using System.Xml;
using Microsoft.SharePoint.Publishing.WebControls;


namespace Atkins.Intranet.Utilities.HelperUtils
{
    public class WebPartUtility
    {
        public static void AddListViewWebPart(SPWeb currentWeb,SPWeb sourceWeb,string listName,string title,string viewName,string zoneId,int zoneIndex )
        {
            string startPage = currentWeb.RootFolder.WelcomePage;
            string fullUrlOfStartPage = currentWeb.Url+"/"+startPage;
            SPFile startPageFile = currentWeb.GetFile(fullUrlOfStartPage);
            
            SPList currentList = sourceWeb.Lists.TryGetList(listName);
            if (currentList != null)
            {
                if(startPageFile.Level != SPFileLevel.Checkout)
                    startPageFile.CheckOut();
                SPLimitedWebPartManager manager = currentWeb.GetLimitedWebPartManager(startPage, PersonalizationScope.Shared);
                ListViewWebPart webPart = new ListViewWebPart();
                webPart.Title = title;
                webPart.WebId = sourceWeb.ID;
                webPart.ListName = currentList.ID.ToString();
                if (CustomListHelper.checkIfViewExist(currentList, viewName))
                {
                    SPView webPartView = currentList.Views[viewName];
                    webPart.ViewGuid = webPartView.ID.ToString("B").ToUpper();
                    webPart.ViewType = ViewType.Html;
                    if (!FindWebPart(manager, title))
                        manager.AddWebPart(webPart, zoneId, zoneIndex);
                    
                }
                if (startPageFile.Level == SPFileLevel.Checkout)
                    startPageFile.CheckIn("Added webpart");
            }
            currentWeb.Update();
        }
        public static void AddContentEditorWebPart(SPWeb currentWeb, string title, string zoneId, int zoneIndex,string content)
        {
            string startPage = "default.aspx";
            
            SPLimitedWebPartManager manager = currentWeb.GetLimitedWebPartManager(startPage, PersonalizationScope.Shared);
            ContentEditorWebPart contentEditorWebpart = new ContentEditorWebPart();
            contentEditorWebpart.ZoneID = zoneId;
            contentEditorWebpart.Title = title;
            contentEditorWebpart.ChromeState = System.Web.UI.WebControls.WebParts.PartChromeState.Normal;
            contentEditorWebpart.ChromeType = System.Web.UI.WebControls.WebParts.PartChromeType.None;

            //Add content to CEWP
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement xmlElement = xmlDoc.CreateElement("Root");
            xmlElement.InnerText = content;
            contentEditorWebpart.Content = xmlElement;
            contentEditorWebpart.Content.InnerText = xmlElement.InnerText;

            //Add it to the zone
            if (!FindWebPart(manager, title))
                manager.AddWebPart(contentEditorWebpart, zoneId, zoneIndex);
        }
        public static void AddCQWP(SPWeb currentWeb, SPWeb sourceWeb, string listName, string title, string zoneId, int zoneIndex,string xslPath,string itemstyle,string viewFields)
        {
            string startPage = currentWeb.RootFolder.WelcomePage;
            string fullUrlOfStartPage = currentWeb.Url + "/" + startPage;
            SPFile startPageFile = currentWeb.GetFile(fullUrlOfStartPage);
            SPList currentList = sourceWeb.Lists.TryGetList(listName);
            if (currentList != null)
            {
                if (startPageFile.Level != SPFileLevel.Checkout)
                    startPageFile.CheckOut();
                SPLimitedWebPartManager manager = currentWeb.GetLimitedWebPartManager(startPage, PersonalizationScope.Shared);
                ContentByQueryWebPart contentByQery = new ContentByQueryWebPart();
                contentByQery.UseCopyUtil = true;
                contentByQery.ItemXslLink = xslPath;
                contentByQery.Title = title;
                contentByQery.WebUrl = sourceWeb.Url;
                contentByQery.ListName = currentList.Title;
                contentByQery.ListGuid = currentList.ID.ToString("B");
                contentByQery.ItemStyle = itemstyle;
                contentByQery.ItemLimit = 10;
                contentByQery.CommonViewFields = viewFields;
                if (!FindWebPart(manager, title))
                    manager.AddWebPart(contentByQery, zoneId, zoneIndex);
                if (startPageFile.Level == SPFileLevel.Checkout)
                    startPageFile.CheckIn("Added webpart");
            }
            currentWeb.Update();
        }

        public static void AddAnnouncementCQWP(SPWeb currentWeb,string title, string zoneId, int zoneIndex, string xslPath, string itemstyle)
        {
            string startPage = currentWeb.RootFolder.WelcomePage;
            string fullUrlOfStartPage = currentWeb.Url + "/" + startPage;
            SPFile startPageFile = currentWeb.GetFile(fullUrlOfStartPage);
            if (startPageFile.Level != SPFileLevel.Checkout)
                startPageFile.CheckOut();
            SPLimitedWebPartManager manager = currentWeb.GetLimitedWebPartManager(startPage, PersonalizationScope.Shared);
            ContentByQueryWebPart contentByQery = new ContentByQueryWebPart();
            contentByQery.UseCopyUtil = true;
            contentByQery.ItemXslLink = xslPath;
            contentByQery.Title = title;
            contentByQery.BaseType = "104";
            contentByQery.ItemStyle = itemstyle; 
            contentByQery.ItemLimit = 10;
            contentByQery.SortBy = "Created";
            if (!FindWebPart(manager, title))
                manager.AddWebPart(contentByQery, zoneId, zoneIndex);
            if (startPageFile.Level == SPFileLevel.Checkout)
                startPageFile.CheckIn("Added webpart");
            currentWeb.Update();
        }
        //ADDS CQWB TO QSE DISPLAYING LAST EDITED OR CREATED DOCUMENTS
        public static void AddLastCreatedDocuments(SPWeb currentWeb, string title, string zoneId, int zoneIndex, string xslPath, string itemstyle)
        {
            string startPage = currentWeb.RootFolder.WelcomePage;
            string fullUrlOfStartPage = currentWeb.Url + "/" + startPage;
            SPFile startPageFile = currentWeb.GetFile(fullUrlOfStartPage);
            if (startPageFile.Level != SPFileLevel.Checkout)
                startPageFile.CheckOut();
            SPLimitedWebPartManager manager = currentWeb.GetLimitedWebPartManager(startPage,  PersonalizationScope.Shared);
            ContentByQueryWebPart contentByQery = new ContentByQueryWebPart();
            contentByQery.UseCopyUtil = true;

            contentByQery.ItemStyle = "TitleOnly";
            
            contentByQery.FilterField1 = "Created";
            contentByQery.FilterOperator1 = ContentByQueryWebPart.FilterFieldQueryOperator.Gt;
            contentByQery.FilterType1 = "DateTime";
            contentByQery.Filter1IsCustomValue = true;
            contentByQery.FilterDisplayValue1 = "-2";
            contentByQery.FilterValue1 = "-2";
            
            contentByQery.Filter1ChainingOperator = ContentByQueryWebPart.FilterChainingOperator.Or;
            contentByQery.FilterField2 = "Modified";
            contentByQery.FilterOperator2 = ContentByQueryWebPart.FilterFieldQueryOperator.Gt;
            contentByQery.FilterType2 = "DateTime";
            contentByQery.Filter2IsCustomValue = true;
            contentByQery.FilterDisplayValue2 = "-2";
            contentByQery.FilterValue2 = "-2";
            
            contentByQery.Title = title;
            contentByQery.BaseType = "101";
            contentByQery.WebUrl = currentWeb.Url;
            
            contentByQery.ItemLimit = 10;
            
            if (!FindWebPart(manager, title))
            {
                manager.AddWebPart(contentByQery, zoneId, zoneIndex);
                contentByQery.DataMappings = "LinkUrl:{c29e077d-f466-4d8e-8bbe-72b66c5f205c},URL,URL;|Title:{8553196d-ec8d-4564-9861-3dbe931050c8},FileLeadRef,Lookup;";
                contentByQery.DataMappingViewFields = "{c29e077d-f466-4d8e-8bbe-72b66c5f205c},URL;|{8553196d-ec8d-4564-9861-3dbe931050c8},Text;";
                manager.SaveChanges(contentByQery);
            }
            if (startPageFile.Level == SPFileLevel.Checkout)
                startPageFile.CheckIn("Added webpart");
            currentWeb.Update();
        }

        public static void AddRelevantDocuments(SPWeb currentWeb, string title, string zoneId, int zoneIndex)
        {
            string startPage = currentWeb.RootFolder.WelcomePage;
            string fullUrlOfStartPage = currentWeb.Url + "/" + startPage;
            SPFile startPageFile = currentWeb.GetFile(fullUrlOfStartPage);
            if (startPageFile.Level != SPFileLevel.Checkout)
                startPageFile.CheckOut();
            SPLimitedWebPartManager manager = currentWeb.GetLimitedWebPartManager(startPage, PersonalizationScope.Shared);
            UserDocsWebPart relevantDocuments = new UserDocsWebPart();
            relevantDocuments.QueryCheckedOutBy = true;
            relevantDocuments.QueryCreatedBy = true;
            relevantDocuments.QueryLastModifiedBy = true;
            relevantDocuments.Title = title;
            
            if (!FindWebPart(manager, title))
                manager.AddWebPart(relevantDocuments, zoneId, zoneIndex);
            if (startPageFile.Level == SPFileLevel.Checkout)
                startPageFile.CheckIn("Added webpart");
            currentWeb.Update();
        }

       
        private static bool FindWebPart(SPLimitedWebPartManager manager,string title)
        {
            try
            {
                SPLimitedWebPartCollection webparts = manager.WebParts;
                foreach (System.Web.UI.WebControls.WebParts.WebPart wp in webparts)
                {
                    if (wp.Title == title)
                        return true;
                }
                return false;
            }
            catch (Exception ex) 
            {
                return false;
            }
        }

        
    }
}
