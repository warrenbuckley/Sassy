<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SassyFileEditor.aspx.cs" MasterPageFile="../../masterpages/umbracoPage.Master" Inherits="CWS.Sassy.developer.Sassy.SassyFileEditor" %>
<%@ Register TagPrefix="umb" Namespace="umbraco.uicontrols" Assembly="controls" %>

<asp:content ID="Content1" contentplaceholderid="body" runat="server">

    <umb:UmbracoPanel runat="server" ID="UmbracoPanel" Text="Sass Editor" hasMenu="true">
                
            <umb:Pane runat="server" ID="EditPane" Text="Edit Sass File">
                        
                    <umb:Feedback runat="server" ID="Feedback" Visible="false" />

                    <umb:PropertyPanel runat="server" ID="NamePanel" Text="Name">
                            <asp:TextBox ID="TxtName" Width="350px" runat="server" />
                    </umb:PropertyPanel>
                        
                    <umb:PropertyPanel runat="server" id="PathPanel" Text="Path">
                            <asp:Literal ID="LtrlPath" runat="server" />
                    </umb:PropertyPanel>

                    <umb:PropertyPanel runat="server" ID="EditorPanel">
                            <umb:CodeArea runat="server" ID="EditorSource" EditorMimeType="text/x-sass" AutoResize="true" OffSetX="47" OffSetY="47"  />
                    </umb:PropertyPanel>

            </umb:Pane>

    </umb:UmbracoPanel>

</asp:content>