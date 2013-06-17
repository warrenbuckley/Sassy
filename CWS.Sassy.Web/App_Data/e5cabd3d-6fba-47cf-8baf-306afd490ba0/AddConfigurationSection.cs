using System;
using System.Configuration;
using System.Reflection;
using System.Web.Configuration;
using System.Xml;

using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.interfaces;
using umbraco.BusinessLogic;

namespace PackageActionsContrib
{
    /// <summary>
    /// Adds a configuration section to the web.config
    /// Custom type must derive from <seealso cref="ConfigurationSection"/>
    /// </summary>
    public class AddConfigurationSection : IPackageAction
    {
        #region IPackageAction Members

        public bool Execute(string packageName, XmlNode xmlData)
        {

            try
            {
                var config = WebConfigurationManager.OpenWebConfiguration("~");
                var sectionName = xmlData.SelectSingleNode("//Section").Attributes["name"].Value;

                if (config.Sections[sectionName] == null)
                {
                    var assemblyName = xmlData.SelectSingleNode("//Section").Attributes["assembly"].Value;
                    var typeName = xmlData.SelectSingleNode("//Section").Attributes["type"].Value;
                    var assembly = Assembly.Load(assemblyName);

                    if (assembly == null) return false;

                    var configSection = assembly.CreateInstance(typeName) as ConfigurationSection;

                    if (configSection == null) return false;

                    config.Sections.Add(sectionName, configSection);
                    configSection.SectionInformation.ForceSave = true;
                    config.Save(ConfigurationSaveMode.Full);
                }

                return true;
            }
            catch (Exception e)
            {
                string message = "Error at execute AddConfigurationSection package action: " + e.Message;
                Log.Add(LogTypes.Error,  -1, message);
                return false;
            }
        }

        public string Alias()
        {
            return "AddConfigurationSection";
        }

        public bool Undo(string packageName, XmlNode xmlData)
        {
            try
            {
                var config = WebConfigurationManager.OpenWebConfiguration("~");
                var sectionName = xmlData.SelectSingleNode("//Section").Attributes["name"].Value;

                if (config.Sections[sectionName] != null)
                {

                    config.Sections.Remove(sectionName);
                    config.Save(ConfigurationSaveMode.Full);
                }
                return true;
            }
            catch
            {

                return false;
            }
        }

        public XmlNode SampleXml()
        {
            var sample = "<Action runat=\"install\" undo=\"true\" alias=\"AddConfigurationSection\"><Section name=\"\" assembly=\"\" type=\"\" /></Action>";
            return helper.parseStringToXmlNode(sample);
        }

        #endregion
    }
}
