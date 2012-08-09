using System;
using System.Collections.Generic;
using System.Linq;
using Atkins.Intranet.Utilities.HelperUtils;
using Microsoft.SharePoint;

namespace Atkins.Intranet.HR.ContactTaskEventReceiver
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class ContactTaskEventReceiver : SPItemEventReceiver
    {

        private static List<SPListItem> initialSelectedTemplate = new List<SPListItem>();
       /// <summary>
       /// An item was added.
       /// </summary>
        public override void ItemAdded(SPItemEventProperties properties)
       {
           base.ItemAdded(properties);

           try
           {
               SPSite currentSite = new SPSite(properties.SiteId);
               SPWeb currentWeb = currentSite.OpenWeb(properties.RelativeWebUrl);
               SPList employeeList = properties.List;
               SPListItem newEmployeeItem = properties.ListItem;

               Guid templateFieldId = CustomListHelper.ReturnListField(employeeList, EmployeeContactFields.IntroductionTemplate).Id;
               GenerateTasksForEmployee(currentWeb, newEmployeeItem, templateFieldId);
           }
           catch (SPException exception)
           {
               throw exception;
           }
       }

       
        /// <summary>
       /// An item was updated.
       /// </summary>
       public override void ItemUpdating(SPItemEventProperties properties)
       {
           base.ItemUpdating(properties);
           try
           {
               SPListItem employeeItem = properties.ListItem;
               initialSelectedTemplate.Add(employeeItem);
           }
           catch (SPException exception)
           {
               throw exception;
           }
       }

       /// <summary>
       /// An item was updated.
       /// </summary>
       public override void ItemUpdated(SPItemEventProperties properties)
       {
           base.ItemUpdated(properties);
           try
           {
               SPSite currentSite = new SPSite(properties.SiteId);
               SPWeb currentWeb = currentSite.OpenWeb(properties.RelativeWebUrl);
               SPList employeeList = properties.List;
               SPListItem newEmployeeItem = properties.ListItem;

               SPListItem oldEmployeeItem = null;
               if (initialSelectedTemplate.Where(x => x.ID .Equals(newEmployeeItem.ID)).Count()>0)
               {
                   oldEmployeeItem = initialSelectedTemplate.Where(x => x.ID.Equals(newEmployeeItem.ID)).ToList()[0];
               }

               Guid templateFieldId = CustomListHelper.ReturnListField(employeeList, EmployeeContactFields.IntroductionTemplate).Id;
               if ((oldEmployeeItem != null) && (!newEmployeeItem[templateFieldId].Equals(oldEmployeeItem[templateFieldId])))
               {
                   GenerateTasksForEmployee(currentWeb, newEmployeeItem, templateFieldId);
                   initialSelectedTemplate.Remove(oldEmployeeItem);
               }
           }
           catch (SPException exception)
           {
               throw exception;
           }
       }

       private static void GenerateTasksForEmployee(SPWeb currentWeb, SPListItem newEmployeeItem, Guid templateFieldId)
       {

           SPList templateList = CustomListHelper.ReturnList(currentWeb, IntroductionTemplateFields.ListName);
           if (templateList != null)
           {
               string templateValue = newEmployeeItem[templateFieldId].ToString();
               SPListItem templateItem = templateList.Items.GetItemById(Int16.Parse(templateValue.Split(';')[0]));

               Guid templateActiveFieldId = CustomListHelper.ReturnListField(templateList, IntroductionTemplateFields.TemplateIsActive).Id;
               if ((bool)templateItem[templateActiveFieldId])
               {
                   SPField templateStepField = CustomListHelper.ReturnListField(templateList, IntroductionTemplateFields.TemplateSteps);
                   if (templateStepField != null)
                   {
                       Guid templateStepFieldId = templateStepField.Id;
                       List<string> templateSteps = templateItem[templateStepFieldId].ToString().Split('\n').ToList();

                       SPList taskList = CustomListHelper.ReturnList(currentWeb, IntroductionTasksFields.ListName);
                       if (taskList != null)
                       {
                           foreach (string templateStep in templateSteps)
                           {
                               if (!string.IsNullOrEmpty(templateStep))
                               {
                                   SPListItem newTask = taskList.Items.Add();
                                   newTask[SPBuiltInFieldId.Title] = templateStep;
                                   Guid employeeFieldId = CustomListHelper.ReturnListField(taskList, IntroductionTasksFields.Employee).Id;
                                   newTask[employeeFieldId] = newEmployeeItem;
                                   newTask.Update();
                               }
                           }
                       }
                   }
               }
           }
       }


    }
}
