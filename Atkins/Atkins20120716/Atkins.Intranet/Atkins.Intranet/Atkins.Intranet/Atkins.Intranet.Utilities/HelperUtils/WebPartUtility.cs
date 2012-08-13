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
                if (checkIfViewExist(currentList, viewName))
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

        public static void AddCQWP(SPWeb currentWeb,string title, string zoneId, int zoneIndex, string xslPath, string itemstyle, string viewFields)
        {
            string startPage = currentWeb.RootFolder.WelcomePage;
            string fullUrlOfStartPage = currentWeb.Url + "/" + startPage;
            SPFile startPageFile = currentWeb.GetFile(fullUrlOfStartPage);

            
            if (startPageFile.Level != SPFileLevel.Checkout)
                startPageFile.CheckOut();
            SPLimitedWebPartManager manager = currentWeb.GetLimitedWebPartManager(startPage, PersonalizationScope.Shared);
            ContentByQueryWebPart contentByQery = new ContentByQueryWebPart();
            contentByQery.ItemXslLink = xslPath;
            //contentByQery.WebUrl = currentWeb.Site.ServerRelativeUrl;
            contentByQery.Title = title;
            contentByQery.BaseType = "104";
            
            //contentByQery.ContentTypeName = "Meddelande";
            //contentByQery.ListName = currentList.Title;
            //contentByQery.ListGuid = currentList.ID.ToString("B");
            contentByQery.ItemStyle = "Announcements";
            contentByQery.ItemLimit = 10;
            contentByQery.AdditionalFilterFields = "Title";
            
            if (!FindWebPart(manager, title))
                manager.AddWebPart(contentByQery, zoneId, zoneIndex);
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
                    // Here perform the webpart check 
                    // For instance you could identify the web part by 
                    // its class name 

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

        public static string CreateView(SPWeb currentWeb, string listName, string viewName,string[] viewFields,string query,uint rowlimit)
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

        private static bool checkIfViewExist(SPList currentList,string name)
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
    }
}
