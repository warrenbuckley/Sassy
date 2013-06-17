using System;
using System.Collections.Generic;
using System.Text;
using umbraco.interfaces;
using System.Xml;
using System.IO;
using System.Web;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.packager.standardPackageActions;

namespace PackageActionsContrib
{
    public class ExecuteSql : IPackageAction
    {
        #region IPackageAction Members

        public string Alias()
        {
            return "ExecuteSql";
        }

        /// <summary>
        /// Executes the sql action
        /// </summary>
        public bool Execute(string packageName, System.Xml.XmlNode xmlData)
        {
            bool result = false;
            try
            {
                //Get SQl From xmlData object
                string sql = GetSqlFromXmlNode(xmlData);
                Application.SqlHelper.ExecuteNonQuery(sql);

                //SQl Executed check if file needs to be deleted and if so delete it
                CheckAndDeleteSqlFile(xmlData);

                //Everything okay return true
                result = true;
            }
            catch(Exception ex)
            {
                Log.Add(LogTypes.Error,-1,string.Format("Error in Execute SQL Package action for package {0} error:{1} ",packageName,ex.ToString()));
            }
            return result;
        }

        public System.Xml.XmlNode SampleXml()
        {
            string sample = "<Action runat=\"install\" undo=\"false\" alias=\"ExecuteSql\"><![CDATA[CREATE TABLE tmp (	[ClientCategoryId] [int] IDENTITY(1,1) NOT NULL)]]></Action>";
            return helper.parseStringToXmlNode(sample);
        }

        /// <summary>
        /// When you want to Undo an Sql Install create a new action that only runs at UnInstall 
        /// </summary>
        public bool Undo(string packageName, System.Xml.XmlNode xmlData)
        {
            XmlNode uninstallNode = xmlData.SelectSingleNode("//Action[@runat='uninstall' and @alias='" + Alias() + "']");
            if (uninstallNode != null)
                return Execute(packageName, uninstallNode);
            return false;
        }


        #endregion

        /// <summary>
        /// Returns sql from the Node 
        /// This can be a reference to a sql file or inline SQL
        /// </summary>
        private string GetSqlFromXmlNode(XmlNode xmlData)
        {
            string sql = string.Empty;
            string fileLocation = GetFileLocationFromAttribute(xmlData);

            if (!string.IsNullOrEmpty(fileLocation))
            {
                //Read sql file
                sql = File.ReadAllText(fileLocation);
            }
            else
            {
                //Use inline sql
                sql = xmlData.InnerText;
            }
            return sql;
        }

        /// <summary>
        /// Checks if the deletefile attribute = true 
        /// If so deletes the sql file
        /// </summary>
        private void CheckAndDeleteSqlFile(XmlNode xmlData)
        {
            string fileLocation = GetFileLocationFromAttribute(xmlData);
            if ((!string.IsNullOrEmpty(fileLocation)) && AttributeToBool(xmlData.Attributes["deleteSqlFileAfterExecute"]))
            {
                //Specified that we must delete the file, so delete it
                try
                {
                    File.Delete(fileLocation);
                }
                catch (Exception ex)
                {
                    //Error during delete log it.
                    Log.Add(LogTypes.Error, -1,string.Format("Could not delete the file {0} reason: {1}",fileLocation,ex));
                }
            }
        }

        /// <summary>
        /// Returns the full path to the sql file (if specified)
        /// </summary>
        private string GetFileLocationFromAttribute(XmlNode xmlData)
        {
            string result = string.Empty;
            if (xmlData.Attributes["scriptfile"] != null)
            {
                result = HttpContext.Current.Server.MapPath(xmlData.Attributes["scriptfile"].InnerText);
            }
            return result;
        }

        /// <summary>
        /// Returns a boolean based on attribute value
        /// </summary>
        private bool AttributeToBool(XmlAttribute attribute)
        {
            return attribute != null && attribute.InnerText.ToLower() == "true";
        }
    }
}
