using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Taxonomy;
using Atkins.Intranet.Utilities.HelperUtils;

namespace Atkins.Intranet.Features.Atkins.Intranet.Portal.Taxonomy
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("64e3889c-76c3-4189-ba89-0893f04dbf45")]
    public class AtkinsIntranetPortalEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPSite currentSite = (SPSite)properties.Feature.Parent;
            //MANUAL DOCUMENTS - CATEGORY 
            TaxonomyUtility.CreateTermSet(currentSite, ManualsDocuments.TermSetCategory);
            //DEVIATION - STATUS
            TaxonomyUtility.CreateTermSet(currentSite, DeviationsList.TermSetStatus);
            //EMPLOYEE HANDBOOK - CATEGORY
            TaxonomyUtility.CreateTermSet(currentSite, EmployeeHandbook.TermSet);
            //TEMPLATE DOCUMENT - CATEGORY
            TaxonomyUtility.CreateTermSet(currentSite, TemplateDocuments.TermSetTemplateDocumentCategory);
            //PROCESS LIST - PROCESS
            TaxonomyUtility.CreateTermSet(currentSite, ProcessStepList.TermSetProcess);
            //RESULTING DOCUMENTS - CATEGORY
            TaxonomyUtility.CreateTermSet(currentSite, ResultingDocuments.TermSetResultingDocumentCategory);
            //CONTROLLING DOCUMENTS - ISO9001
            TaxonomyUtility.CreateTermSet(currentSite, ControllingDocuments.TermSetISO9001);
            //CONTROLLING DOCUMENTS - ISO14001
            TaxonomyUtility.CreateTermSet(currentSite, ControllingDocuments.TermSetISO14001);
            //CONTROLLING DOCUMENTS - ISO18001
            TaxonomyUtility.CreateTermSet(currentSite, ControllingDocuments.TermSetISO18001);
            //CONTROLLING DOCUMENTS - CHAPTER
            TaxonomyUtility.CreateTermSet(currentSite, ControllingDocuments.TermSetChapter);
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
