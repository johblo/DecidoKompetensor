using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Xml.Linq;
using Atkins.Intranet.SampleData.Features.RootWebLists;
using Atkins.Intranet.Utilities.HelperUtils;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;

namespace Atkins.Intranet.SampleData.Features.QSEWebLists
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("d5b2aa3f-4dd7-4dda-94e1-b07fe39655b4")]
    public class QSEWebListsEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            //DEVIATIONS
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                try
                {
                    SPWeb web = (SPWeb)properties.Feature.Parent;

                    SPList DeviationList = CustomListHelper.ReturnList(web, DeviationsList.ListName);
                    if (DeviationList != null)
                    {
                        string configFilePath = SPUtility.GetGenericSetupPath(@"template\layouts\Atkins.Intranet.SampleData\DeviationData.xml");
                        XDocument sampleDocument = XDocument.Load(configFilePath);
                        List<DeviationData> data = CollectDeviationData(sampleDocument);
                        GenerateDeviationItems(web, DeviationList, data);
                    }

                }
                catch (SPException exception)
                {
                    // throw exception;
                }
            });
        }
        private static List<DeviationData> CollectDeviationData(XDocument sampleDocument)
        {
            IEnumerable<XElement> nodes = sampleDocument.Descendants("Deviations").Elements("Deviation");
            
            List<DeviationData> events = nodes.Select(x => new DeviationData()
            {

                Title = x.Element(DeviationsList.Title).Value,
                KeyDate = x.Element(CustomListHelper.ReturnTrimmedString(DeviationsList.KeyDate)).Value,
                DeviationStatus = x.Element(CustomListHelper.ReturnTrimmedString(DeviationsList.DeviationStatus)).Value,
                DeviationDescription = x.Element(CustomListHelper.ReturnTrimmedString(DeviationsList.DeviationDescription)).Value,
                DeviationResponsible = x.Element(CustomListHelper.ReturnTrimmedString(DeviationsList.Responsible)).Value,
                DecisionDate = x.Element(CustomListHelper.ReturnTrimmedString(DeviationsList.DecisionDate)).Value,
                DecisionComment = x.Element(CustomListHelper.ReturnTrimmedString(DeviationsList.DecisionComment)).Value,
                ActionByDate = x.Element(CustomListHelper.ReturnTrimmedString(DeviationsList.ActionByDate)).Value,
                FollowUpDate = x.Element(CustomListHelper.ReturnTrimmedString(DeviationsList.FollowUpDate)).Value,
                FollowUpComment = x.Element(CustomListHelper.ReturnTrimmedString(DeviationsList.FollowUpComment)).Value
                 

            }).ToList();

            return events;
        }
        private static void GenerateDeviationItems(SPWeb web, SPList DeviationList, List<DeviationData> Deviations)
        {
            foreach (DeviationData DeviationData in Deviations)
            {
                SPListItem newItem = DeviationList.AddItem();
                newItem[SPBuiltInFieldId.Title] = DeviationData.Title;
                newItem[CustomListHelper.ReturnListField(DeviationList, DeviationsList.KeyDate).Id] = DeviationData.KeyDate;
                newItem[CustomListHelper.ReturnListField(DeviationList, DeviationsList.DeviationDescription).Id] = DeviationData.DeviationDescription;
                newItem[CustomListHelper.ReturnListField(DeviationList, DeviationsList.Responsible).Id] = DeviationData.DeviationResponsible;
                newItem[CustomListHelper.ReturnListField(DeviationList, DeviationsList.DecisionDate).Id] = DeviationData.DecisionDate;
                newItem[CustomListHelper.ReturnListField(DeviationList, DeviationsList.DecisionComment).Id] = DeviationData.DecisionComment;
                newItem[CustomListHelper.ReturnListField(DeviationList, DeviationsList.ActionByDate).Id] = DeviationData.ActionByDate;
                newItem[CustomListHelper.ReturnListField(DeviationList, DeviationsList.FollowUpDate).Id] = DeviationData.FollowUpDate;
                newItem[CustomListHelper.ReturnListField(DeviationList, DeviationsList.FollowUpComment).Id] = DeviationData.FollowUpComment;
                newItem.Update();
            }

            DeviationList.Update();
        }
    }

    class DeviationData
    {

        public string Title, KeyDate, DeviationStatus, DeviationDescription, DeviationResponsible, DecisionDate, DecisionComment, ActionByDate, FollowUpDate, FollowUpComment;
    }

}
