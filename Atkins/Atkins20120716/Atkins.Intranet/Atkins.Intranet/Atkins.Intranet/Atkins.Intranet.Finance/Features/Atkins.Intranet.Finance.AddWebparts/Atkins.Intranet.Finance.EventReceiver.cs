using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Atkins.Intranet.Utilities.HelperUtils;
using System.Web;
using System.IO;


namespace Atkins.Intranet.Finance.Features.Atkins.Intranet.Finance.AddWebparts
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("2d777e25-4321-4430-a5c0-f8decd4289d8")]
    public class AtkinsIntranetFinanceEventReceiver : SPFeatureReceiver
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
            //BLOG WEBPART FILTER Finance CATEGORY
            using (SPWeb sourceWeb = web.Site.AllWebs[BlogPosts.webName])
            {
                WebPartUtility.AddBlogWebpart(web, sourceWeb, BlogPosts.ListName, BlogPosts.webPartTitle, BlogPosts.ZoneId, 3, BlogPosts.xslPath, BlogPosts.webpartItemStyle, BlogPosts.webPartViewFields, BlogPosts.webpartTitleImageUrl, BlogPosts.categoryFilterFinance);
            }
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