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
using Microsoft.SharePoint.Portal.WebControls;


namespace Atkins.Intranet.Utilities.HelperUtils
{
    public class WebPartUtility
    {
        public static void AddPageViewWebPart(SPWeb currentWeb, string title, string zoneId, int zoneIndex, string titleImageUrl,string contentLink,PathPattern sourceType)
        {
            string startPage = currentWeb.RootFolder.WelcomePage;
            string fullUrlOfStartPage = currentWeb.Url + "/" + startPage;
            SPFile startPageFile = currentWeb.GetFile(fullUrlOfStartPage);
            
            if (startPageFile.Level != SPFileLevel.Checkout)
                startPageFile.CheckOut();
            SPLimitedWebPartManager manager = currentWeb.GetLimitedWebPartManager(startPage, PersonalizationScope.Shared);
            //ListViewWebPart webPart = new ListViewWebPart();
            PageViewerWebPart webPart = new PageViewerWebPart();
            webPart.Title = title;
            webPart.ContentLink = contentLink;
            webPart.SourceType = sourceType;
            if (!string.IsNullOrEmpty(titleImageUrl))
                webPart.TitleIconImageUrl = titleImageUrl;
            if (!FindWebPart(manager, title))
                manager.AddWebPart(webPart, zoneId, zoneIndex);
            if (startPageFile.Level == SPFileLevel.Checkout)
                startPageFile.CheckIn("Added webpart");
            
            currentWeb.Update();
        }
        /*
        public static void AddRSSViewWebPart(SPWeb currentWeb, string title, string zoneId, int zoneIndex, string titleImageUrl)
        {
            string startPage = currentWeb.RootFolder.WelcomePage;
            string fullUrlOfStartPage = currentWeb.Url + "/" + startPage;
            SPFile startPageFile = currentWeb.GetFile(fullUrlOfStartPage);

            if (startPageFile.Level != SPFileLevel.Checkout)
                startPageFile.CheckOut();
            SPLimitedWebPartManager manager = currentWeb.GetLimitedWebPartManager(startPage, PersonalizationScope.Shared);
            RSSAggregatorWebPart webPart = new RSSAggregatorWebPart();
            
            webPart.Title = title;
            webPart.FeedUrl = "http://www.infrastrukturnyheter.se/rss.xml";
            webPart.FeedLimit = 10;
            
            
            if (!string.IsNullOrEmpty(titleImageUrl))
                webPart.TitleIconImageUrl = titleImageUrl;
            if (!FindWebPart(manager, title))
                manager.AddWebPart(webPart, zoneId, zoneIndex);
            if (startPageFile.Level == SPFileLevel.Checkout)
                startPageFile.CheckIn("Added webpart");

            currentWeb.Update();
        }
        */
   




        public static void AddListViewWebPart(SPWeb currentWeb,SPWeb sourceWeb,string listName,string title,string viewName,string zoneId,int zoneIndex,string titleImageUrl)
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
                if(!string.IsNullOrEmpty(titleImageUrl))
                    webPart.TitleIconImageUrl = titleImageUrl;
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

        public static void AddXSLTListViewWebPart(SPWeb currentWeb, SPWeb sourceWeb, string listName, string title, string viewName, string zoneId, int zoneIndex, string titleImageUrl)
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

                XsltListViewWebPart webPart = new XsltListViewWebPart();
                webPart.Title = title;
                webPart.WebId = sourceWeb.ID;
                if (!string.IsNullOrEmpty(titleImageUrl))
                    webPart.TitleIconImageUrl = titleImageUrl;
                webPart.ListName = currentList.ID.ToString();
                
