using System;
using System.Collections.Generic;
using System.Text;
using umbraco.interfaces;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using System.Xml;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.web;
using umbraco.cms.businesslogic.template;

namespace PackageActionsContrib
{
    public class AllowTemplateForDoctype : IPackageAction
    {
        #region IPackageAction Members

        public string Alias()
        {
            return "AllowTemplateForDoctype";
        }

        public bool Execute(string packageName, System.Xml.XmlNode xmlData)
        {
            bool result = false;
            try
            {
                //Get Doctype alias an Template alias from package action
                string doctypeAlias = xmlData.Attributes["documentTypeAlias"].InnerText;
                string templateAlias = xmlData.Attributes["templateAlias"].InnerText;
                
                //Instantiate doctype and template based on the alias
                DocumentType doctype = DocumentType.GetByAlias(doctypeAlias);
                Template allowedTemplate = Template.GetByAlias(templateAlias);
                
                //Hold the defaultTemplateId for later reference
                int defaultTemplate = doctype.DefaultTemplate;

                //Create a list of allowed templates.
                List<Template> allowedTemplateList = ArrayToTemplateList(doctype.allowedTemplates);

                //Check if template is added as an allowed template
                if (!DocTypeContainsAllowedTemplate(allowedTemplateList, allowedTemplate))
                {
                    //Template is not yet allowed add it.
                    //Assign the new allowed template
                    allowedTemplateList.Add(allowedTemplate);
                    //Update doctype with the new AllowedTemplates
                    doctype.allowedTemplates = TemplateListToArray(allowedTemplateList);

                    //Some how default template gets removed when allowed templates are assigned reset the defailt templatye
                    doctype.DefaultTemplate = defaultTemplate;

                    //Save the modified value(s)
                    doctype.Save();
                }

                result = true;
            }
            catch (Exception ex)
            {
                Log.Add(LogTypes.Error,-1, string.Format("Error in Execute method of Package Action{0} Details{1}",Alias(),ex.ToString()));
            }
            return result;
        }

             /// <summary>
        /// Returns a Sample XML Node 
        /// In this case the Sample XML for adding Runway Homepage template to runway textpage document type
        /// </summary>
        /// <returns>The sample xml as node</returns>
        public XmlNode SampleXml()
        {
            string sample = "<Action runat=\"install\" undo=\"true/false\" alias=\"AllowTemplateForDoctype\" documentTypeAlias=\"RunwayTextpage\" templateAlias=\"RunwayHomepage\" ></Action>";
            return helper.parseStringToXmlNode(sample);
        }

        /// <summary>
        /// Removes the Allowed template from the document allowed template list
        /// Use this Only for none shared templates
        /// </summary>
        /// <returns></returns>
        public bool Undo(string packageName, System.Xml.XmlNode xmlData)
        {
            bool result = false;
            try
            {
                //Get Doctype alias an Template alias from package action
                string doctypeAlias = xmlData.Attributes["documentTypeAlias"].InnerText;
                string templateAlias = xmlData.Attributes["templateAlias"].InnerText;

                //Instantiate doctype and template based on the alias
                DocumentType doctype = DocumentType.GetByAlias(doctypeAlias);
                Template allowedTemplate = Template.GetByAlias(templateAlias);

                //Hold the defaultTemplateId for later reference
                int defaultTemplate = doctype.DefaultTemplate;

                List<Template> allowedTemplateList = ArrayToTemplateList(doctype.allowedTemplates);

                //Check if template is added as an allowed template
                if (DocTypeContainsAllowedTemplate(allowedTemplateList, allowedTemplate))
                {
                    //Yes it is remove it
                    //Find the position to remove
                    int index = allowedTemplateList.FindIndex(delegate(Template t) { return t.Alias == allowedTemplate.Alias; });
                    //Remove the item based on index
                    allowedTemplateList.RemoveAt(index);
                    //Update doctype with the new AllowedTemplates
                    doctype.allowedTemplates = TemplateListToArray(allowedTemplateList);

                    //Some how default template gets removed when allowed templates are assigned reset the defailt templatye
                    doctype.DefaultTemplate = defaultTemplate;

                    //Save the modified value(s)
                    doctype.Save();
                }

                result = true;
            }
            catch (Exception ex)
            {
                Log.Add(LogTypes.Error, -1, string.Format("Error in Undo method of Package Action{0} Details{1}", Alias(), ex.ToString()));
            }
            return result;
        }

        #endregion

        /// <summary>
        /// Returns true if the template is assigned as allowedtemplate on the documenttype 
        /// </summary>
        /// <returns></returns>
        private bool DocTypeContainsAllowedTemplate(List<Template> allowedTemplates, Template allowedTemplate)
        {
            return allowedTemplates.Exists(delegate(Template t) { return t.Alias == allowedTemplate.Alias; });
        }

        /// <summary>
        /// Helper method that converts the Template Array to a List of Templates
        /// </summary>
        /// <param name="templateArray"></param>
        /// <returns></returns>
        private List<Template> ArrayToTemplateList( Template[] templateArray )
        {
            List<Template> templateList = new List<Template>();
            templateList.AddRange(templateArray);
            return templateList;
        }

        /// <summary>
        /// Converts a Template list back to an array so Umbraco can work with it.
        /// </summary>
        private Template[] TemplateListToArray(List<Template> templateList)
        {
            return templateList.ToArray();
        }
    }
}
