using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using umbraco.cms.presentation.Trees;
using umbraco;
using umbraco.interfaces;

namespace tswe.pat
{
	/// <summary>
	/// Summary description for Tree.
	/// </summary>
    public class loadPat : BaseTree
	{
        public loadPat(string application) : base(application) { }

        protected override void CreateRootNodeActions(ref List<IAction> actions)
        {
            actions.Clear();
        }

        protected override void CreateAllowedActions(ref List<IAction> actions)
        {
            actions.Clear();
        }

        protected override void CreateRootNode(ref XmlTreeNode rootNode)
        {
            string iconPath = "../../plugins/tswe/pat/images/patmanager.png";

            rootNode.Icon = iconPath;
            rootNode.OpenIcon = iconPath;
            rootNode.NodeType = "init" + TreeAlias;
            rootNode.NodeID = "init";
            rootNode.Text = "Package Actions Tester";
            rootNode.Action = "javascript:openPatManager();";
        }

        /// <summary>
        /// Renders the JS.
        /// </summary>
        /// <param name="Javascript">The javascript.</param>
        public override void RenderJS(ref StringBuilder Javascript)
        {
            Javascript.Append(@"
function openPatManager() {
    parent.right.document.location.href = 'plugins/tswe/pat/manager.aspx';
}");
        }

        /// <summary>
        /// This will call the normal Render method by passing the converted XmlTree to an XmlDocument.
        /// </summary>
        /// <param name="tree"></param>
        public override void Render(ref XmlTree tree)
        {
            // do nothing cause there are no sub nodes
        }
	}
}
