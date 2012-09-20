<%@ Page language="C#" MasterPageFile="~masterurl/default.master"  Inherits="Microsoft.SharePoint.Publishing.PublishingLayoutPage,Microsoft.SharePoint.Publishing,Version=14.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePointWebControls" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="PublishingWebControls" Namespace="Microsoft.SharePoint.Publishing.WebControls" Assembly="Microsoft.SharePoint.Publishing, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="PublishingNavigation" Namespace="Microsoft.SharePoint.Publishing.Navigation" Assembly="Microsoft.SharePoint.Publishing, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ContentPlaceholderID="PlaceHolderPageTitle" runat="server">
	<SharePoint:ListItemProperty ID="ListItemProperty1" Property="Title" maxlength="40" runat="server"/>
</asp:Content>

<asp:Content ContentPlaceholderID="PlaceHolderAdditionalPageHead" runat="server">
	<SharePoint:CssRegistration ID="AtkinsPortalMasterCss" name="<% $SPUrl:~SiteCollection/_layouts/Atkins.Intranet.Portal/css/AtkinsSubSitePageLayout.css%>" After="corev4.css" runat="server"/>
</asp:Content>





<asp:Content ContentPlaceholderID="PlaceHolderMain" runat="server"> 
       <table width="100%" cellpadding="0" cellspacing="0"><tr>
       <td class="zonegap"></td>                                       
       <td class="zoneColumnLeft">
        <div class="layout-left-zone">
            <WebPartPages:WebPartZone runat="server" Title="loc:Left" ID="Left" FrameType="TitleBarOnly" class="layout-right-zone">
            <ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
        </div>
        </td>
        
        <td class="zonegap"></td>
        <td class="zoneColumnRight">
        <div class="layout-right-zone">
            <WebPartPages:WebPartZone runat="server" Title="loc:Right" ID="Right" FrameType="TitleBarOnly" class="layout-right-zone"><ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
        </div>
        </td>
        <td class="zonegap"></td>
       </tr>
       </table> 
    
</asp:Content>
