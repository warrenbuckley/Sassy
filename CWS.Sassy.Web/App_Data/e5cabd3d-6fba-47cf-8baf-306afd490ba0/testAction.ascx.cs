using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

using umbraco;
using umbraco.BusinessLogic.Utils;
using umbraco.interfaces;
using umbraco.uicontrols;
using System.Xml;
using umbraco.cms.businesslogic.packager;
using System.IO;
using umbraco.BasePages;
using umbraco.BusinessLogic;

namespace tswe.pat
{
    public partial class testAction : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            }
        }

        protected override void OnInit(EventArgs e)
        {
            if (Parent is TabPage)
                InitControlItems();

            base.OnInit(e);
        }

        public void InitControlItems()
        {
            // init menu buttons
            TabPage tp = this.Parent as TabPage;

            ImageButton install = tp.Menu.NewImageButton();
            install.ImageUrl = GlobalSettings.Path + "/plugins/tswe/pat/images/install.png";
            install.CommandName = "install";
            install.AlternateText = "Install Actions";
            install.Command += new CommandEventHandler(install_Command);

            tp.Menu.InsertSplitter();

            ImageButton undo = tp.Menu.NewImageButton();
            undo.ImageUrl = GlobalSettings.Path + "/plugins/tswe/pat/images/undo.png";
            undo.CommandName = "undo";
            undo.AlternateText = "Undo Actions";
            undo.Command += new CommandEventHandler(undo_Command);

            tp.Menu.InsertSplitter();

            ImageButton clearScript = tp.Menu.NewImageButton();
            clearScript.ImageUrl = GlobalSettings.Path + "/plugins/tswe/pat/images/clear_script.png";
            clearScript.CommandName = "clear";
            clearScript.AlternateText = "Clear Script";
            clearScript.Command += new CommandEventHandler(clearScript_Command);

            // list all available package actions
            SortedList<string, string> actions = new SortedList<string, string>();

            List<Type> types = TypeFinder.FindClassesOfType<IPackageAction>(true);
            foreach (Type type in types)
            {
                IPackageAction typeInstance = Activator.CreateInstance(type) as IPackageAction;
                if (typeInstance != null)
                {
                    string alias = typeInstance.Alias();
                    string sample = String.Format(
                        "<!-- Sample action for {0} is not implemented! -->", alias);
                    try
                    {
                        // Create a stream buffer that can be read as a string
                        using (StringWriter sw = new StringWriter())

                        // Create a specialized writer for XML code
                        using (XmlTextWriter xtw = new XmlTextWriter(sw))
                        {
                            // Set the writer to use indented (hierarchical) elements
                            xtw.Formatting = System.Xml.Formatting.Indented;

                            // Write the XML sample to the stream
                            typeInstance.SampleXml().WriteContentTo(xtw);

                            // write the stream to a string
                            sample = sw.ToString();
                        }
                    }
                    catch
                    {
                    }
                    if (!actions.ContainsKey(alias))
                    {
                        actions.Add(alias, sample);
                    }
                }
            }

            foreach (KeyValuePair<string, string> kvp in actions)
            {
                ListItem li = new ListItem(kvp.Key, kvp.Value);
                ddlPackageActions.Items.Add(li);
            }
        }

        void clearScript_Command(object sender, CommandEventArgs e)
        {
            paneResult.Visible = false;
            caActionSource.Text = String.Empty;
            caActionSource.Focus();
        }

        void undo_Command(object sender, CommandEventArgs e)
        {
            executePackageActions(false);
        }

        void install_Command(object sender, CommandEventArgs e)
        {
            executePackageActions(true);
        }

        protected void btnAddTemplate_Click(object sender, EventArgs e)
        {
            
            caActionSource.Text += ddlPackageActions.SelectedValue + "\n";
        }

        private User getUser()
        {
            if (Page is BasePage)
                //Okay we are in Umbraco
                return ((BasePage)Page).getUser();
            else
                return User.GetUser(0);
        }
        
        private XmlNode getXmlData(string value)
        {
            XmlDocument document = new XmlDocument();
            XmlNode node = document.CreateElement("Actions");
            node.InnerXml = value;
            document.AppendChild(node);
            document.Normalize();
            return document;
        }

        private void executePackageActions(bool install)
        {
            string actionType = (install) ? "Install" : "Undo";
            int actionsCount = 0;
            string packageName = tbxPackageName.Text;
            try
            {
                XmlNode actionData = getXmlData(caActionSource.Text);
                XmlNodeList actions = actionData.SelectNodes("//Actions/Action");
                foreach (XmlNode action in actions)
                {
                    string actionAlias = action.Attributes["alias"].Value;
                    if (install)
                        PackageAction.RunPackageAction(packageName, actionAlias, action);
                    else
                        PackageAction.UndoPackageAction(packageName, actionAlias, action);
                    actionsCount++;
                }

                string header = String.Format("{0} successful", actionType);
                string message = String.Format("{0} package actions executed.", actionsCount);
                showMessage(header, message, false);
            }
            catch (Exception e)
            {
                string header = String.Format("{0} Error", actionType);
                string message = String.Format("Exception on {0}: {1}", actionType, e.Message);
                Log.Add(LogTypes.Error, getUser(), -1, message);
                message = String.Format("Unable to {0} package actions. Check the log.", actionType.ToLower());
                showMessage(header, message, true);
            }
        }

        private void showMessage(string header, string message, bool isError)
        {

            //Are we are in Umbraco
            if (Page is BasePage)
            {
                //Set speech bubble icon
                BasePage.speechBubbleIcon icon = (isError) ? BasePage.speechBubbleIcon.error
                    : BasePage.speechBubbleIcon.success;

                //Show speech bubble
                ((BasePage)Page).speechBubble(icon, header, message);
            }
            else
            {
                //Show error result pane
                paneResult.Visible = true;
                lblResult.Text = message;
                if (isError)
                    lblResult.ForeColor = Color.Red;
                else
                    lblResult.ForeColor = Color.Black;
            }
        }
    }
}