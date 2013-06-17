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
    /// This Package action will Add a new Mime Type to the web.config file
    /// </summary>
    /// <remarks>
    /// This package action is part of the PackageActionsContrib Project
    /// </remarks>
    public class AddMimeType : IPackageAction
    {
        //Set the web.config full path
        const string FULL_PATH = "/web.config";

        #region IPackageAction Members

        /// <summary>
        /// This Alias must be unique and is used as an identifier that must match 
        /// the alias in the package action XML
        /// </summary>
        /// <returns>The Alias in string format</returns>
        public string Alias()
        {
            return "AddMimeType";
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
            string position, extension, mimetype;
            position = getAttributeDefault(xmlData, "position", null);
            if (!getAttribute(xmlData, "extension", out extension)) return result;
            if (!getAttribute(xmlData, "mimetype", out mimetype)) return result;

            //Create a new xml document
            XmlDocument document = new XmlDocument();

            //Keep current indentions format
            document.PreserveWhitespace = true;

            //Load the web.config file into the xml document
            document.Load(HttpContext.Current.Server.MapPath(FULL_PATH));
            //Select root node in the web.config file for insert new nodes
            XmlNode rootNode = document.SelectSingleNode("//configuration/system.webServer");

            //Check for rootNode exists
            if (rootNode == null) return result;

            //Set modified document default to false
            bool modified = false;

            //Set insert node default true
            bool insertNode = true;

            //Check for staticContent node
            if (rootNode.SelectSingleNode("staticContent") != null)
            {
                //Replace root node
                rootNode = rootNode.SelectSingleNode("staticContent");

                //Look for existing nodes with same path like the new node
                if (rootNode.HasChildNodes)
                {
                    //Look for existing nodeType nodes
                    XmlNode node = rootNode.SelectSingleNode(
                        String.Format("//mimeMap[@fileExtension = '{0}' and @mimeType = '{1}']", extension, mimetype));

                    //If path already exists 
                    if (node != null)
                    {
                        //Cancel insert node operation
                        insertNode = false;
                    }
                }
            }
            else
            {
                //Create staticContent node
                var staticContentNode = document.CreateElement("staticContent");
                rootNode.AppendChild(staticContentNode);

                //Replace root node
                rootNode = staticContentNode;

                //Mark document modified
                modified = true;
            }

            //Check for insert flag
            if (insertNode)
            {
                //Create new node with attributes
                XmlNode newNode = document.CreateElement("mimeMap");
                newNode.Attributes.Append(
                    xmlHelper.addAttribute(document, "fileExtension", extension));
                newNode.Attributes.Append(
                    xmlHelper.addAttribute(document, "mimeType", mimetype));

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

            //Check for modified document
            if (modified)
            {
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
                    string message = "Error at execute AddMimeType package action: " + e.Message;
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
            string extension, mimetype;
            if (!getAttribute(xmlData, "extension", out extension)) return result;
            if (!getAttribute(xmlData, "mimetype", out mimetype)) return result;

            //Create a new xml document
            XmlDocument document = new XmlDocument();

            //Keep current indentions format
            document.PreserveWhitespace = true;

            //Load the web.config file into the xml document
            document.Load(HttpContext.Current.Server.MapPath(FULL_PATH));

            //Select root node in the web.config file for insert new nodes
            XmlNode rootNode = document.SelectSingleNode("//configuration/system.webServer/staticContent");

            //Check for rootNode exists
            if (rootNode == null) return result;

            //Set modified document default to false
            bool modified = false;

            //Look for existing nodes with same path of undo attribute
            if (rootNode.HasChildNodes)
            {
                //Look for existing add nodes with attribute path
                foreach (XmlNode existingNode in rootNode.SelectNodes(
                    String.Format("//mimeMap[@fileExtension = '{0}' and @mimeType = '{1}']", extension, mimetype)))
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
                    document.Save(HttpContext.Current.Server.MapPath(FULL_PATH));

                    //No errors so the result is true
                    result = true;
                }
                catch (Exception e)
                {
                    //Log error message
                    string message = "Error at undo AddMimeType package action: " + e.Message;
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
                string message = "Error at AddMimeType package action: "
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
                "<Action runat=\"install\" undo=\"true/false\" alias=\"AddMimeType\" "
                    + "position=\"beginning/end\" "
                    + "extension=\".txt\" "
                    + "mimetype=\"text/plain\" />"
            );
        }

        #endregion
    }
}
