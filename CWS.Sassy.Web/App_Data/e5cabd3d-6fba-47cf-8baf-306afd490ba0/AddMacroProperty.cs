using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using umbraco.interfaces;
using System.Xml;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.BusinessLogic;
using PackageActionsContrib.Helpers;

namespace PackageActionsContrib
{
    /// <summary>
    /// This Package action will Add a Macro propperty to the database
    /// </summary>
    /// <remarks>
    /// This package action is part of the PackageActionsContrib Project
    /// </remarks>
    class AddMacroProperty : IPackageAction
    {
        #region Implementation of IPackageAction

        public string Alias()
        {
            return "AddMacroProperty";
        }

        public XmlNode SampleXml()
        {
            const string sample = "<Action runat=\"install\" undo=\"true\" alias=\"AddMacroProperty\" macroPropertyTypeAlias=\"myPropertyTypeAlias\" macroPropertyTypeRenderAssembly=\"MyAssemblyName\" macroPropertyTypeRenderType=\"MyTypeName\" macroPropertyTypeBaseType=\"String\" />";
            return helper.parseStringToXmlNode(sample);
        }

        public bool Execute(string packageName, XmlNode xmlData)
        {
            try
            {
                MacroPropertyHelper macroPropertyHelper = GetMacroPropertyHelper(xmlData);
                AddMacropPropertyToDatabase(macroPropertyHelper);
                return true;
            }
            catch (Exception ex)
            {
                Log.Add(LogTypes.Error, -1,
                        string.Format("Error in AddMacroProperty Package action (Execute) for package {0} error:{1} ",
                                      packageName,
                                      ex));
                return false;
            }
        }

        public bool Undo(string packageName, XmlNode xmlData)
        {
            try
            {
                MacroPropertyHelper macroPropertyHelper = GetMacroPropertyHelper(xmlData);
                RemoveMacropPropertyFromDatabase(macroPropertyHelper);
                return true;
            }
            catch (Exception ex)
            {
                Log.Add(LogTypes.Error, -1,
                        string.Format("Error in AddMacroProperty Package action (Undo) for package {0} error:{1} ",
                                      packageName,
                                      ex));
                return false;
            }
        }

        #endregion

        #region Private

        private class MacroPropertyHelper
        {
            public string MacroPropertyTypeAlias { get; set; }
            public string MacroPropertyTypeRenderAssembly { get; set; }
            public string MacroPropertyTypeRenderType { get; set; }
            public string MacroPropertyTypeBaseType { get; set; }
        }

        private static MacroPropertyHelper GetMacroPropertyHelper(XmlNode xmlData)
        {
            var macroPropertyHelper = new MacroPropertyHelper
            {
                MacroPropertyTypeAlias = XmlHelper.GetAttributeValueFromNode(xmlData, "macroPropertyTypeAlias", string.Empty),
                MacroPropertyTypeRenderAssembly = XmlHelper.GetAttributeValueFromNode(xmlData, "macroPropertyTypeRenderAssembly", string.Empty),
                MacroPropertyTypeRenderType = XmlHelper.GetAttributeValueFromNode(xmlData, "macroPropertyTypeRenderType", string.Empty),
                MacroPropertyTypeBaseType = XmlHelper.GetAttributeValueFromNode(xmlData, "macroPropertyTypeBaseType", string.Empty)
            };

            bool missingMandatoryProperties = (
                string.IsNullOrEmpty(macroPropertyHelper.MacroPropertyTypeAlias) ||
                string.IsNullOrEmpty(macroPropertyHelper.MacroPropertyTypeBaseType) ||
                string.IsNullOrEmpty(macroPropertyHelper.MacroPropertyTypeRenderAssembly) ||
                string.IsNullOrEmpty(macroPropertyHelper.MacroPropertyTypeRenderType));

            if (missingMandatoryProperties)
                throw new ArgumentException("Check your package XML, one or more mandatory properties are missing.");

            return macroPropertyHelper;
        }

        private static void AddMacropPropertyToDatabase(MacroPropertyHelper macroPropertyHelper)
        {
            bool macroPropertyExists = CheckIfExists(macroPropertyHelper);

            if (macroPropertyExists)
                return;

            const string sql = "insert into cmsMacroPropertyType ( macroPropertyTypeAlias, macroPropertyTypeRenderAssembly, macroPropertyTypeRenderType, macroPropertyTypeBaseType ) values ( @macroPropertyTypeAlias, @macroPropertyTypeRenderAssembly, @macroPropertyTypeRenderType, @macroPropertyTypeBaseType )";

            Application.SqlHelper.ExecuteNonQuery(sql,
                Application.SqlHelper.CreateParameter("@macroPropertyTypeAlias", macroPropertyHelper.MacroPropertyTypeAlias),
                Application.SqlHelper.CreateParameter("@macroPropertyTypeRenderAssembly", macroPropertyHelper.MacroPropertyTypeRenderAssembly),
                Application.SqlHelper.CreateParameter("@macroPropertyTypeRenderType", macroPropertyHelper.MacroPropertyTypeRenderType),
                Application.SqlHelper.CreateParameter("@macroPropertyTypeBaseType", macroPropertyHelper.MacroPropertyTypeBaseType));
        }

        private static void RemoveMacropPropertyFromDatabase(MacroPropertyHelper macroPropertyHelper)
        {
            bool macroPropertyExists = CheckIfExists(macroPropertyHelper);

            if (!macroPropertyExists)
                return;

            const string sql = "delete from cmsMacroPropertyType where macroPropertyTypeAlias = @macroPropertyTypeAlias";

            Application.SqlHelper.ExecuteNonQuery(sql,
                Application.SqlHelper.CreateParameter("@macroPropertyTypeAlias", macroPropertyHelper.MacroPropertyTypeAlias));
        }

        private static bool CheckIfExists(MacroPropertyHelper macroPropertyHelper)
        {
            const string sql = "select count(*) from  cmsMacroPropertyType where  macroPropertyTypeAlias = @macroPropertyTypeAlias";

            var count = Application.SqlHelper.ExecuteScalar<int>(sql,
                Application.SqlHelper.CreateParameter("@macroPropertyTypeAlias", macroPropertyHelper.MacroPropertyTypeAlias));

            return count > 0;
        }


        #endregion

    }
}
