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
    /// This Package action will Add a new HTTP Module to the web.config file
    /// </summary>
    /// <remarks>
    /// This package action is part of the PackageActionsContrib Project
    /// </remarks>
    public class AddHttpModule : IPackageAction
    {
        //Set the web.config full path
        const string FULL_PATH = "~/web.config";

        #region IPackageAction Members

        /// <summary>
        /// This Alias must be unique and is used as an identifier that must match 
        /// the alias in the package action XML
        /// </summary>
        /// <returns>The Alias in string format</returns>
        public string Alias()
        {
            return "AddHttpModule";
        }

        /// <summary>
        /// Append the xmlData node to the web.config file
        /// </summary>
        /// <param name="packageName">Name of the package that we install</param>
        /// <param name="xmlData">The data that must be appended to the web.config file</param>
        /// <returns>True when succeeded</returns>
        public bool Execute(string packageName, XmlNode xmlData)
        {
            //Set result default to false
            bool result = false;

            //Get attribute values of xmlData
            string position, name, type, preCondition;
            position = getAttributeDefault(xmlData, "position", null);
            if (!getAttribute(xmlData, "name", out name)) return result;
            if (!getAttribute(xmlData, "type", out type)) return result;
            preCondition = getAttributeDefault(xmlData, "preCondition", null);

            //Create a new xml document
            XmlDocument document = new XmlDocument();

            //Keep current indentions format
            document.PreserveWhitespace = true;

            //Load the web.config file into the xml document
            document.Load(HttpContext.Current.Server.MapPath(FULL_PATH));

            //Set modified document default to false
            bool modified = false;

            #region IIS6

            //Select root node in the web.config file for insert new nodes
            XmlNode rootNode = document.SelectSingleNode("//configuration/system.web/httpModules");

            //Set insert node default true
            bool insertNode = true;

            //Check for rootNode exists
            if (rootNode != null)
            {
                //Look for existing nodes with same name like the new node
                if (rootNode.HasChildNodes)
                {
                    //Look for existing nodeType nodes
                    XmlNode node = rootNode.SelectSingleNode(
                        String.Format("add[@name = '{0}']", name));

                    //If name already exists 
                    if (node != null)
                    {
                        //Cancel insert node operation
                        insertNode = false;
                    }
                }
                //Check for insert flag
                if (insertNode)
                {
                    //Create new node with attributes
                    XmlNode newNode = document.CreateElement("add");
                    newNode.Attributes.Append(
                        xmlHelper.addAttribute(document, "name", name));
                    newNode.Attributes.Append(
                        xmlHelper.addAttribute(document, "type", type));

                    //Select for new node insert position
                    if (position == null || position == "end")
                    {
                        //Append new node at the end of root node
                        rootNode.AppendChild(newNode);

                        //Mark document modified
                        modified = true;
                    }
                    else if (position == "beginning")
                    {
                        //Prepend new node at the beginnig of root node
                        rootNode.PrependChild(newNode);

                        //Mark document modified
                        modified = true;
                    }
                }
            }

            #endregion

            #region IIS7

            //Set insert node default true
            insertNode = true;

            rootNode = document.SelectSingleNode("//configuration/system.webServer/modules");

            if (rootNode != null && name != null)
            {
                //Look for existing nodes with same path like the new node
                if (rootNode.HasChildNodes)
                {
                    //Look for existing nodeType nodes
                    XmlNode node = rootNode.SelectSingleNode(
                        String.Format("add[@name = '{0}']", name));

                    //If path already exists 
                    if (node != null)
                    {
                        //Cancel insert node operation
                        insertNode = false;
                    }
                }
                //Check for insert flag
                if (insertNode)
                {
                    //Create new remove node with attributes
                    XmlNode newRemoveNode = document.CreateElement("remove");
                    newRemoveNode.Attributes.Append(
                        xmlHelper.addAttribute(document, "name", name));

                    //Create new add node with attributes
                    XmlNode newAddNode = document.CreateElement("add");
                    newAddNode.Attributes.Append(
                        xmlHelper.addAttribute(document, "name", name));                                        
                    newAddNode.Attributes.Append(
                        xmlHelper.addAttribute(document, "type", type));

                    //Attribute preCondition is optional
                    if (preCondition != null)
                    {
                        newAddNode.Attributes.Append(
                            xmlHelper.addAttribute(document, "preCondition", preCondition));
                    }

                    //Select for new node insert position
                    if (position == null || position == "end")
                    {
                        //Append new node at the end of root node
                        rootNode.AppendChild(newRemoveNode);
                        rootNode.AppendChild(newAddNode);

                        //Mark document modified
                        modified = true;
                    }
                    else if (position == "beginning")
                    {
                        //Prepend new node at the beginnig of root node
                        rootNode.PrependChild(newAddNode);
                        rootNode.PrependChild(newRemoveNode);

                        //Mark document modified
                        modified = true;
                    }
                }
            }

            #endregion

            //Check for modified document
            if (modified)
            {
                try
                {
                    //Save the web config file with the new HttpModule
                    document.Save(HttpContext.Current.Server.MapPath(FULL_PATH));

                    //No errors so the result is true
                    result = true;
                }
                catch (Exception e)
                {
                    //Log error message
                    string message = "Error at execute AddHttpModule package action: " + e.Message;
                    Log.Add(LogTypes.Error, getUser(), -1, message);
                }
            }
            return result;
        }

        /// <summary>
        /// Removes the xmlData node from the web.config file
        /// </summary>
        /// <param name="packageName">Name of the package that we install</param>
        /// <param name="xmlData">The data that must be appended to the web.config file</param>
        /// <returns>True when succeeded</returns>
        public bool Undo(string packageName, System.Xml.XmlNode xmlData)
        {
            //Set result default to false
            bool result = false;

            //Get attribute values of xmlData
            string name;
            if (!getAttribute(xmlData, "name", out name)) return result;

            //Create a new xml document
            XmlDocument document = new XmlDocument();

            //Keep current indentions format
            document.PreserveWhitespace = true;

            //Load the web.config file into the xml document
            document.Load(HttpContext.Current.Server.MapPath(FULL_PATH));

            //Set modified document default to false
            bool modified = false;

            #region IIS6

            //Select root node in the web.config file for remove nodes
            XmlNode rootNode = document.SelectSingleNode("//configuration/system.web/httpModules");

            //Check for rootNode exists
            if (rootNode != null)
            {
                //Look for existing nodes with same name of undo attribute
                if (rootNode.HasChildNodes)
                {
                    //Look for existing add nodes with attribute name
                    foreach (XmlNode existingNode in rootNode.SelectNodes(
                        String.Format("add[@name = '{0}']", name)))
                    {
                        //Remove existing node from root node
                        rootNode.RemoveChild(existingNode);
                        modified = true;
                    }
                }
            }

            #endregion

            #region IIS7

            //Select root node in the web.config file for insert new nodes
            rootNode = document.SelectSingleNode("//configuration/system.webServer/modules");

            //Check for rootNode exists
            if (rootNode != null && name != null)
            {
                //Look for existing nodes with same name of undo attribute
                if (rootNode.HasChildNodes)
                {
                    //Look for existing remove nodes with attribute path
                    foreach (XmlNode existingNode in rootNode.SelectNodes(String.Format("remove[@name = '{0}']", name)))
                    {
                        //Remove existing node from root node
                        rootNode.RemoveChild(existingNode);
                        modified = true;
                    }

                    //Look for existing add nodes with attribute path
                    foreach (XmlNode existingNode in rootNode.SelectNodes(String.Format("add[@name = '{0}']", name)))
                    {
                        //Remove existing node from root node
                        rootNode.RemoveChild(existingNode);
                        modified = true;
                    }
                }
            }

            #endregion

            if (modified)
            {
                try
                {
                    //Save the Web config file with the new HttpModule rule
                    document.Save(HttpContext.Current.Server.MapPath(FULL_PATH));
                    //No errors so the result is true
                    result = true;
                }
                catch (Exception e)
                {
                    //Log error message
                    string message = "Error at undo AddHttpModule package action: " + e.Message;
                    Log.Add(LogTypes.Error, getUser(), -1, message);
                }
            }
            return result;
        }

        /// <summary>
        /// Get the current user, or when unavailable admin user
        /// </summary>
        /// <returns>The current user</returns>
        private User getUser()
        {
            int id = BasePage.GetUserId(BasePage.umbracoUserContextID);
            id = (id < 0) ? 0 : id;
            return User.GetUser(id);
        }

        /// <summary>
        /// Get a named attribute from xmlData root node
        /// </summary>
        /// <param name="xmlData">The data that must be appended to the web.config file</param>
        /// <param name="attribute">The name of the attribute</param>
        /// <param name="value">returns the attribute value from xmlData</param>
        /// <returns>True, when attribute value available</returns>
        private bool getAttribute(XmlNode xmlData, string attribute, out string value)
        {
            //Set result default to false
            bool result = false;

            //Out params must be assigned
            value = String.Empty;

            //Search xml attribute
            XmlAttribute xmlAttribute = xmlData.Attributes[attribute];

            //When xml attribute exists
            if (xmlAttribute != null)
            {
                //Get xml attribute value
                value = xmlAttribute.Value;

                //Set result successful to true
                result = true;
            }
            else
            {
                //Log error message
                string message = "Error at AddHttpModule package action: "
                    + "Attribute \"" + attribute + "\" not found.";
                Log.Add(LogTypes.Error, getUser(), -1, message);
            }
            return result;
        }

        /// <summary>
        /// Get an optional named attribute from xmlData root node
        /// when attribute is unavailable, return the default value
        /// </summary>
        /// <param name="xmlData">The data that must be appended to the web.config file</param>
        /// <param name="attribute">The name of the attribute</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns>The attribute value or the default value</returns>
        private string getAttributeDefault(XmlNode xmlData, string attribute, string defaultValue)
        {
            //Set result default value
            string result = defaultValue;

            //Search xml attribute
            XmlAttribute xmlAttribute = xmlData.Attributes[attribute];

            //When xml attribute exists
            if (xmlAttribute != null)
            {
                //Get available xml attribute value
                result = xmlAttribute.Value;
            }
            return result;
        }

        /// <summary>
        /// Returns a Sample XML Node 
        /// In this case the Sample HTTP Module TimingModule 
        /// </summary>
        /// <returns>The sample xml as node</returns>
        public XmlNode SampleXml()
        {
            return spa.helper.parseStringToXmlNode(
                "<Action runat=\"install\" undo=\"true/false\" alias=\"AddHttpModule\" "
                    + "position=\"beginning/end\" "
                    + "name=\"TimingModule\" "
                    + "type=\"Timer, TimingModule\" "
                    + "preCondition=\"managedHandler\" />"
            );
        }

        #endregion
    }
}