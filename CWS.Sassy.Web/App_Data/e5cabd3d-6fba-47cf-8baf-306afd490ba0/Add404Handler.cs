using System.Xml;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.interfaces;
using System.Web;

namespace PackageActionsContrib
{
    /// <summary>
    /// This Package action will add a new entry to the 404handlers.config file
    /// </summary>
    /// <remarks>
    /// This package action is part of the PackageActionsContrib Project
    /// </remarks>
    public class Add404Handler : IPackageAction
    {
        #region IPackageAction Members

        /// <summary>
        /// This Alias must be unique and is used as an identifier that must match the alias in the package action XML
        /// </summary>
        /// <returns>The Alias in string format</returns>
        public string Alias()
        {
            return "Add404Handler";
        }

        /// <summary>
        /// Appends the xmlData Node to the 404handlers.config file
        /// </summary>
        /// <param name="packageName">Name of the package that we install</param>
        /// <param name="xmlData">The data that must be appended to the 404handlers.config file</param>
        /// <returns>True when succeeded</returns>
        public bool Execute(string packageName, System.Xml.XmlNode xmlData)
        {
            //Set result default to false
            bool result = false;

            //Open the 404 handlers config file
            XmlDocument handlerFile = umbraco.xmlHelper.OpenAsXmlDocument(VirtualPathUtility.ToAbsolute("~/config/404handlers.config"));

            //Select notfound node in the config file
            XmlNode handlerRootNode = handlerFile.SelectSingleNode("//NotFoundHandlers");

            //Create a new handler node
            XmlNode newHandlerNode = (XmlNode)handlerFile.CreateElement("notFound");

            //Copy some attributes from the package xml to the new handler node
            foreach (XmlAttribute att in xmlData.SelectSingleNode("//Action[@alias='" + Alias() + "']").Attributes)
            {
                if (att.Name == "assembly" || att.Name == "type")
                {
                    XmlAttribute attribute = handlerFile.CreateAttribute(att.Name);
                    attribute.Value = att.Value;
                    newHandlerNode.Attributes.Append(attribute);
                }
            }

            // Append the new handler node to the 404handlers config file before the 'handle404' entry
            handlerRootNode.InsertBefore(newHandlerNode, handlerRootNode.SelectSingleNode("//notFound[@type = 'handle404']"));

            //Save the config file
            handlerFile.Save(System.Web.HttpContext.Current.Server.MapPath(VirtualPathUtility.ToAbsolute("/config/404handlers.config")));

            //No errors so the result is true
            result = true;

            return result;
        }

        /// <summary>
        /// Removes the xmlData Node from the 404handlers.config file based on the rulename 
        /// </summary>
        /// <param name="packageName">Name of the package that we install</param>
        /// <param name="xmlData">The data</param>
        /// <returns>True when succeeded</returns>
        public bool Undo(string packageName, System.Xml.XmlNode xmlData)
        {
            bool result = false;

            //Get the properties of the handler
            XmlNode node = xmlData.SelectSingleNode("//Action[@alias='" + Alias() + "']");
            string assembly = node.Attributes["assembly"].Value;
            string type = node.Attributes["type"].Value;

            //Open the 404handlers config file
            XmlDocument rewriteFile = umbraco.xmlHelper.OpenAsXmlDocument(VirtualPathUtility.ToAbsolute("/config/404handlers.config"));

            //Select the rootnode where we want to delete from
            XmlNode handlersRootNode = rewriteFile.SelectSingleNode("//NotFoundHandlers");

            //Select the handler node by attributes from the config file
            XmlNode handlerEntry = handlersRootNode.SelectSingleNode("//notFound[@assembly = '" + assembly + "' and @type = '" + type + "']");
            if (handlerEntry != null)
            {
                //Node is found, remove it from the xml document
                handlersRootNode.RemoveChild(handlerEntry);
                //Save the modified configuration file
                rewriteFile.Save(System.Web.HttpContext.Current.Server.MapPath(VirtualPathUtility.ToAbsolute("/config/404handlers.config")));
            }
            result = true;

            return result;
        }

        /// <summary>
        /// Returns a Sample XML Node
        /// </summary>
        /// <returns>The sample xml as node</returns>
        public XmlNode SampleXml()
        {
            string sample = "<Action runat=\"install\" undo=\"true/false\" alias=\"Add404Handler\" assembly=\"assembly\" type=\"type\" />";
            return helper.parseStringToXmlNode(sample);
        }

        #endregion

    }
}
