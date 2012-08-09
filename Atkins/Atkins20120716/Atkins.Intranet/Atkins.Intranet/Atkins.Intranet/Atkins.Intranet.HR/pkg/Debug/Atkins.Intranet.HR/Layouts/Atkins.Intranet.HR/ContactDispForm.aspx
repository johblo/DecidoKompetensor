<%@ Assembly Name="Atkins.Intranet.HR, Version=1.0.0.0, Culture=neutral, PublicKeyToken=d815e360c31c37e0" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContactDispForm.aspx.cs" Inherits="Atkins.Intranet.HR.Layouts.ContactDispForm" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server"></asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    
<table  cellpadding="0" cellspacing="0" width="100%" >
<tr><td style="direction: rtl;padding-right: 20px; padding-top:15px;">
          <asp:Button runat="server" Text="Edit" class="ms-ButtonHeightWidth" ID="EditButton"/>        
        </td></tr>
<tr><td>

<table class="ms-formtable" style="margin-top: 8px; padding: 10px" border="0" cellpadding="0" cellspacing="0" width="100%">
        
	    
        <tr><td>
	    <h3 style="text-align:justify;" class="ms-standardheader ms-WPTitle"><nobr><span>
           <img src="/_Layouts/Images/Atkins.Intranet.Portal/Icons/employee.png"/> Employee Information
        </span></nobr></h3>
	    </td></tr>
    
    	<tr>
		<td nowrap="true" valign="top" width="165px" class="ms-formlabel">
		    <h3 class="ms-standardheader"><asp:Label runat="server" ID="employeeNameText" /></h3>
		</td>
        <td valign="top" class="ms-formbody" width="450px" >
            <asp:Label runat="server" ID="employeeNameValue" />
        </td>
        </tr>
        
        <tr>
		<td nowrap="true" valign="top" width="165px" class="ms-formlabel">
		    <h3 class="ms-standardheader"><asp:Label runat="server" ID="employeePersonalNumberText" /></h3>
		</td>
        <td valign="top" class="ms-formbody" width="450px" id="Td1">
             <asp:Label runat="server" ID="employeePersonalNumberValue" />
        </td>
        </tr>
        
       <tr>
		<td nowrap="true" valign="top" width="165px" class="ms-formlabel">
		    <h3 class="ms-standardheader"><asp:Label runat="server" ID="employeePositionText" /></h3>
		</td>
        <td valign="top" class="ms-formbody" width="450px" id="Td2">
             <asp:Label runat="server" ID="employeePositionValue" />
        </td>
        </tr>
        
        <tr>
		<td nowrap="true" valign="top" width="165px" class="ms-formlabel">
		    <h3 class="ms-standardheader"><asp:Label runat="server" ID="employeeManagerText" /></h3>
		</td>
        <td valign="top" class="ms-formbody" width="450px" id="Td3">
             <asp:Label runat="server" ID="employeeManagerValue" />
        </td>
        </tr>
        
       <tr>
		<td nowrap="true" valign="top" width="165px" class="ms-formlabel">
		    <h3 class="ms-standardheader"><asp:Label runat="server" ID="HRResponsibleText" /></h3>
		</td>
        <td valign="top" class="ms-formbody" width="450px" id="Td4">
             <asp:Label runat="server" ID="HRResponsibleValue" />
        </td>
        </tr>
        
         <tr>
		<td nowrap="true" valign="top" width="165px" class="ms-formlabel">
		    <h3 class="ms-standardheader"><asp:Label runat="server" ID="employeeMentorText" /></h3>
		</td>
        <td valign="top" class="ms-formbody" width="450px" id="Td5">
             <asp:Label runat="server" ID="employeeMentorValue" />
        </td>
        </tr>
        
         <tr>
		<td nowrap="true" valign="top" width="165px" class="ms-formlabel">
		    <h3 class="ms-standardheader"><asp:Label runat="server" ID="employeeOfficeText" /></h3>
		</td>
        <td valign="top" class="ms-formbody" width="450px" id="Td6">
             <asp:Label runat="server" ID="employeeOfficeValue" />
        </td>
        </tr>
        
         <tr>
		<td nowrap="true" valign="top" width="165px" class="ms-formlabel">
		    <h3 class="ms-standardheader"><asp:Label runat="server" ID="employeeTemplateText" /></h3>
		</td>
        <td valign="top" class="ms-formbody" width="450px" id="Td7">
             <asp:Label runat="server" ID="employeeTemplateValue" />
        </td>
        </tr>
		</table>

        </td></tr>
        
        <tr><td></td></tr>
        <tr><td id="employeeDocumentsRow" runat="server" style="padding: 10px">
                <h3 style="text-align:justify;" class="ms-standardheader ms-WPTitle"><nobr><span>
        <img src="/_Layouts/Images/Atkins.Intranet.Portal/Icons/docs.png"/> Employee Documents
        </span></nobr></h3> 
        <p style="border-bottom: 1px grey solid;"></p>
            </td></tr>
        
        <tr><td></td></tr>
        <tr><td id="employeeTasksRow" runat="server" style="padding: 10px">
                <h3 style="text-align:justify;" class="ms-standardheader ms-WPTitle"><nobr><span>
        <img src="/_Layouts/Images/Atkins.Intranet.Portal/Icons/task.png"/> Employee Tasks
        </span></nobr></h3> 
        <p style="border-bottom: 1px grey solid;"></p>
            </td></tr>

        <tr><td>
          <asp:Label runat="server" ID="errorMessage"></asp:Label>
        </td></tr>
        
        <tr><td style="direction: rtl;padding-right: 20px;">
          <asp:Button runat="server" Text="Close" class="ms-ButtonHeightWidth" ID="closeButton"/>        
        </td></tr>
        </table>  
        <br/>
        <br/>  
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Contact Information
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Contact Information
</asp:Content>
