using System.Xml;
using umbraco;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.interfaces;
using umbraco.BusinessLogic;
using umbraco.BasePages;
using umbraco.cms.businesslogic.web;
using spa = umbraco.cms.businesslogic.packager.standardPackageActions;

namespace PackageActionsContrib
{
    /// <summary>
    /// This Package action will update a property on a page with the node id of a child page.. Can be used for redirects after sending email or similar. 
    /// </summary>
    /// <remarks>
    /// This package action is part of the PackageActionsContrib Project
    /// </remarks>
    public class UpdateNodeIdProperty : IPackageAction
    {

        #region IPackageAction Members

        /// <summary>
        /// This Alias must be unique and is used as an identifier that must match the alias in the package action XML
        /// </summary>
        /// <returns>The Alias in string format</returns>
        public string Alias()
        {
            return "UpdateNodeIdProperty";
        }

        /// <summary>
        /// Update the node id on the contacts page. 
        /// </summary>
        /// <param name="packageName">Name of the package that we install</param>
        /// <param name="xmlData">The data that must be appended to the UrlRewriting.config file</param>
        /// <returns>True when succeeded</returns>
        public bool Execute(string packageName, System.Xml.XmlNode xmlData)
        {
            //Set result default to false
            bool result = false;

            Document[] roots = umbraco.cms.businesslogic.web.Document.GetRootDocuments();

            try
            {
                //check for nodes. 
                if (xmlData.HasChildNodes)
                {
                    //get all root documents and start updating;
                    XmlNodeList updateProperties = xmlData.SelectNodes("./UpdateNodeIdProperty");
                    foreach (XmlNode updateProperty in updateProperties)
                        foreach (Document root in roots)
                            UpdateDocument(root, updateProperty);
                }

                result = true;
            }
            catch (System.Exception ex)
            {
                string message = string.Concat("Error at UpdateNodeIdProperty while checking and updating pages. ", ex.Message);
                Log.Add(LogTypes.Error, getUser(), -1, message);
            }

           return result;
        }
        private bool MatchNameAndType(Document document, XmlAttribute name, XmlAttribute type)
        {
            DocumentType matchType = null;
            string matchDocName = null;

            if (name != null)
                matchDocName = name.Value;
            if (type != null && !string.IsNullOrEmpty(type.Value))
                matchType = DocumentType.GetByAlias(type.Value);

            if ((string.IsNullOrEmpty(matchDocName) || document.Text == matchDocName) &&
                (matchType == null || matchType.UniqueId == document.nodeObjectType))
                return true;
            else
                return false;
            

        }
        
        private void UpdateDocument(Document document, XmlNode updatePropertyNode)
        {
            //check if current document match.
            if (MatchNameAndType(document,updatePropertyNode.Attributes["onDocumentName"],updatePropertyNode.Attributes["onDocumentType"]))
            {
                //locate sub page - when lookup on page name path has been implemented, this should look based on path instead of child nodes. 
                int subId = 0;
                foreach(Document child in document.Children)
                    if (MatchNameAndType(child, updatePropertyNode.Attributes["fromDocumentName"], updatePropertyNode.Attributes["fromDocumentType"]))
                    {
                        subId = child.Id;
                        break;
                    }

                //update if correct child page was found. 
                if (subId != 0)
                {
                    document.getProperty(updatePropertyNode.Attributes["propertyName"].Value).Value = subId;
                    document.Publish(getUser());
                }
            }

            //and update child pages while we're at it.
            foreach (Document child in document.Children)
                UpdateDocument(child, updatePropertyNode);

            
        }

        private User getUser()
        {
            int id = BasePage.GetUserId(BasePage.umbracoUserContextID);
            id = (id < 0) ? 0 : id;
            return User.GetUser(id);
        }

        /// <summary>
        /// As we only update property of a document WE created, we can ignore undo.
        /// </summary>
        /// <param name="packageName">Name of the package that we install</param>
        /// <param name="xmlData">The data that must be appended to the UrlRewriting.config file</param>
        /// <returns>True when succeeded</returns>
        public bool Undo(string packageName, System.Xml.XmlNode xmlData)
        {
            
            return true;
        }

        /// <summary>
        /// Returns a Sample XML Node 
        /// In this case the Sample XML Rule for the UpdateNodeIdProperty.
        /// </summary>
        /// <returns>The sample xml as node</returns>
        public XmlNode SampleXml()
        {
            string sample = "<Action runat=\"install\" undo=\"true/false\" alias=\"UpdateNodeIdProperty\"><UpdateNodeIdProperty onDocumentType=\"Contact\" onDocumentName=\"\" propertyName=\"redirectTo\" fromDocumentType=\"Textpage\" fromDocumentName=\"Thank you\"></Action>";
            return spa.helper.parseStringToXmlNode(sample);
        }

        #endregion

    }
}
