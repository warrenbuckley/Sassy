using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using umbraco.interfaces;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using System.Xml;
using System.Web;
using PackageActionsContrib.Helpers;
using umbraco.BusinessLogic;

namespace PackageActionsContrib
{
    public class AddXmlFragment : IPackageAction
    {
        public string Alias()
        {
            return "AddXmlFragment";
        }

        public bool Execute(string packageName, System.Xml.XmlNode xmlData)
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

            //The xml fragment we want to insert
            XmlNode xmlFragment = xmlData.SelectSingleNode("./*");

            //Select rootnode using the xpath
            XmlNode rootNode = configDocument.SelectSingleNode(xPath);

            if (position.Equals("beginning", StringComparison.CurrentCultureIgnoreCase))
            {
                //Add xml fragment to the beginning of the selected rootnode
                rootNode.PrependChild(configDocument.ImportNode(xmlFragment, true));
            }
            else
            {
                //add xml fragment to the end of the selected rootnode
                rootNode.AppendChild(configDocument.ImportNode(xmlFragment, true));
            }

            //Save the modified document
            configDocument.Save(HttpContext.Current.Server.MapPath(configFileName));

            result = true;

            return result;
        }

        public System.Xml.XmlNode SampleXml()
        {
            string sample = "<Action runat=\"install\" undo=\"false\" alias=\"AddXmlFragment\" file=\"~/config/umbracosettings.config\" xpath=\"//help\" position=\"end\"><link application=\"content\" applicationUrl=\"dashboard.aspx\"  language=\"en\" userType=\"Administrators\" helpUrl=\"http://www.xyz.no?{0}/{1}/{2}/{3}\" /></Action>";
            return helper.parseStringToXmlNode(sample);
        }

        /// <summary>
        /// User RemoveXMLFragment action to uninstall.
        /// </summary>
        /// <param name="packageName">Name of the package.</param>
        /// <param name="xmlData">The XML data.</param>
        /// <returns></returns>
        public bool Undo(string packageName, System.Xml.XmlNode xmlData)
        {
            return false;
        }
    }
}
