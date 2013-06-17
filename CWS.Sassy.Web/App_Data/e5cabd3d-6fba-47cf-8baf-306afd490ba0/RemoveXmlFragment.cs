using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using umbraco.interfaces;
using System.Web;
using PackageActionsContrib.Helpers;
using System.Xml;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.packager.standardPackageActions;

namespace PackageActionsContrib
{
    public class RemoveXmlFragment : IPackageAction
    {
        public string Alias()
        {
            return "RemoveXmlFragment";
        }

        /// <summary>
        /// When you want to add a fragment use AddXmlFragment
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="xmlData"></param>
        /// <returns></returns>
        public bool Execute(string packageName, System.Xml.XmlNode xmlData)
        {
            return false;
        }

        public System.Xml.XmlNode SampleXml()
        {
            string sample = "<Action runat=\"uninstall\" alias=\"RemoveXmlFragment\" file=\"~/config/umbracosettings.config\" xpath=\"//help/link[@application='content']\" />";
            return helper.parseStringToXmlNode(sample);
        }

        public bool Undo(string packageName, System.Xml.XmlNode xmlData)
        {
            //Set result default to false
            bool result = false;

            //The config file we want to modify
            string configFileName = VirtualPathUtility.ToAbsolute(XmlHelper.GetAttributeValueFromNode(xmlData, "file"));

            //Xpath expression to determine the rootnode
            string xPath = XmlHelper.GetAttributeValueFromNode(xmlData, "xpath");

            //Holds the position where we want to insert the xml Fragment
            string position = XmlHelper.GetAttributeValueFromNode(xmlData, "position", "end");

            //Open the config file
            XmlDocument configDocument = umbraco.xmlHelper.OpenAsXmlDocument(configFileName);

            //Select the node to remove using the xpath
            XmlNode node = configDocument.SelectSingleNode(xPath);

            //Remove the node 
            if (node != null)
            {
                node.ParentNode.RemoveChild(node);
            }

            //Save the modified document
            configDocument.Save(HttpContext.Current.Server.MapPath(configFileName));

            result = true;

            return result;
        }
    }
}
