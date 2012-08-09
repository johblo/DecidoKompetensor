using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using System.Web;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Linq;
using Atkins.Intranet.Utilities.HelperUtils;


namespace Atkins.Intranet.HR.Layouts
{
    public partial class ContactDispForm : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                InitializePage();

                errorMessage.Text = string.Empty;
                string employeeId = Request.QueryString["ID"];
                SPListItem currentEmployee = ExtractEmployeeInformation(employeeId);

                if (currentEmployee != null)
                {
                    //Add Employee's document listview
                    AddEmployeeDocumentsView(currentEmployee);

                    //Add Employee's task listview
                    AddEmployeeTasksView(currentEmployee);
                }
            }
            catch (SPException exception)
            {
                errorMessage.Text = exception.Message;
            }
        }

        private void AddEmployeeDocumentsView(SPListItem currentEmployee)
        {
            SPList documentLibrary = CustomListHelper.ReturnList(SPContext.Current.Web, EmployeeDocuments.ListName);
            ListViewByQuery documentView = new ListViewByQuery();
            if (documentLibrary != null)
            {
                documentView.List = documentLibrary;
                SPQuery documentQuery = new SPQuery(documentView.List.DefaultView);
                documentQuery.Query = string.Empty;

                SPView viewTest = documentView.List.DefaultView;
                viewTest.Scope = SPViewScope.Recursive;
                viewTest.Query = "<Where><Eq><FieldRef Name='" + CustomListHelper.ReturnTrimmedString(EmployeeDocuments.EmployeeName) + "' /><Value Type='Lookup'>" + currentEmployee[SPBuiltInFieldId.Title].ToString() + "</Value></Eq></Where>";
                viewTest.Paged = true;
                documentQuery = new SPQuery(viewTest);
                documentQuery.ViewFields = "<FieldRef Name='DocIcon'/><FieldRef Name='LinkFilename'/>";

                documentView.Query = documentQuery;
            }
            employeeDocumentsRow.Controls.Add(documentView);
        }

        private void AddEmployeeTasksView(SPListItem currentEmployee)
        {
            SPList taskList = CustomListHelper.ReturnList(SPContext.Current.Web, IntroductionTasksFields.ListName);
            ListViewByQuery taskView = new ListViewByQuery();
            if (taskList != null)
            {
                taskView.List = taskList;
                SPQuery taskQuery = new SPQuery(taskView.List.DefaultView);
                taskQuery.Query = string.Empty;

                SPView viewTest = taskView.List.DefaultView;
                viewTest.Scope = SPViewScope.Recursive;
                viewTest.Query = "<Where><Eq><FieldRef Name='" +
                                 CustomListHelper.ReturnTrimmedString(IntroductionTasksFields.Employee) +
                                 "' /><Value Type='Lookup'>" + currentEmployee[SPBuiltInFieldId.Title].ToString() +
                                 "</Value></Eq></Where>";
                viewTest.Paged = true;
                taskQuery = new SPQuery(viewTest);
                taskQuery.ViewFields = "<FieldRef Name='LinkTitle'/><FieldRef Name='" + CustomListHelper.ReturnTrimmedString(IntroductionTasksFields.TaskAssignee) + "'/><FieldRef Name='"+ IntroductionTasksFields.Completed +"'/><FieldRef Name='"+ IntroductionTasksFields.DueDate +"'/>";

                taskView.Query = taskQuery;
            }
            employeeTasksRow.Controls.Add(taskView);
        }


        private SPListItem ExtractEmployeeInformation(string employeeId)
        {
            SPListItem employeeItem = null;

            if (!string.IsNullOrEmpty(employeeId))
            {
                SPWeb currentWeb = SPContext.Current.Web;
                SPList contactList = CustomListHelper.ReturnList(currentWeb, EmployeeContactFields.ListName);
                if (contactList != null)
                {
                    List<SPListItem> currentEmployee = contactList.Items.Cast<SPListItem>().Where(x => x.ID.Equals(Int32.Parse(employeeId))).ToList();
                    if (currentEmployee.Count > 0)
                    {
                        employeeItem = currentEmployee[0];

                        employeeNameText.Text = EmployeeContactFields.Title;
                        employeeNameValue.Text = employeeItem[SPBuiltInFieldId.Title].ToString();

                        employeePersonalNumberText.Text = EmployeeContactFields.PersonalNumber;
                        employeePersonalNumberValue.Text = employeeItem[CustomListHelper.ReturnListField(contactList, EmployeeContactFields.PersonalNumber).Id].ToString();
                        
                        employeePositionText.Text = EmployeeContactFields.Position;
                        employeePositionValue.Text = employeeItem[CustomListHelper.ReturnListField(contactList, EmployeeContactFields.Position).Id].ToString();

                        employeeManagerText.Text = EmployeeContactFields.Manager;
                        employeeManagerValue.Text = CustomListHelper.ReturnUserDisplayName(employeeItem[CustomListHelper.ReturnListField(contactList, EmployeeContactFields.Manager).Id]);

                        HRResponsibleText.Text = EmployeeContactFields.HR_Responsible;
                        HRResponsibleValue.Text =
                            CustomListHelper.ReturnUserDisplayName(employeeItem[CustomListHelper.ReturnListField(contactList, EmployeeContactFields.HR_Responsible).Id]);

                        employeeMentorText.Text = EmployeeContactFields.Mentor;
                        employeeMentorValue.Text =
                            CustomListHelper.ReturnUserDisplayName(employeeItem[CustomListHelper.ReturnListField(contactList, EmployeeContactFields.Mentor).Id]);

                        employeeOfficeText.Text = EmployeeContactFields.Office;
                        employeeOfficeValue.Text = (employeeItem[CustomListHelper.ReturnListField(contactList, EmployeeContactFields.Office).Id] == null)
                                                     ? string.Empty
                                                     : employeeItem[CustomListHelper.ReturnListField(contactList, EmployeeContactFields.Office).Id].ToString().Split('#')[1];

                        employeeTemplateText.Text = EmployeeContactFields.IntroductionTemplate;
                        employeeTemplateValue.Text = (employeeItem[CustomListHelper.ReturnListField(contactList, EmployeeContactFields.IntroductionTemplate).Id] == null)
                                                         ? string.Empty
                                                         : employeeItem[CustomListHelper.ReturnListField(contactList, EmployeeContactFields.IntroductionTemplate).Id].
                                                               ToString().Split('#')[1];
                    }
                }
            }

            return employeeItem;
        }

        private void InitializePage()
        {
            Page.Header.Controls.Add(new Literal() {Text = "<link rel='stylesheet' type='text/css' href='/_layouts/1033/styles/Themable/forms.css' />"});
            Page.Header.Controls.Add(new Literal() {Text = "<link rel='stylesheet' type='text/css' href='/_layouts/1033/styles/Themable/search.css' />"});
            Page.Header.Controls.Add(new Literal() {Text = "<link rel='stylesheet' type='text/css' href='/_layouts/1033/styles/Themable/corev4.css' />"});

            closeButton.Click += new EventHandler(closeButton_Click);
            EditButton.Click += new EventHandler(EditButton_Click);
        }

        void EditButton_Click(object sender, EventArgs e)
        {
            string employeeId = Request.QueryString["ID"];
            if(!string.IsNullOrEmpty(employeeId))
            {
                SPWeb currentWeb = SPContext.Current.Web;
                SPList contactList = CustomListHelper.ReturnList(currentWeb, EmployeeContactFields.ListName);
                string edidFormUrl = contactList.DefaultEditFormUrl + "?ID=" + employeeId;
                Response.Redirect(edidFormUrl);
            }
        }

        void closeButton_Click(object sender, EventArgs e)
        {
            string previousPage = Request.QueryString["Source"];
            if (!string.IsNullOrEmpty(previousPage))
            {

                Response.Redirect(previousPage);
            }
        }
    }
}
