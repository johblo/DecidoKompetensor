using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using Atkins.Intranet.Utilities.HelperUtils;
using System.Web;
using System.IO;

namespace Atkins.Intranet.Features.Atkins.Intranet.Portal.AddWebParts
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("15e44b59-4a8c-400a-807b-2f3d3a01bf33")]
    public class AtkinsIntranetPortalEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPWeb web = (SPWeb)properties.Feature.Parent;
            
            //FIX That enables us to provide cqwp through PowerShell Clears the error that otherwise occures: 
            bool contextCreated = false;
            if (HttpContext.Current == null)
            {
                HttpRequest request = new HttpRequest("", web.Url, "");
                HttpContext.Current = new HttpContext(request,
                    new HttpResponse(new StringWriter()));
                HttpContext.Current.Items["HttpHandlerSPWeb"] = web;
                contextCreated = true;
            }
            //ADD BLOG WP TO START
            using (SPWeb sourceWeb = web.Site.AllWebs[BlogPosts.webName])
            {
                WebPartUtility.AddCQWP(web, sourceWeb, SPUtility.GetLocalizedString(BlogPosts.ListName, CommonSettings.resourceFile, CommonSettings.resourceLCID), BlogPosts.webPartTitle, BlogPosts.rowLimitStartPage, BlogPosts.ZoneId, 1, BlogPosts.xslPath, BlogPosts.webpartItemStyle, BlogPosts.webPartViewFields, BlogPosts.webpartTitleImageUrl);
            }
            //ADD ANNOUNCEMENT Webpart

            SPList announcementsList = web.Lists.TryGetList(SPUtility.GetLocalizedString(Announcements.ListName, CommonSettings.resourceFile, CommonSettings.resourceLCID));
            if (announcementsList != null)
            {
                if (!CustomListHelper.checkIfViewExist(announcementsList, Announcements.webPartView))
                {
                    CustomListHelper.CreateView(announcementsList, Announcements.webPartView, CustomListHelper.returnStringArray(Announcements.webPartViewFields), Announcements.webPartQuery, Announcements.webPartRowLimit);
                }
                WebPartUtility.AddXSLTListViewWebPart(web, web, SPUtility.GetLocalizedString(Announcements.ListName, CommonSettings.resourceFile, CommonSettings.resourceLCID), Announcements.webPartTitle, Announcements.webPartView, Announcements.ZoneId, 1, Announcements.webpartTitleImageUrl);
            }
            
            
            
            //ADD Calendar Webpart
            WebPartUtility.AddXSLTListViewWebPart(web, web, CalendarStartSite.ListName, CalendarStartSite.webPartTitle, CalendarStartSite.webPartView, CalendarStartSite.ZoneId, 1, CalendarStartSite.webpartTitleImageUrl);

            //ADDS PAGEVIEWER WEBPART 
            WebPartUtility.AddPageViewWebPart(web, KpiStock.webPartTitle, KpiStock.ZoneId, 1, KpiStock.webpartTitleImageUrl, KpiStock.contentLink, Microsoft.SharePoint.WebPartPages.PathPattern.URL);
            
            //ADD LINKS
            WebPartUtility.AddXSLTListViewWebPart(web, web, LinksStartSite.ListName, LinksStartSite.webPartTitle, LinksStartSite.webPartView, LinksStartSite.ZoneId, 1, LinksStartSite.webpartTitleImageUrl);
            
            
            

            if (contextCreated)
            {
                HttpContext.Current = null;
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
