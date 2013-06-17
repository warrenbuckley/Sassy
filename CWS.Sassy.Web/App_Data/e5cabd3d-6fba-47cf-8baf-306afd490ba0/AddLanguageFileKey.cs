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
    /// This Package action will Add a key to one of the language files
    /// Added on 15/07/09 by Chris Houston from http://www.vizioz.com
    /// </summary>
    /// <remarks>
    /// This package action is part of the PackageActionsContrib Project
    /// </remarks>
    public class AddLanguageFileKey : IPackageAction
    {
        //Set the path of the language files directory
        const string DIR_PATH = "~/umbraco/config/lang/";

        #region IPackageAction Members

        /// <summary>
        /// This Alias must be unique and is used as an identifier that must match 
        /// the alias in the package action XML
        /// </summary>
        /// <returns>The Alias in string format</returns>
        public string Alias()
        {
            return "AddLanguageFileKey";
        }

        /// <summary>
        /// Add the new language key to a specified language file
        /// </summary>
        /// <param name="packageName">Name of the package that we are installing</param>
        /// <param name="xmlData">The data that must be appended to the language file</param>
        /// <returns>True when succeeded</returns>
        public bool Execute(string packageName, XmlNode xmlData)
        {
            //Set result default to false
            bool result = false;

            //Get attribute values of xmlData
            string language, position, area, key, value;
            language = getAttributeDefault(xmlData, "language", "en");
            position = getAttributeDefault(xmlData, "position", null);
            if (!getAttribute(xmlData, "area", out area)) return result;
            if (!getAttribute(xmlData, "key", out key)) return result;
            if (!getAttribute(xmlData, "value", out value)) return result;

            //Create a new xml document
            XmlDocument document = new XmlDocument();

            //Keep current indentions format
            document.PreserveWhitespace = true;

            //Load the language file into the xml document
            document.Load(HttpContext.Current.Server.MapPath(DIR_PATH + language + ".xml"));
            //Select root node in the web.config file to insert new node
            //RS: We need to ensure that the area rootnode exists
            XmlNode rootNode = EnsureAreaRootNode(area, document);

            //Check for rootNode exists
            if (rootNode == null) return result;

            //Set modified document default to false
            bool modified = false;

            //Set insert node default true
            bool insertNode = true;

            //Look for existing nodes with same path like the new node
            if (rootNode.HasChildNodes)
            {
                //Look for existing nodeType nodes
                XmlNode node = rootNode.SelectSingleNode(
                    String.Format("//key[@alias = '{0}']", key));

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
                //Create new node with attributes
                XmlNode newNode = document.CreateElement("key");
                newNode.Attributes.Append(
                    xmlHelper.addAttribute(document, "alias", key));
                newNode.InnerText = value;

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
                    //Prepend new node at the beginning of root node
                    rootNode.PrependChild(newNode);

                    //Mark document modified
                    modified = true;
                }
            }

            //Check for modified document
            if (modified)
            {
                try
                {
                    //Save the Rewrite config file with the new rewerite rule
                    document.Save(HttpContext.Current.Server.MapPath(DIR_PATH + language + ".xml"));

                    //No errors so the result is true
                    result = true;
                }
                catch (Exception e)
                {
                    //Log error message
                    string message = "Error at execute AddLanguageFileKey package action: " + e.Message;
                    Log.Add(LogTypes.Error, getUser(), -1, message);
                }
            }
            return result;
        }

        /// <summary>
        /// Return the area root node.
        /// When the area rootnode doesn't exist the area will be created
        /// </summary>
        /// <param name="area">The area.</param>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        private XmlNode EnsureAreaRootNode(string area, XmlDocument document)
        {
            XmlNode rootNode = document.SelectSingleNode(String.Format("//language/area[@alias = '{0}']", area));
            if (rootNode == null)
            {
                rootNode = CreateRootNode(area, document);
            }
            return rootNode;
        }

        /// <summary>
        /// Creates the area node.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        private XmlNode CreateRootNode(string area, XmlDocument document)
        {
            //Create the rootnode
            XmlNode node = document.CreateElement("area");
            //Append alias
            node.Attributes.Append(xmlHelper.addAttribute(document, "alias", area));

            //append the rootnode to xml
            document.DocumentElement.AppendChild(node);

            return node;
        }

        /// <summary>
        /// Removes the xmlData node from the language file
        /// </summary>
        /// <param name="packageName">Name of the package that we are un-installing</param>
        /// <param name="xmlData">The data that must be removed from the language file</param>
        /// <returns>True when succeeded</returns>
        public bool Undo(string packageName, System.Xml.XmlNode xmlData)
        {
            //Set result default to false
            bool result = false;

            //Get attribute values of xmlData
            string language, area, key;
            language = getAttributeDefault(xmlData, "language", "en");
            if (!getAttribute(xmlData, "area", out area)) return result;
            if (!getAttribute(xmlData, "key", out key)) return result;

            //Create a new xml document
            XmlDocument document = new XmlDocument();

            //Keep current indentions format
            document.PreserveWhitespace = true;

            //Load the web.config file into the xml document
            document.Load(HttpContext.Current.Server.MapPath(DIR_PATH + language + ".xml"));

            //Select root node in the web.config file for insert new nodes
            XmlNode rootNode = document.SelectSingleNode(String.Format("//language/area[@alias = '{0}']", area));

            //Check for rootNode exists
            if (rootNode == null) return result;

            //Set modified document default to false
            bool modified = false;

            //Look for existing nodes with same path of undo attribute
            if (rootNode.HasChildNodes)
            {
                //Look for existing add nodes with attribute path
                foreach (XmlNode existingNode in rootNode.SelectNodes(
                    String.Format("//key[@alias = '{0}']", key)))
                {
                    //Remove existing node from root node
                    rootNode.RemoveChild(existingNode);
                    modified = true;
                }
            }

            if (modified)
            {
                try
                {
                    //Save the Rewrite config file with the new rewerite rule
                    document.Save(HttpContext.Current.Server.MapPath(DIR_PATH + language + ".xml"));

                    //No errors so the result is true
                    result = true;
                }
                catch (Exception e)
                {
                    //Log error message
                    string message = "Error in the undo section of the AddLanguageFileKey package action: " + e.Message;
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
                string message = "Error at AddLanguageFileKey package action: "
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
        /// In this case a key called demo to be added to the template area
        /// </summary>
        /// <returns>The sample xml as node</returns>
        public XmlNode SampleXml()
        {
            return spa.helper.parseStringToXmlNode(
                "<Action runat=\"install\" undo=\"true/false\" alias=\"AddLanguageFileKey\" "
                    + "language=\"en/da/de/es/fr/it/nl/no/se/sv\" "
                    + "position=\"beginning/end\" "
                    + "area=\"template\" "
                    + "key=\"demo\" "
                    + "value=\"This is a demo string\" />"
            );
        }

        #endregion
    }
}

