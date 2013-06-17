<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="manager.aspx.cs" Inherits="tswe.pat.manager" MasterPageFile="/umbraco/masterpages/umbracoPage.Master" validateRequest="false" %>
<%@ Register TagPrefix="cc1" Namespace="umbraco.uicontrols" Assembly="controls" %>
<%@ Register src="testAction.ascx" tagname="testAction" tagprefix="ta1" %>
<asp:Content ID="Content" ContentPlaceHolderID="body" runat="server">
    <cc1:TabView ID="TabView1" runat="server" Width="552px" Height="392px" />
    <ta1:testAction ID="testAction" runat="server" />
</asp:Content>    
