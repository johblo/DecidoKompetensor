using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Atkins.Intranet.Utilities.HelperUtils;
using Microsoft.SharePoint.Utilities;
using System.Web;
using System.IO;

namespace Atkins.Intranet.QSE.Features.Atkins.Intranet.QSE.AddWebparts
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("387ac255-e2c7-43a4-963d-7e8078ff1bf1")]
    public class AtkinsIntranetQSEEventReceiver : SPFeatureReceiver
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
            //QSE LINKS
            WebPartUtility.AddXSLTListViewWebPart(web, web, SPUtility.GetLocalizedString(QSELinks.ListName, QSELinks.resourceFile, QSELinks.resourceLCID), QSELinks.webPartTitle, QSELinks.webPartView, QSELinks.ZoneId, 1, QSELinks.webpartTitleImageUrl);
            //DEVIATION
            WebPartUtility.AddXSLTListViewWebPart(web, web, DeviationsList.ListName, DeviationsList.webPartTitle, DeviationsList.webPartView, DeviationsList.ZoneId, 2, DeviationsList.webpartTitleImageUrl);
            
            //BLOG WEBPART FILTER QSE CATEGORY
            using (SPWeb sourceWeb = web.Site.AllWebs[BlogPosts.webName])
            {
                WebPartUtility.AddBlogWebpart(web, sourceWeb, BlogPosts.ListName, BlogPosts.webPartTitle, BlogPosts.ZoneId, 1, BlogPosts.xslPath, BlogPosts.webpartItemStyle, BlogPosts.webPartViewFields, BlogPosts.webpartTitleImageUrl, BlogPosts.categoryFilterQSE);
            }
            //LAST ADDED DOCUMENTS
            WebPartUtility.AddLastCreatedDocuments(web, LastAddedModiefiedDocuments.webPartTitle, LastAddedModiefiedDocuments.ZoneId, 2, "", "", LastAddedModiefiedDocuments.webpartTitleImageUrl);
            //RELEVANT DOCUMENTS
            WebPartUtility.AddRelevantDocuments(web, RelevantDocuments.webPartTitle, RelevantDocuments.ZoneId, 3, RelevantDocuments.webpartTitleImageUrl);
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
