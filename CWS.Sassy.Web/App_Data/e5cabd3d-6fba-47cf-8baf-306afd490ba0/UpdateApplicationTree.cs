using System;
using System.Xml;
using PackageActionsContrib.Helpers;
using umbraco.BusinessLogic;
using umbraco.interfaces;
using spa = umbraco.cms.businesslogic.packager.standardPackageActions;

namespace PackageActionsContrib
{
    public class UpdateApplicationTree : IPackageAction
    {
        /// <summary>
        /// This Alias must be unique and is used as an identifier that must match 
        /// the alias in the package action XML
        /// </summary>
        /// <returns>The Alias in string format</returns>
        public string Alias()
        {
            return "UpdateAppTree";
        }

        /// <summary>
        /// Returns a Sample XML Node 
        /// </summary>
        /// <returns>The sample xml as node</returns>
        public XmlNode SampleXml()
        {
            //TODO: Add Sample XML
            return spa.helper.parseStringToXmlNode(
                @"<Action runat=""uninstall"" " + Environment.NewLine +
                @"    undo=""false"" " + Environment.NewLine +
                @"    alias=""UpdateAppTree"" " + Environment.NewLine +
                @"    treeAlias=""media"" " + Environment.NewLine +
                @"    treeHandlerAssembly=""umbraco"" " + Environment.NewLine +
                @"    treeHandlerType=""loadMedia"" />"
            );
        }

        /// <summary>
        /// Execute the sql action required to update the appTree
        /// </summary>
        public bool Execute(string packageName, System.Xml.XmlNode xmlData)
        {
            try
            {
                string sql = "UPDATE umbracoAppTree SET treeHandlerAssembly = '" + XmlHelper.GetAttributeValueFromNode(xmlData, "treeHandlerAssembly") + "', treeHandlerType='" + XmlHelper.GetAttributeValueFromNode(xmlData, "treeHandlerType") + "' WHERE treeAlias='" + XmlHelper.GetAttributeValueFromNode(xmlData, "treeAlias") + "'";
                Application.SqlHelper.ExecuteNonQuery(sql);

                //Everything okay return true
                return true;
            }
            catch (Exception ex)
            {
                Log.Add(LogTypes.Error, -1, string.Format("Error in UpdateAppTree Package action for package {0} error:{1} ", packageName, ex.ToString()));
            }

            return false;
        }

        /// <summary>
        /// Executes the action on uninstall
        /// </summary>
        public bool Undo(string packageName, System.Xml.XmlNode xmlData)
        {
            return Execute(packageName, xmlData);
        }
    }
}
