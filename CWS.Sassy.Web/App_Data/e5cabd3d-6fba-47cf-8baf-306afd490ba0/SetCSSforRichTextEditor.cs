using System;
using System.Xml;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.DataLayer;
using umbraco.interfaces;

namespace PackageActionsContrib
{

    /// <summary>
    /// This Package action will update a RichTextEditor dataType with an associated CSS file
    /// </summary>
    /// <remarks>
    /// This package action is part of the PackageActionsContrib Project
    /// </remarks>
    public class SetCSSforRichTextEditor : IPackageAction
    {
        /// <summary>
        /// Gets the SQL helper.
        /// </summary>
        /// <value>The SQL helper.</value>
        protected static ISqlHelper SqlHelper
        {
            get { return Application.SqlHelper; }
        }

        #region IPackageAction Members

        /// <summary>
        /// This Alias must be unique and is used as an identifier that must match the alias in the package action XML
        /// </summary>
        /// <returns>The Alias in string format</returns>
        public string Alias()
        {
            return "SetCSSforRichTextEditor";
        }

        /// <summary>
        /// Runs a SQL statement to update a Richtext editor for an associated CSS file.
        /// </summary>
        /// <param name="packageName">Name of the package that we install</param>
        /// <param name="xmlData"></param>
        /// <returns>True when succeeded</returns>
        public bool Execute(string packageName, System.Xml.XmlNode xmlData)
        {
            //Get the values from the package action
            string dataTypeName = xmlData.Attributes["dataTypeName"].Value;
            string cssName = xmlData.Attributes["cssName"].Value;

            //Setup variable sql
            string sql = string.Empty;

            //If dataTypeName AND cssName are NOT empty then...
            if (dataTypeName != String.Empty && cssName != String.Empty)
            {
               
                //Setup default values for ID's
                int cssID = 0;
                int datatypeID = 0;

                try
                {
                    //GET the ID of the CSS from the name of the CSS file
                    cssID = SqlHelper.ExecuteScalar<int>("select Id from UmbracoNode where Text = '" + cssName + "';");

                }
                catch
                {
                    umbraco.BusinessLogic.Log.Add(LogTypes.Error, -1, "couldn't get css");
                }
                
                try
                {
                    //GET the ID of the dataType from the name
                    datatypeID = SqlHelper.ExecuteScalar<int>("select Id from UmbracoNode where Text = '" + dataTypeName + "';");

                }
                catch
                {
                    umbraco.BusinessLogic.Log.Add(LogTypes.Error, -1, "couldn't get datatype");
                }

                /*
                =================================================
                EXAMPLE PreValue String
                =================================================                
                    ,code,undo,redo,cut,copy,mcepasteword,stylepicker,bold,italic,justifyleft,justifycenter,justifyright,bullist,numlist,outdent,indent,mcelink,unlink,mceinsertanchor,mceimage,umbracomacro,mceinserttable,
                    |1
                    |1,2,3,
                    |0
                    |500,500
                    |1042,
                    |True
                    |500|
                
                */

                /*
                =================================================
                The values for the PreValue string 
                =================================================
                    The buttons to be used for the editor
                    Boolean (0/1) to enable the ContextMenu
                    ID's for usergroups allowed access to advanced settings
                    Boolean (0/1) for full width editor
                    Related Stylesheets ID's
                    Boolean (0/1) for Show label of editor
                    Max Image Width value
                */

                //BUILD UP OUR SQL WE WANT TO EXCUTE
                sql = "Update cmsDataTypePreValues set value = ',code,undo,redo,cut,copy,mcepasteword,stylepicker,bold,italic,justifyleft,justifycenter,justifyright,bullist,numlist,outdent,indent,mcelink,unlink,mceinsertanchor,mceimage,umbracomacro,mceinserttable,|1|1,2,3,|0|500,500|" + cssID.ToString() + ",|True|500|' where DataTypeNodeId = " + datatypeID.ToString() + ";";


                //Run our SQL against the Umbraco DataLayer
                try
                {
                    //Run the SQL command
                    SqlHelper.ExecuteNonQuery(sql);

                    //No errors and everything worked OK, so the result is true
                    return true;
                }
                catch
                {
                    //IF it does not work return false
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Undos the Áction
        /// </summary>
        /// <param name="packageName">Name of the package that we install</param>
        /// <param name="xmlData"></param>
        /// <returns>True when succeeded</returns>
        /// <remarks>Undo on this action is not possible</remarks>
        public bool Undo(string packageName, System.Xml.XmlNode xmlData)
        {
            return true;
        }

        /// <summary>
        /// Returns a Sample XML Node 
        /// </summary>
        /// <returns>The sample xml as node</returns>
        public XmlNode SampleXml()
        {
            string sample = "<Action runat=\"install\" undo=\"true/false\" alias=\"SetCSSforRichTextEditor\" dataTypeName=\"yourDataTypeName\" cssName=\"yourCSSName\"/>";
            return helper.parseStringToXmlNode(sample);
        }
        #endregion

    }
}
