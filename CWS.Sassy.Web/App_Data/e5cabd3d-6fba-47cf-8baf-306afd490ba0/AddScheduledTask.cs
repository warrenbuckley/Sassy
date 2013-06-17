using System;
using System.Collections.Generic;
using System.Text;
using umbraco.interfaces;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using System.Web;
using System.Xml;
using umbraco.BusinessLogic;
using PackageActionsContrib.Helpers;

namespace PackageActionsContrib
{
    /// <summary>
    /// Adds a scheduled task to the scheduled tasks section of the UmbracoSettings.config file
    /// </summary>
    public class AddScheduledTask : IPackageAction
    {

        #region IPackageAction Members

        public string Alias()
        {
            return "AddScheduledTask";
        }

        public bool Execute(string packageName, System.Xml.XmlNode xmlData)
        {
            bool result = false;

            //Get attribute values
            string log = XmlHelper.GetAttributeValueFromNode(xmlData, "log").ToLower() == "true" ? "true" : "false";
            string scheduledTaskAlias = XmlHelper.GetAttributeValueFromNode(xmlData, "scheduledTaskAlias");
            string interval = XmlHelper.GetAttributeValueFromNode(xmlData, "interval");
            string url = ParseUrl(XmlHelper.GetAttributeValueFromNode(xmlData, "url"));

            //Open the Umbraco Settings config file
            XmlDocument umbracoSettingsFile = umbraco.xmlHelper.OpenAsXmlDocument("/config/umbracoSettings.config");

            //Select scheduled tasks node from the settings file
            XmlNode scheduledTaskRootNode = umbracoSettingsFile.SelectSingleNode("//scheduledTasks");

            //Create a new scheduled task node 
            XmlNode scheduledTaskNode = (XmlNode)umbracoSettingsFile.CreateElement("task");
            
            //Append addributes
            scheduledTaskNode.Attributes.Append(umbraco.xmlHelper.addAttribute(umbracoSettingsFile, "log", log));
            scheduledTaskNode.Attributes.Append(umbraco.xmlHelper.addAttribute(umbracoSettingsFile, "alias", scheduledTaskAlias));
            scheduledTaskNode.Attributes.Append(umbraco.xmlHelper.addAttribute(umbracoSettingsFile, "interval", interval));
            scheduledTaskNode.Attributes.Append(umbraco.xmlHelper.addAttribute(umbracoSettingsFile, "url", url));


            //Append the new rewrite scheduled task to the Umbraco Settings config file
            scheduledTaskRootNode.AppendChild(scheduledTaskNode);

            //Save the Umbraco Settings config file with the new Scheduled task
            umbracoSettingsFile.Save(System.Web.HttpContext.Current.Server.MapPath("/config/umbracoSettings.config"));

            //No errors so the result is true
            result = true;

            return result;
        }

        /// <summary>
        /// Sample xml
        /// </summary>
        public System.Xml.XmlNode SampleXml()
        {
            string sample = "<Action runat=\"install\" undo=\"true/false\" alias=\"AddScheduledTask\" scheduledTaskAlias=\"myscheduledtask\" log=\"true\" interval=\"60\"  url=\"/myscheduledpage.aspx\"></Action>";
            return helper.parseStringToXmlNode(sample);
        }

        /// <summary>
        /// Removes the scheduled task from the UmbracoSettings.config file
        /// </summary>
        public bool Undo(string packageName, System.Xml.XmlNode xmlData)
        {
            bool result = false;
            //Get alias to remove
            string scheduledTaskAlias = XmlHelper.GetAttributeValueFromNode(xmlData, "scheduledTaskAlias");

            //Open the Umbraco Settings config file
            XmlDocument umbracoSettingsFile = umbraco.xmlHelper.OpenAsXmlDocument("/config/umbracoSettings.config");

            //Select scheduled tasks root node from the settings file
            XmlNode scheduledTaskRootNode = umbracoSettingsFile.SelectSingleNode("//scheduledTasks");

            //Get the child node with the scheduled task we want to remove
            //Select the url rewrite rule by name from the config file
            XmlNode scheduledTaskNode = scheduledTaskRootNode.SelectSingleNode("//task[@alias = '" + scheduledTaskAlias + "']");
            
            if (scheduledTaskNode != null)
            {
                //Child node is not null, remove it
                scheduledTaskRootNode.RemoveChild(scheduledTaskNode);

                //Save the modified configuration file
                umbracoSettingsFile.Save(System.Web.HttpContext.Current.Server.MapPath("/config/umbracoSettings.config"));
            }
            
            return result;
        }

        #endregion

        #region Helper Methods
        /// <summary>
        /// Takes the url determines if its a virtual url, if so convert it to an absolute url
        /// </summary>
        /// <param name="url">The url to parse</param>
        private string ParseUrl(string url)
        {
            try
            {
                if (url.StartsWith("~"))
                {
                    // Not allowed skip it
                    url = url.Substring(1);
                }
                if (url.StartsWith("/"))
                {
                    UriBuilder u = new UriBuilder();
                    u.Host = HttpContext.Current.Request.Url.Host;
                    u.Path = url;

                    url = u.Uri.ToString();
                }
            }
            catch
            {
                Log.Add(LogTypes.Error, -1, "Error parsing the url for AddScheduledTask package action ");
            }
            //Return the url
            return url;
        }
        #endregion

    }
}
