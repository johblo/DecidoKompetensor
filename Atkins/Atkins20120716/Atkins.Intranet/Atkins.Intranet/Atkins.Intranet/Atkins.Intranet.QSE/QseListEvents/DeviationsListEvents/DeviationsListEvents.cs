using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using Atkins.Intranet.Utilities.HelperUtils;
using System.Text;
using System.Collections.Specialized;
using Microsoft.SharePoint.Taxonomy;
using System.Collections.Generic;

namespace Atkins.Intranet.QSE.QseListEvents.DeviationsListEvents
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class DeviationsListEvents : SPItemEventReceiver
    {
        string temporaryTaxonomy;
       /// <summary>
       /// An item is being added.
       /// </summary>
       /// 
       public override void ItemAdding(SPItemEventProperties properties)
       {
           base.ItemAdding(properties);
           try
           {
               //TAKE USER IN QSEansvarig and make him dafault responsible user
               SPList settingsList = properties.Web.Lists.TryGetList(CustomListHelper.ReturnTrimmedString(DeviationsSettingsList.ListName));
               if (settingsList != null)
               {
                   if (settingsList.ItemCount > 0)
                   {
                       SPListItem settingsItem = settingsList.Items[0];

                       string fieldName = CustomListHelper.ReturnTrimmedString(DeviationsSettingsList.Responsible);
                       var userField = settingsItem.Fields.GetField(fieldName);

                       SPFieldUserValue userFieldValue = ((SPFieldUserValue)userField.GetFieldValue((string)settingsItem[fieldName]));
                       properties.AfterProperties[CustomListHelper.ReturnTrimmedString(DeviationsList.Responsible)] = userFieldValue;
                   }
               }
           }
           catch (Exception ex) { }
       }

       /// <summary>
       /// An item is being updated.
       /// </summary>
       public override void ItemUpdating(SPItemEventProperties properties)
       {
           base.ItemUpdating(properties);
           /*
           TaxonomyField statusField = properties.ListItem.Fields.GetFieldByInternalName(CustomListHelper.ReturnTrimmedString(DeviationsList.DeviationStatus)) as TaxonomyField;
           TaxonomyFieldValue statusBefore = properties.BeforeProperties[statusField.InternalName] as TaxonomyFieldValue;
           TaxonomyFieldValue statusAfter = properties.ListItem[statusField.Id] as TaxonomyFieldValue;
           TaxonomyFieldValue statusAfter2 = properties.AfterProperties[statusField.InternalName] as TaxonomyFieldValue;
           SPField desField = properties.ListItem.Fields.GetFieldByInternalName(CustomListHelper.ReturnTrimmedString(DeviationsList.DeviationDescription));
           string desBefore = (string)properties.ListItem[desField.Id];
           string desAfter = (string)properties.AfterProperties[desField.InternalName];
           if (statusBefore != statusAfter)
           {
               string s = "";
           }
           */
           List<string> userIds = new List<string>();
           SPField responsibleField = properties.ListItem.Fields.GetFieldByInternalName(CustomListHelper.ReturnTrimmedString(DeviationsList.Responsible));
           SPFieldUserValue responsibleBefore = (SPFieldUserValue)responsibleField.GetFieldValue((string)properties.ListItem[responsibleField.Id]);
           SPFieldUserValue responsibleAfter = (SPFieldUserValue)responsibleField.GetFieldValue((string)properties.AfterProperties[responsibleField.InternalName]);
           if (responsibleAfter != null)
           {
               if ((responsibleBefore == null && responsibleAfter != null) || (responsibleAfter.User.ID != responsibleBefore.User.ID))
               {
                   userIds.Add(responsibleAfter.User.Email);
                   StringDictionary headers = new StringDictionary();
                   headers.Add("to", responsibleAfter.User.Email);
                   headers.Add("cc", "");
                   headers.Add("bcc", "");
                   headers.Add("subject", "Tilldelning av avvikelse/förslag");
                   headers.Add("content-type", "text/html");

                   StringBuilder emailBody = new StringBuilder();
                   emailBody.AppendLine("<h4>Du har blivit tilldelad en avvikelse/förslag.</h4><br/>");
                   emailBody.AppendFormat("<a href='{0}/{1}?ID={2}'>{3}</a> för att se posten", properties.Web.Url, properties.List.Forms[PAGETYPE.PAGE_DISPLAYFORM].Url, properties.ListItemId, "Klicka här");

                   SPUtility.SendEmail(properties.Web, headers, emailBody.ToString());
               }
           }

           SPField userField = properties.ListItem.Fields.GetFieldByInternalName("Author");
           if (userField != null)
           {
               SPFieldUserValue responsible = null;
               SPList settingsList = properties.Web.Lists.TryGetList(CustomListHelper.ReturnTrimmedString(DeviationsSettingsList.ListName));
               if (settingsList != null)
               {
                   if (settingsList.ItemCount > 0)
                   {
                       SPListItem settingsItem = settingsList.Items[0];
                       string fieldName = CustomListHelper.ReturnTrimmedString(DeviationsSettingsList.Responsible);
                       var qseResponsibleField = settingsItem.Fields.GetField(fieldName);
                       if (userField != null)
                       {
                           responsible = ((SPFieldUserValue)userField.GetFieldValue((string)settingsItem[fieldName]));
                       }
                   }
               }


               SPFieldUserValue author = userField.GetFieldValue(properties.ListItem["Author"].ToString()) as SPFieldUserValue;
               
               StringDictionary headers = new StringDictionary();

               string emailUsers = "";
               if (!userIds.Contains(author.User.Email))
               {
                   emailUsers += author.User.Email;
                   userIds.Add(author.User.Email);
               }
               if (!userIds.Contains(responsible.User.Email))
               {
                   if(emailUsers.Length!=0)
                       emailUsers += "," + responsible.User.Email;
                   else
                       emailUsers += responsible.User.Email;
                   userIds.Add(responsible.User.Email);
               }
               headers.Add("to", emailUsers);
               headers.Add("cc", "");
               headers.Add("bcc", "");
               headers.Add("subject", "Ändring av avvikelse/förslag");
               headers.Add("content-type", "text/html");

               StringBuilder emailBody = new StringBuilder();
               emailBody.AppendLine("<h4>Avvikelse/förslag har blivit uppdaterad.</h4><br/>");
               emailBody.AppendFormat("<a href='{0}/{1}?ID={2}'>{3}</a> för att se posten", properties.Web.Url, properties.List.Forms[PAGETYPE.PAGE_DISPLAYFORM].Url, properties.ListItemId, "Klicka här");

               SPUtility.SendEmail(properties.Web, headers, emailBody.ToString());
           }
       }

       /// <summary>
       /// An item was added
       /// </summary>
       public override void ItemAdded(SPItemEventProperties properties)
       {
           //notify default responsible
           SPList settingsList = properties.Web.Lists.TryGetList(CustomListHelper.ReturnTrimmedString(DeviationsSettingsList.ListName));
           if (settingsList != null)
           {
               if (settingsList.ItemCount > 0)
               {
                   SPListItem settingsItem = settingsList.Items[0];
                   string fieldName = CustomListHelper.ReturnTrimmedString(DeviationsSettingsList.Responsible);
                   var userField = settingsItem.Fields.GetField(fieldName);
                   if (userField != null)
                   {
                       SPFieldUserValue responsible = ((SPFieldUserValue)userField.GetFieldValue((string)settingsItem[fieldName]));
                       if (!string.IsNullOrEmpty(responsible.LookupValue))
                       {
                           StringDictionary headers = new StringDictionary();
                           headers.Add("to", responsible.User.Email);
                           headers.Add("cc", "");
                           headers.Add("bcc", "");
                           headers.Add("subject", "Ny avvikelse/förslag");
                           headers.Add("content-type", "text/html");

                           StringBuilder emailBody = new StringBuilder();
                           emailBody.AppendLine("<h4>Ny registrerad avvikelse/förslag.</h4><br/>");
                           emailBody.AppendFormat("<a href='{0}/{1}?ID={2}'>{3}</a> för att se posten", properties.Web.Url, properties.List.Forms[PAGETYPE.PAGE_DISPLAYFORM].Url, properties.ListItemId, "Klicka här");

                           SPUtility.SendEmail(properties.Web, headers, emailBody.ToString());
                       }
                   }
               }
           }
           base.ItemAdded(properties);
       }
    }
}