                if (CustomListHelper.checkIfViewExist(currentList, viewName))
                {
                    SPView webPartView = currentList.Views[viewName];
                    webPart.ViewGuid = webPartView.ID.ToString("B").ToUpper();
                    if (!FindWebPart(manager, title))
                    {
                        manager.AddWebPart(webPart, zoneId, zoneIndex);
                       
                    }

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
        public static void AddCQWP(SPWeb currentWeb, SPWeb sourceWeb, string listName, string title, string zoneId, int zoneIndex,string xslPath,string itemstyle,string viewFields,string titleImageUrl)
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
                if (!string.IsNullOrEmpty(titleImageUrl))
                    contentByQery.TitleIconImageUrl = titleImageUrl;
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

        public static void AddAnnouncementCQWP(SPWeb currentWeb, string title, string zoneId, int zoneIndex, string xslPath, string itemstyle, string viewFields, string titleImageUrl)
        {
            string startPage = currentWeb.RootFolder.WelcomePage;
            string fullUrlOfStartPage = currentWeb.Url + "/" + startPage;
            SPFile startPageFile = currentWeb.GetFile(fullUrlOfStartPage);
            if (startPageFile.Level != SPFileLevel.Checkout)
                startPageFile.CheckOut();
            SPLimitedWebPartManager manager = currentWeb.GetLimitedWebPartManager(startPage, PersonalizationScope.Shared);
            ContentByQueryWebPart contentByQery = new ContentByQueryWebPart();
            contentByQery.UseCopyUtil = true;
            if (!string.IsNullOrEmpty(titleImageUrl))
                contentByQery.TitleIconImageUrl = titleImageUrl;
            //contentByQery.ViewFieldsOverride = "<![CDATA[<FieldRef ID='{fa564e0f-0c70-4ab9-b863-0177e6ddd247}' Nullable='True' Type='Text' /><FieldRef ID='{94f89715-e097-4e8b-ba79-ea02aa8b7adb}' Nullable='True' Type='Lookup' /><FieldRef ID='{1d22ea11-1e32-424e-89ab-9fedbadb6ce1}' Nullable='True' Type='Counter' /><FieldRef ID='{28cf69c5-fa48-462a-b5cd-27b6f9d2bd5f}' Nullable='True' Type='DateTime' /><FieldRef ID='{1df5e554-ec7e-46a6-901d-d85a3881cb18}' Nullable='True' Type='User' /><FieldRef ID='{d31655d1-1d5b-4511-95a1-7a09e9b75bf2}' Nullable='True' Type='User' /><FieldRef ID='{8c06beca-0777-48f7-91c7-6da68bc07b69}' Nullable='True' Type='DateTime' /><FieldRef Name='PublishingRollupImage' Nullable='True' Type='Image' /><FieldRef Name='_Level' Nullable='True' Type='Number' /><FieldRef Name='Comments' Nullable='True' Type='Note' /><ListProperty Name='Title' /><ProjectProperty Name='Title' />]]>";
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
        public static void AddLastCreatedDocuments(SPWeb currentWeb, string title, string zoneId, int zoneIndex, string xslPath, string itemstyle, string titleImageUrl)
        {
            string startPage = currentWeb.RootFolder.WelcomePage;
            string fullUrlOfStartPage = currentWeb.Url + "/" + startPage;
            SPFile startPageFile = currentWeb.GetFile(fullUrlOfStartPage);
            if (startPageFile.Level != SPFileLevel.Checkout)
                startPageFile.CheckOut();
            SPLimitedWebPartManager manager = currentWeb.GetLimitedWebPartManager(startPage,  PersonalizationScope.Shared);
            ContentByQueryWebPart contentByQery = new ContentByQueryWebPart();
            contentByQery.UseCopyUtil = true;
            if (!string.IsNullOrEmpty(titleImageUrl))
                contentByQery.TitleIconImageUrl = titleImageUrl;
            contentByQery.ItemStyle = "TitleOnly";
            //contentByQery.ViewFieldsOverride = "<ViewFields>"+
            //                                        "<FieldRef Name='Title' Nullable='True' Type='Text'/>" +
            //                                        "<FieldRef Name='FileLeadRef' Nullable='True' Type='URL'/>" +
            //                                        "<ListProperty Name='Title' />"+
            //                                        "<ProjectProperty Name='Title' />"+
            //                                   "</ViewFields>";
            //<ListProperty Name="Title" /><ProjectProperty Name="Title" />
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
                //manager.SaveChanges(contentByQery);
            }
            if (startPageFile.Level == SPFileLevel.Checkout)
                startPageFile.CheckIn("Added webpart");
            currentWeb.Update();
        }

        public static void AddRelevantDocuments(SPWeb currentWeb, string title, string zoneId, int zoneIndex, string titleImageUrl)
        {
            string startPage = currentWeb.RootFolder.WelcomePage;
            string fullUrlOfStartPage = currentWeb.Url + "/" + startPage;
            SPFile startPageFile = currentWeb.GetFile(fullUrlOfStartPage);
            if (startPageFile.Level != SPFileLevel.Checkout)
                startPageFile.CheckOut();
            SPLimitedWebPartManager manager = currentWeb.GetLimitedWebPartManager(startPage, PersonalizationScope.Shared);
            UserDocsWebPart relevantDocuments = new UserDocsWebPart();
            if (!string.IsNullOrEmpty(titleImageUrl))
                relevantDocuments.TitleIconImageUrl = titleImageUrl;
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
        public static void AddBlogWebpart(SPWeb currentWeb, SPWeb sourceWeb, string listName, string title, string zoneId, int zoneIndex, string xslPath, string itemstyle, string viewFields, string titleImageUrl,string filter)
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
                if (!string.IsNullOrEmpty(titleImageUrl))
                    contentByQery.TitleIconImageUrl = titleImageUrl;

                contentByQery.FilterField1 = "PostCategory";
                contentByQery.FilterOperator1 = ContentByQueryWebPart.FilterFieldQueryOperator.Eq;
                contentByQery.FilterType1 = "Lookup";
                //contentByQery.Filter1IsCustomValue = true;
                //contentByQery.FilterDisplayValue1 = "-2";
                contentByQery.FilterValue1 = filter;

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
        //Method to Set the Tool bar type      
        public static void SetToolbarType(SPView spView, String toolBarType)      
        {          
            spView.GetType().InvokeMember("EnsureFullBlownXmlDocument", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.InvokeMethod, null, spView, null, System.Globalization.CultureInfo.CurrentCulture);   
            System.Reflection.PropertyInfo nodeProp = spView.GetType().GetProperty("Node", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);       
            XmlNode node = nodeProp.GetValue(spView, null) as XmlNode;   
            XmlNode toolbarNode = node.SelectSingleNode("Toolbar");       
            if (toolbarNode != null)    
            {         
                toolbarNode.Attributes["Type"].Value = toolBarType;      
                // If the toolbartype is Freeform (i.e. Summary Toolbar) then we need to manually        
                // add some CAML to get it to work.       
                if (String.Compare(toolBarType, "Freeform", true, System.Globalization.CultureInfo.InvariantCulture) == 0)          
                {          
                    string newItemString = "";    
                    XmlAttribute positionNode = toolbarNode.OwnerDocument.CreateAttribute("Position");          
                    positionNode.Value = "After";               
                    toolbarNode.Attributes.Append(positionNode);                
                    switch (spView.ParentList.BaseTemplate)            
                    {                   
                        case SPListTemplateType.Announcements:     
                            newItemString = "announcement";                
                            break;                     
                        case SPListTemplateType.Events:                 
                            newItemString = "event";                    
                            break;                   
                        case SPListTemplateType.Tasks:        
                            newItemString = "task";              
                            break;                  
                        case SPListTemplateType.DiscussionBoard:    
                            newItemString = "discussion";                  
                            break;                
                        case SPListTemplateType.Links:             
                            newItemString = "link";               
                            break;            
                        case SPListTemplateType.GenericList:       
                            newItemString = "item";                  
                            break;             
                        case SPListTemplateType.DocumentLibrary:    
                            newItemString = "document";              
                            break;              
                        default:               
                            newItemString = "item";        
                            break;             
                    }                
                    if (spView.ParentList.BaseType == SPBaseType.DocumentLibrary)  
                    {                    
                        newItemString = "document";         
                    }                  
                    // Add the CAML    
                    toolbarNode.InnerXml = @"<IfHasRights><RightsChoices><RightsGroup PermAddListItems=""required"" /></RightsChoices><Then><HTML><![CDATA[ <table width=100% cellpadding=0 cellspacing=0 border=0 > <tr> <td colspan=""2"" class=""ms-partline""><IMG   src=""/_layouts/images/blank.gif"" width=1 height=1 alt=""""></td> </tr> <tr> <td class=""ms-addnew"" style=""padding-bottom: 3px""> <img src=""/_layouts/images/rect.gif"" alt="""">&nbsp;<a class=""ms-addnew"" ID=""idAddNewItem"" href=""]]></HTML><URL Cmd=""New"" /><HTML><![CDATA["" ONCLICK=""BLOCKED SCRIPTNewItem(']]></HTML><URL Cmd=""New"" /><HTML><![CDATA[', true);BLOCKED SCRIPTreturn false;"" target=""_self"">]]></HTML><HTML>Add new " + newItemString + @"</HTML><HTML><![CDATA[</a> </td> </tr> <tr><td><IMG src=""/_layouts/images/blank.gif"" width=1 height=5 alt=""""></td></tr> </table>]]></HTML></Then></IfHasRights>";           
                }               spView.Update();       
            }     
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
