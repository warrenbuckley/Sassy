using System;
using System.Web;
using System.Xml;
using umbraco;
using umbraco.BasePages;
using umbraco.BusinessLogic;
using spa = umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.interfaces;

namespace PackageActionsContrib
{
    /// <summary>
    /// This Package action will Add a new nodeType to the UI.xml file
    /// for additional TreeNode tasks of Umbraco backend trees.
    /// </summary>
    /// <remarks>
    /// This package action is part of the PackageActionsContrib Project
    /// </remarks>
    public class AddTreeNodeType : IPackageAction
    {
        //Set the UI.xml full path
        const string FULL_PATH = "/umbraco/config/create/UI.xml";

        #region IPackageAction Members

        /// <summary>
        /// This Alias must be unique and is used as an identifier that must match 
        /// the alias in the package action XML
        /// </summary>
        /// <returns>The Alias in string format</returns>
        public string Alias()
        {
            return "AddTreeNodeType";
        }

        /// <summary>
        /// Append the xmlData nodes to the UI.xml file
        /// </summary>
        /// <param name="packageName">Name of the package that we install</param>
        /// <param name="xmlData">The data that must be appended to the UI.xml file</param>
        /// <returns>True when succeeded</returns>
        public bool Execute(string packageName, XmlNode xmlData)
        {
            //Set result default to false
            bool result = false;

            //Select new nodes from the supplied xmlData
            XmlNodeList newNodes = xmlData.SelectNodes("//nodeType");

            //Check for new nodes to insert
            if (newNodes.Count == 0) return result;

            //Open the UI.xml file
            XmlDocument document = xmlHelper.OpenAsXmlDocument(FULL_PATH);

            //Select root node in the ui.xml file for append new nodes
            XmlNode rootNode = document.SelectSingleNode("//createUI");

            //Check for rootNode exists
            if (rootNode == null) return result;

            //Set modified document default to false
            bool modified = false;

            //Proceed for each new node fom supplied xmlData
            foreach (XmlNode newNode in newNodes)
            {
                //Set insert node default true
                bool insertNode = true;

                //Look for existing nodes with same alias of new node
                if (rootNode.HasChildNodes)
                {
                    //Get alias of new node
                    string alias = newNode.Attributes["alias"].Value;

                    //Look for existing nodeType nodes
                    XmlNode node = rootNode.SelectSingleNode(
                        String.Format("//nodeType[@alias = '{0}']", alias));

                    //If alias already exists 
                    if (node != null)
                    {
                        //Cancel insert node operation
                        insertNode = false;
                    }
                }
                //Check for insert flag
                if (insertNode)
                {
                    //Append new node to createUI
                    rootNode.AppendChild(document.ImportNode(newNode, true));

                    //Mark document modified
                    modified = true;
                }
            }
            //Check for modified document
            if (modified)
            {
                //Set document node indent in a human readable form
                document.Normalize();
                try
                {
                    //Save the Rewrite config file with the new rewerite rule
                    document.Save(HttpContext.Current.Server.MapPath(FULL_PATH));

                    //No errors so the result is true
                    result = true;
                }
                catch (Exception e)
                {
                    //Log error message
                    string message = "Error at execute AddTreeNodeType package action: " + e.Message;
                    Log.Add(LogTypes.Error, getUser(), -1, message);
                }
            }
            return result;
        }

        /// <summary>
        /// Removes the xmlData nodes from the UI.xml file based on the nodes alias names
        /// </summary>
        /// <param name="packageName">Name of the package that we install</param>
        /// <param name="xmlData">The data that must be appended to the UI.xml file</param>
        /// <returns>True when succeeded</returns>
        public bool Undo(string packageName, System.Xml.XmlNode xmlData)
        {
            //Set result default to false
            bool result = false;

            //Select undo nodes from the supplied xmlData
            XmlNodeList undoNodes = xmlData.SelectNodes("//nodeType");

            //Check for undo nodes to remove
            if (undoNodes.Count == 0) return result;

            //Open the UI.xml file
            XmlDocument document = xmlHelper.OpenAsXmlDocument(FULL_PATH);

            //Select root node in the ui.xml file for remove undo nodes
            XmlNode rootNode = document.SelectSingleNode("//createUI");

            //Check for rootNode exists
            if (rootNode == null) return result;

            //Set modified document default to false
            bool modified = false;

            //Proceed for each undo node fom supplied xmlData
            foreach (XmlNode undoNode in undoNodes)
            {
                //Look for existing nodes with same alias of undo node
                if (rootNode.HasChildNodes)
                {
                    //Get alias of undo node
                    string alias = undoNode.Attributes["alias"].Value;

                    //Look for existing nodeType node with this alias
                    foreach (XmlNode existingNode in rootNode.SelectNodes(
                        String.Format("//nodeType[@alias = '{0}']", alias)))
                    {
                        //Remove existing node from createUI
                        rootNode.RemoveChild(existingNode);
                        modified = true;
                    }
                }
            }
            if (modified)
            {
                //Set document node indent in a human readable form
                document.Normalize();
                try
                {
                    //Save the Rewrite config file with the new rewerite rule
                    document.Save(HttpContext.Current.Server.MapPath(FULL_PATH));

                    //No errors so the result is true
                    result = true;
                }
                catch (Exception e)
                {
                    //Log error message
                    string message = "Error at undo AddTreeNodeType package action: " + e.Message;
                    Log.Add(LogTypes.Error, getUser(), -1, message);
                }
            }
            return result;
        }

        private User getUser()
        {
            int id = BasePage.GetUserId(BasePage.umbracoUserContextID);
            id = (id < 0) ? 0 : id;
            return User.GetUser(id);
        }

        /// <summary>
        /// Returns a Sample XML Node 
        /// In this case the Sample XML Rule for the application tree UI nodeTypes 
        /// of the RSS Feed package
        /// </summary>
        /// <returns>The sample xml as node</returns>
        public XmlNode SampleXml()
        {
            return spa.helper.parseStringToXmlNode(
                "<Action runat=\"install\" undo=\"true/false\" alias=\"AddTreeNodeType\">"
                    + "<nodeType alias=\"initrss\">"
                        + "<header>RSS Feed</header>"
                        + "<usercontrol>/create/simple.ascx</usercontrol>"
                        + "<tasks><create assembly=\"tswe.rss\" type=\"rssCreateTasks\" /></tasks>"
                    + "</nodeType>"
                    + "<nodeType alias=\"rssInstance\">"
                        + "<header>RSS Feed</header>"
                        + "<usercontrol>/create/simple.ascx</usercontrol>"
                        + "<tasks><delete assembly=\"tswe.rss\" type=\"rssCreateTasks\" /></tasks>"
                    + "</nodeType>"
                + "</Action>"
            );
        }

        #endregion

    }
}