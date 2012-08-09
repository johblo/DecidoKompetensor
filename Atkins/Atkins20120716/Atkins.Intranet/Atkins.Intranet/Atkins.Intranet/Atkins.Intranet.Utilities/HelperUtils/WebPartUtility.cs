using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint.WebPartPages;

namespace Atkins.Intranet.Utilities.HelperUtils
{
    public class WebPartUtility
    {
        public static void AddListViewWebPart(SPWeb currentWeb,SPWeb sourceWeb,string listName,string viewName,string zoneId,int zoneIndex )
        {
            string startPage = currentWeb.RootFolder.WelcomePage;
            string fullUrlOfStartPage = currentWeb.Url+"/"+startPage;
            SPFile startPageFile = currentWeb.GetFile(fullUrlOfStartPage);
            
            SPList currentList = sourceWeb.Lists.TryGetList(listName);
            if (currentList != null)
            {
                startPageFile.CheckOut();
                SPLimitedWebPartManager manager = currentWeb.GetLimitedWebPartManager(startPage, PersonalizationScope.Shared);
                ListViewWebPart webPart = new ListViewWebPart();
                webPart.WebId = sourceWeb.ID;
                webPart.ListName = currentList.ID.ToString();
                if (checkIfViewExist(currentList, viewName))
                {
                    SPView webPartView = currentList.Views[viewName];
                    webPart.ViewGuid = webPartView.ID.ToString("B").ToUpper();
                    webPart.ViewType = ViewType.Html;
                    manager.AddWebPart(webPart, zoneId, zoneIndex);
                }
                startPageFile.CheckIn("Added webpart");
            }
            currentWeb.Update();
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
