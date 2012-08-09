using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Atkins.Intranet.Utilities.HelperUtils;

namespace Atkins.Intranet.Features.Atkins.Intranet.Portal.Settings
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("de295112-5b16-4ccd-9d1a-ad36f6d16d99")]
    public class AtkinsIntranetPortalEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPWeb web = (SPWeb)properties.Feature.Parent;

            if (!FeatureUtility.IsFeatureActivated(web.Site, new Guid("b50e3104-6812-424f-a011-cc90e6327318")))
            {
                web.Site.Features.Add(new Guid("b50e3104-6812-424f-a011-cc90e6327318"));
                web.Properties["docid_settings_ui"] = "<?xml version=\"1.0\" encoding=\"utf-16\"?><DocIdUiSettings xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Prefix>ATKINS</Prefix><AssignmentEnabled>true</AssignmentEnabled></DocIdUiSettings>";
                web.AllProperties["docid_settings_ui"] = "<?xml version=\"1.0\" encoding=\"utf-16\"?><DocIdUiSettings xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Prefix>ATKINS</Prefix><AssignmentEnabled>true</AssignmentEnabled></DocIdUiSettings>";
                web.Update();
            }
            //CREATE PERMISSION ROLE
            SecurityUtility.CreateDeviationCustomRoleDefinition(web,DeviationsList.DeviationRoleDefinition);
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
