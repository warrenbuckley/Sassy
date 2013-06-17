using System.Xml;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.interfaces;
using System.Web;

namespace PackageActionsContrib
{
    /// <summary>
    /// This Package action will add a new entry to the ExamineIndex.config file
    /// </summary>
    /// <remarks>
    /// I hope this package action will be part of the PackageActionsContrib Project
    /// </remarks>
    public class AddExamineIndex : IPackageAction
    {
        #region IPackageAction Members

        /// <summary>
        /// This Alias must be unique and is used as an identifier that must match the alias in the package action XML
        /// </summary>
        /// <returns>The Alias in string format</returns>
        public string Alias()
        {
            return "AddExamineIndex";
        }

        /// <summary>
        /// Appends the xmlData  to the ExamineIndex.config file
        /// </summary>
        /// <param name="packageName">Name of the package that we install</param>
        /// <param name="xmlData">The data that must be appended to the ExamineIndex.config file</param>
        /// <returns>True when succeeded</returns>
        public bool Execute(string packageName, XmlNode xmlData)
        {
            // Set result default to false
            bool result = false;

            // Check if the xmlData has a childnode (the IndexSet rule node)
            if (xmlData.HasChildNodes)
            {
                // Open the examine index file
                XmlDocument examineIndexFile = umbraco.xmlHelper.OpenAsXmlDocument(VirtualPathUtility.ToAbsolute("~/config/ExamineIndex.config"));

                // Select ExamineLuceneIndexSets node in the config file
                XmlNode examineLuceneIndexSetsNode = examineIndexFile.SelectSingleNode("//ExamineLuceneIndexSets");

                // Select IndexSet from the supplied xmlData
                XmlNode indexSetNode = xmlData.SelectSingleNode("./IndexSet");

                // Add the node
                var newNode = examineLuceneIndexSetsNode.OwnerDocument.ImportNode(indexSetNode, true);
                examineLuceneIndexSetsNode.AppendChild(newNode);

                // Save the config file
                examineIndexFile.Save(HttpContext.Current.Server.MapPath(VirtualPathUtility.ToAbsolute("/config/ExamineIndex.config")));

                // No errors so the result is true
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Removes the xmlData Node from the ExamineIndex.config file based on the rulename 
        /// </summary>
        /// <param name="packageName">Name of the package that we install</param>
        /// <param name="xmlData">The data</param>
        /// <returns>True when succeeded</returns>
        public bool Undo(string packageName, XmlNode xmlData)
        {
            bool result = false;

            // Check if the xmlData has a childnode (the IndexSet rule node)
            if (xmlData.HasChildNodes)
            {
                // Open the examine index file
                XmlDocument examineIndexFile = umbraco.xmlHelper.OpenAsXmlDocument(VirtualPathUtility.ToAbsolute("~/config/ExamineIndex.config"));

                // Select ExamineLuceneIndexSets node in the config file
                XmlNode examineLuceneIndexSetsNode = examineIndexFile.SelectSingleNode("//ExamineLuceneIndexSets");

                // Select IndexSet from the supplied xmlData
                XmlNode indexSetNode = xmlData.SelectSingleNode("//IndexSet");

                // Get the index name
                string indexName = indexSetNode.Attributes["SetName"].Value;

                // Select the node by name from the config file
                XmlNode index = examineLuceneIndexSetsNode.SelectSingleNode("//IndexSet[@SetName = '" + indexName + "']");
                if (index != null)
                {
                    // Index is found, remove it from the xml document
                    examineLuceneIndexSetsNode.RemoveChild(index);

                    //Save the modified configuration file
                    examineIndexFile.Save(HttpContext.Current.Server.MapPath("/config/ExamineIndex.config"));
                }

                result = true;
            }
            return result;
        }

        /// <summary>
        /// Returns a Sample XML Node
        /// </summary>
        /// <returns>The sample xml as node</returns>
        public XmlNode SampleXml()
        {
            string sample =
                "<Action runat=\"install\" undo=\"true\" alias=\"AddExamineIndex\">" +
                    "<IndexSet SetName=\"BootstrapENIndexSet\" IndexPath=\"~/App_Data/TEMP/ExamineIndexes/BootstrapENIndexSet/\" IndexParentId=\"1284\">" +
                    "<IndexAttributeFields>" +
                      "<add Name=\"id\" />" +
                      "<add Name=\"nodeName\" />" +
                      "<add Name=\"updateDate\" />" +
                      "<add Name=\"writerName\" />" +
                      "<add Name=\"path\" />" +
                      "<add Name=\"nodeTypeAlias\" />" +
                      "<add Name=\"parentID\" />" +
                    "</IndexAttributeFields>" +
                    "<IndexUserFields>" +
                      "<add Name=\"headerText\" />" +
                      "<add Name=\"bodyText\" />" +
                    "</IndexUserFields>" +
                    "<IncludeNodeTypes>" +
                      "<add Name=\"Homepage\" />" +
                      "<add Name=\"Textpage\" />" +
                      "<add Name=\"Newspage\" />" +
                    "</IncludeNodeTypes>" +
                    "<ExcludeNodeTypes />" +
                  "</IndexSet>" +
                "</Action>";

            return helper.parseStringToXmlNode(sample);
        }

        #endregion

    }
}