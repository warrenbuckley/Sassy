using System;
using System.Collections.Generic;
using System.Text;
using umbraco.interfaces;
using System.Xml;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using System.Configuration;
using PackageActionsContrib.Helpers;

namespace PackageActionsContrib
{

    /// <summary>
    /// Adds a key to the web.config app settings
    /// </summary>
    /// <remarks>Contribution from Paul Sterling</remarks>
    public class AddAppConfigKey : IPackageAction
    {
        #region IPackageAction Members

        public string Alias()
        {
            return "AddAppConfigKey";
        }

        public bool Execute(string packageName, XmlNode xmlData)
        {
            string addKey = string.Empty;
            string addValue = string.Empty;

            try
            {
                addKey = XmlHelper.GetAttributeValueFromNode(xmlData, "key");
                addValue = XmlHelper.GetAttributeValueFromNode(xmlData, "value");
               
                // as long as addKey has a value, create the key entry in web.config
                if (addKey != string.Empty)
                    CreateAppSettingsKey(addKey, addValue);

                return true;
            }
            catch
            {
                return false;
            }

        }

        public XmlNode SampleXml()
        {
            string sample = "<Action runat=\"install\" undo=\"true/false\" alias=\"AddAppConfigKey\" key=\"your key\" value=\"your value\"></Action>";
            return helper.parseStringToXmlNode(sample);
        }

        public bool Undo(string packageName, XmlNode xmlData)
        {
            string addKey = string.Empty;

            try
            {
                addKey = XmlHelper.GetAttributeValueFromNode(xmlData, "key");
                
                // as long as addKey has a value, remove it from the key entry in web.config
                if (addKey != string.Empty)
                    RemoveAppSettingsKey(addKey);

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region helpers
        private void CreateAppSettingsKey(string key, string value)
        {
            System.Configuration.Configuration config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            AppSettingsSection AppSettings = (AppSettingsSection)config.GetSection("appSettings");

            AppSettings.Settings.Add(key, value);

            config.Save(ConfigurationSaveMode.Modified);
        }

        private void RemoveAppSettingsKey(string key)
        {
            System.Configuration.Configuration config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            AppSettingsSection AppSettings = (AppSettingsSection)config.GetSection("appSettings");

            AppSettings.Settings.Remove(key);

            config.Save(ConfigurationSaveMode.Modified);
        }
        #endregion
    }
}
