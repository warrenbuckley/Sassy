using System;
using System.IO;
using System.Web;
using System.Xml;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.interfaces;

namespace PackageActionsContrib
{
    /// <summary>
    /// [Ed] Clears the client dependancy cache allowing items such as the section name to be updated.
    /// </summary>
    public class ClearClientDependacyCache : IPackageAction
    {
        public bool Execute(string packageName, XmlNode xmlData)
        {
            try
            {
                string cacheDirectory = HttpContext.Current.Server.MapPath("/App_Data/TEMP/ClientDependency/");
                Directory.Delete(cacheDirectory);
            }
            catch (Exception ex)
            {
                Log.Add(LogTypes.PackagerInstall, -1, "Unable to clear the client dependency cache: " + ex.Message);
                return false;
            }
            return true;
        }

        public string Alias()
        {
            return "ClearClientDependencyCache";
        }

        public bool Undo(string packageName, XmlNode xmlData)
        {
            return true;
        }

        public XmlNode SampleXml()
        {
            return helper.parseStringToXmlNode(string.Format("<Action runat=\"install\" alias=\"{0}\"/>", Alias()));
        }
    }
}
