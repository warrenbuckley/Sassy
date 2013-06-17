using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using umbraco.interfaces;
using System.Web;
using PackageActionsContrib.Helpers;
using System.Xml;
using umbraco.cms.businesslogic.packager.standardPackageActions;

namespace PackageActionsContrib
{
    public class SetAttributeValue : IPackageAction
    {
        public string Alias()
        {
            return "SetAttributeValue";
        }

        public bool Execute(string packageName, System.Xml.XmlNode xmlData)
        {
            //Set result default to false
            bool result = false;


            //The config file we want to modify
            string configFileName = VirtualPathUtility.ToAbsolute(XmlHelper.GetAttributeValueFromNode(xmlData, "file"));

            //Xpath expression to determine the rootnode
            string xPath = XmlHelper.GetAttributeValueFromNode(xmlData, "xpath");

            //Xpath expression to determine the attributeName we want to select
            string attributeName = XmlHelper.GetAttributeValueFromNode(xmlData, "attributeName");

            //Xpath expression to determine the attributeValue we want to select
            string attributeValue = XmlHelper.GetAttributeValueFromNode(xmlData, "attributeValue");

            //Open the config file
            XmlDocument configDocument = umbraco.xmlHelper.OpenAsXmlDocument(configFileName);

            //Select rootnode using the xpath
            XmlNode rootNode = configDocument.SelectSingleNode(xPath);

            //If the rootnode != null continue
            if (rootNode != null)
            {
                if (rootNode.Attributes[attributeName] == null)
                {
                    //Attribute doesn't exists, create it and set the attribute value
                    XmlAttribute att = umbraco.xmlHelper.addAttribute(configDocument, attributeName, attributeValue);
                    rootNode.Attributes.Append(att);
                }
                else
                {
                    //Set attribute value
                    rootNode.Attributes[attributeName].Value = attributeValue;
                }

                //Save the modified document
                configDocument.Save(HttpContext.Current.Server.MapPath(configFileName));
                result = true;
            }
            return result;
        }

        public System.Xml.XmlNode SampleXml()
        {
            string sample = "<Action runat=\"install\" undo=\"false\" alias=\"SetAttributeValue\" file=\"~/web.config\" xpath=\"//system.webServer/modules\" attributeName=\"runAllManagedModulesForAllRequests\" attributeValue=\"true\"/>";
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
