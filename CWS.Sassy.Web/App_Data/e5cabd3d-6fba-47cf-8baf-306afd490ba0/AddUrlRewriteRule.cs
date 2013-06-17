using System.Xml;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.interfaces;

namespace PackageActionsContrib
{ 
    /// <summary>
    /// This Package action will Add a new rule to the UrlRewriting.config file
    /// </summary>
    /// <remarks>
    /// This package action is part of the PackageActionsContrib Project
    /// </remarks>
    public class AddUrlRewriteRule : IPackageAction
    {
        //Namespace that is used by urlrewriting.net
        private const string NAMESPACEURI = "http://www.urlrewriting.net/schemas/config/2006/07";

        #region IPackageAction Members

        /// <summary>
        /// This Alias must be unique and is used as an identifier that must match the alias in the package action XML
        /// </summary>
        /// <returns>The Alias in string format</returns>
        public string Alias()
        {
            return "AddUrlRewriteRule";
        }

        /// <summary>
        /// Appends the xmlData Node to the UrlRewriting.config file
        /// </summary>
        /// <param name="packageName">Name of the package that we install</param>
        /// <param name="xmlData">The data that must be appended to the UrlRewriting.config file</param>
        /// <returns>True when succeeded</returns>
        public bool Execute(string packageName, System.Xml.XmlNode xmlData)
        {
            //Set result default to false
            bool result = false;

            //Check if the xmlData has a childnode (the xml rule node)
            if (xmlData.HasChildNodes)
            {
                //Open the URL Rewrite config file
                XmlDocument rewriteFile = umbraco.xmlHelper.OpenAsXmlDocument("/config/UrlRewriting.config");

                //Initialize a namespace Manager that adds the namespaces to the umbracoSettingsFile Xml Document 
                System.Xml.XmlNamespaceManager xmlnsManager = new System.Xml.XmlNamespaceManager(rewriteFile.NameTable);
                xmlnsManager.AddNamespace("urlrewritingnet", NAMESPACEURI);
                xmlnsManager.AddNamespace("", NAMESPACEURI);

                //Select RewriteRuleNode from the supplied xmlData
                XmlNode rewriteRuleNode = xmlData.SelectSingleNode("./add");
                
                //Select rewrites node in the config file and append the importNode
                XmlNode rewriteRootNode = rewriteFile.SelectSingleNode("//urlrewritingnet:rewrites", xmlnsManager);

                //Create a new Rewrite rule node 
                XmlNode newRewriteRuleNode = (XmlNode)rewriteFile.CreateElement("add", NAMESPACEURI);
                
                //Copy all attributes from the package xml to the new Rewrite rule node
                foreach (XmlAttribute att in rewriteRuleNode.Attributes)
                {
                    XmlAttribute attribute = rewriteFile.CreateAttribute(att.Name);
                    attribute.Value = att.Value;
                    newRewriteRuleNode.Attributes.Append(attribute);
                }

                //Append the new rewrite rule node to the Rewrite config file
                rewriteRootNode.AppendChild(newRewriteRuleNode);

                //Save the Rewrite config file with the new rewerite rule
                rewriteFile.Save(System.Web.HttpContext.Current.Server.MapPath("/config/UrlRewriting.config"));
                
                //No errors so the result is true
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Removes the xmlData Node from the UrlRewriting.config file based on the rulename 
        /// </summary>
        /// <param name="packageName">Name of the package that we install</param>
        /// <param name="xmlData">The data that must be appended to the UrlRewriting.config file</param>
        /// <returns>True when succeeded</returns>
        public bool Undo(string packageName, System.Xml.XmlNode xmlData)
        {
            bool result = false;
             //Check if the xmlData has a childnode (the xml rule node)
            if (xmlData.HasChildNodes)
            {
                //Get the rewrite rule xml node from the supplied xmlData node
                XmlNode rewriteData = xmlData.SelectSingleNode("//add");

                //Get the rule name
                string rewriteRuleName = rewriteData.Attributes["name"].Value;

                //Open the url Rewrite config file
                XmlDocument rewriteFile = umbraco.xmlHelper.OpenAsXmlDocument("/config/UrlRewriting.config");
                System.Xml.XmlNamespaceManager xmlnsManager = new System.Xml.XmlNamespaceManager(rewriteFile.NameTable);
                xmlnsManager.AddNamespace("urlrewritingnet", NAMESPACEURI);
                xmlnsManager.AddNamespace("", NAMESPACEURI);

                //Select the rootnode where we want to delete from
                XmlNode rewriteRootNode = rewriteFile.SelectSingleNode("//urlrewritingnet:rewrites", xmlnsManager);

                //Select the url rewrite rule by name from the config file
                XmlNode rewriteRule = rewriteRootNode.SelectSingleNode("//urlrewritingnet:add[@name = '" + rewriteRuleName + "']", xmlnsManager);
                if (rewriteRule != null)
                {
                    //Rule is found, remove it from the xml document
                    rewriteRootNode.RemoveChild(rewriteRule);
                    //Save the modified configuration file
                    rewriteFile.Save(System.Web.HttpContext.Current.Server.MapPath("/config/UrlRewriting.config"));
                }
                result = true;
            }
            return result;    
        }

        /// <summary>
        /// Returns a Sample XML Node 
        /// In this case the Sample XML Rule for the CWS2 Email friend functionality
        /// </summary>
        /// <returns>The sample xml as node</returns>
        public XmlNode SampleXml()
        {
            string sample = "<Action runat=\"install\" undo=\"true/false\" alias=\"AddUrlRewriteRule\"><add name=\"CWS_emaiAFriendID\" virtualUrl=\"^~/email-a-friend/(.[0-9]*).aspx\" rewriteUrlParameter=\"ExcludeFromClientQueryString\" destinationUrl=\"~/email-a-friend.aspx?nodeID=$1\" ignoreCase=\"true\" /></Action>";
            return helper.parseStringToXmlNode(sample);
        }
        
        #endregion

    }
}
