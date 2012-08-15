using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Atkins.Intranet.Utilities.HelperUtils;

namespace Atkins.Intranet.Blog.Features.Atkins.Intranet.Blog.Setup
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("7edb4e70-1095-4c84-b74c-3efb60a5125c")]
    public class AtkinsIntranetBlogEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPWeb web = (SPWeb)properties.Feature.Parent;
            //publishing feature
            if (!FeatureUtility.IsFeatureActivated(web, new Guid("22A9EF51-737B-4ff2-9346-694633FE4416")))
            {
                web.Features.Add(new Guid("22A9EF51-737B-4ff2-9346-694633FE4416"));
            }
            

            web.Navigation.UseShared = true;
            web.SiteLogoUrl = "/_layouts/images/Atkins.Intranet.Portal/AtkinsLogo.png";
            web.MasterUrl = web.Site.RootWeb.ServerRelativeUrl + "/_catalogs/masterpage/AtkinsSystemMasterPage.master";
            web.CustomMasterUrl = web.Site.RootWeb.ServerRelativeUrl + "/_catalogs/masterpage/AtkinsPortalMasterPage.master";
            //web.AlternateCssUrl = "/_layouts/Atkins.Intranet.Portal/CSS/Blog.css";
            SPList categories = web.Lists.TryGetList("Kategorier");
            if (categories != null)
            {
                for (int i = categories.Items.Count - 1; i >= 0; i--)
                {
                    categories.Items.Delete(i);
                }

            }
            //ADD Content editorwebpart to hide title in titlearea for the blog site
            WebPartUtility.AddContentEditorWebPart(web, HideTitleBlog.webPartTitle,HideTitleBlog.ZoneId, 1, HideTitleBlog.Content);
            web.Update();
            web.Dispose();
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
