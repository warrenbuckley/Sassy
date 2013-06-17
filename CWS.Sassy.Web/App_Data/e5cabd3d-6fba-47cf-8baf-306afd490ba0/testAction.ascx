<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="testAction.ascx.cs"
    Inherits="tswe.pat.testAction" %>
<%@ Register Assembly="controls" Namespace="umbraco.uicontrols" TagPrefix="cc1" %>
<cc1:Pane ID="paneTemplate" runat="server">
    <cc1:PropertyPanel ID="ppActionTemplates" runat="server" Text="Action Samples<br /><small>Select a package action and add the sample</small>">
        <asp:DropDownList ID="ddlPackageActions" runat="server" CssClass="guiInputText" Width="250px" />
        &nbsp;
        <asp:Button ID="btnAddTemplate" runat="server" Text="Add Sample" CssClass="guiInputButton"
            OnClick="btnAddTemplate_Click" />
    </cc1:PropertyPanel>
</cc1:Pane>
<cc1:Pane ID="paneResult" runat="server" Visible="False">
    <cc1:PropertyPanel ID="ppActionResult" runat="server" Text="Action Result">
        <asp:Label ID="lblResult" runat="server" Text="Action result." />
    </cc1:PropertyPanel>
</cc1:Pane>
<cc1:Pane ID="paneAction" runat="server">
    <cc1:PropertyPanel ID="ppPackageName" runat="server" Text="Package Name<br /><small>Enter the name of the package</small>">
        <asp:TextBox ID="tbxPackageName" runat="server" CssClass="guiInputText" Width="250px" />
    </cc1:PropertyPanel>
    <cc1:PropertyPanel ID="ppActions" runat="server" Text="Actions Script">
        <small>Below you can enter for test your custom installer / uninstaller actions
            to perform certain tasks during package installation and uninstallation.
            <br />
            All actions are formed as a xml node, containing data for the action to be performed.
            <br />
            Refer the <a href="http://umbraco.tv/assets/package%20actions.pdf" target="_blank">Package
            Actions Documentation</a> or the <a href="http://umbraco.org/documentation/books/package-actions-reference"
            target="_blank">Package Actions Reference</a> at <a href="http://umbraco.org/documentation/books/"
            target="_blank">Umbraco Books</a>.<br /></small>
    </cc1:PropertyPanel>
    <cc1:CodeArea ID="caActionSource" AutoResize="true" OffSetX="47" OffSetY="55" runat="server">
    </cc1:CodeArea>
</cc1:Pane>
