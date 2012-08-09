using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Xml.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using Atkins.Intranet.Utilities.HelperUtils;

namespace Atkins.Intranet.SampleData.Features.RootWebLists
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("6541f340-7fc9-4a7a-a31b-6b06f1279fef")]
    public class RootWebListsEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                try
                {
                    SPWeb web = (SPWeb) properties.Feature.Parent;

                    SPList officeList = CustomListHelper.ReturnList(web, OfficeFields.ListName);
                    if (officeList != null)
                    {
                        string configFilePath = SPUtility.GetGenericSetupPath(@"template\layouts\Atkins.Intranet.SampleData\OfficeData.xml");

                        XDocument sampleDocument = XDocument.Load(configFilePath);

                        List<OfficeData> events = CollectOfficeData(sampleDocument);
                        GenerateOfficeItems(web, officeList, events);
                    }
                }
                catch (SPException exception)
                {
                    throw exception;
                }
            });
        }

        private static List<OfficeData> CollectOfficeData(XDocument sampleDocument)
        {
            IEnumerable<XElement> nodes = sampleDocument.Descendants("Offices").Elements("Office");
            List<OfficeData> events = nodes.Select(x => new OfficeData()
            {
                Title = x.Element(OfficeFields.Title).Value,
                Address = x.Element(OfficeFields.Address).Value,
                City = x.Element(OfficeFields.City).Value,
                FaxNumber = x.Element(CustomListHelper.ReturnTrimmedString(OfficeFields.FaxNumber)).Value,
                PhoneNumber = x.Element(CustomListHelper.ReturnTrimmedString(OfficeFields.PhoneNumber)).Value,
                Zip = x.Element(CustomListHelper.ReturnTrimmedString(OfficeFields.Zip)).Value
            }).ToList();

            return events;
        }


        private static void GenerateOfficeItems(SPWeb web, SPList officeList, List<OfficeData> offices)
        {
            foreach (OfficeData officeData in offices)
            {
                SPListItem newItem = officeList.AddItem();
                newItem[SPBuiltInFieldId.Title] = officeData.Title;
                newItem[CustomListHelper.ReturnListField(officeList, OfficeFields.Address).Id] = officeData.Address;
                newItem[CustomListHelper.ReturnListField(officeList, OfficeFields.City).Id] = officeData.City;
                newItem[CustomListHelper.ReturnListField(officeList, OfficeFields.FaxNumber).Id] = officeData.FaxNumber;
                newItem[CustomListHelper.ReturnListField(officeList, OfficeFields.PhoneNumber).Id] = officeData.PhoneNumber;
                newItem[CustomListHelper.ReturnListField(officeList, OfficeFields.Zip).Id] = officeData.Zip;
                newItem.Update();
            }

            officeList.Update();
        }
    }

    class OfficeData
    {
        public string Title, Address, Zip, City, PhoneNumber, FaxNumber;
    }
}
