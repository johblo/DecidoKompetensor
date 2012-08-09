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

namespace Atkins.Intranet.SampleData.Features.HRWebLists
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("1e0c0289-7675-4dba-ae70-92544c0127e8")]
    public class HRWebListsEventReceiver : SPFeatureReceiver
    {
        
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                try
                {
                    SPWeb web = (SPWeb)properties.Feature.Parent;

                    SPList templateList = CustomListHelper.ReturnList(web, IntroductionTemplateFields.ListName);
                    if (templateList != null)
                    {
                        string configFilePath = SPUtility.GetGenericSetupPath(@"template\layouts\Atkins.Intranet.SampleData\TemplateData.xml");
                        XDocument sampleDocument = XDocument.Load(configFilePath);
                        List<TemplateData> data = CollectTemplateData(sampleDocument);
                        GenerateTemplateItems(web, templateList, data);
                    }

                }
                catch (SPException exception)
                {
                   // throw exception;
                }
            });
        }

        private static List<TemplateData> CollectTemplateData(XDocument sampleDocument)
        {
            IEnumerable<XElement> nodes = sampleDocument.Descendants("Templates").Elements("Template");
            List<TemplateData> events = nodes.Select(x => new TemplateData()
            {
                Title = x.Element(IntroductionTemplateFields.Title).Value,
                TemplateSteps= x.Element(CustomListHelper.ReturnTrimmedString(IntroductionTemplateFields.TemplateSteps)).Value,
                TemplateIsActive = x.Element(CustomListHelper.ReturnTrimmedString(IntroductionTemplateFields.TemplateIsActiveXML)).Value
            }).ToList();

            return events;
        }

        private static void GenerateTemplateItems(SPWeb web, SPList templateList, List<TemplateData> templates)
        {
            foreach (TemplateData templateData in templates)
            {
                SPListItem newItem = templateList.AddItem();
                newItem[SPBuiltInFieldId.Title] = templateData.Title;
                newItem[CustomListHelper.ReturnListField(templateList, IntroductionTemplateFields.TemplateSteps).Id] = templateData.TemplateSteps;
                newItem[CustomListHelper.ReturnListField(templateList, IntroductionTemplateFields.TemplateIsActive).Id] = templateData.TemplateIsActive;
                newItem.Update();
            }

            templateList.Update();
        }

    }


    class TemplateData
    {
        public string Title, TemplateSteps, TemplateIsActive;
    }

    class EmployeeData
    {
        public string Title, Position, PersonalNumber, Office, Manager, HRResponsible, Mentor, IntroductionTemplate;
    }
    
}
