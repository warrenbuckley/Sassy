using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Text;
using umbraco.BusinessLogic.Actions;
using umbraco.businesslogic;
using umbraco.cms.presentation.Trees;
using umbraco.interfaces;

namespace CWS.Sassy
{
    [Tree("settings", "sassyTree", "Sassy")]
    public class LoadSassyTree : FileSystemTree
    {
        public const string SassPath            = "/css/sass/";
        public const string SassSearchPattern   = "*.scss";

        public LoadSassyTree(string application) : base(application)
        {
            //Ensure SASS folder is on disk inside CSS
            if (!Directory.Exists(SassPath))
            {
                //If it doesn't exist - lets create it
                Directory.CreateDirectory(SassPath);
            }
        }

        public override void Render(ref XmlTree tree)
        {
            base.Render(ref tree);
            if (!string.IsNullOrEmpty(NodeKey))
            {
                return;
            }
        }

        protected override string FilePath
        {
            get { return SassPath; }
        }

        protected override string FileSearchPattern
        {
            get {  return SassSearchPattern; }
        }

        protected override void CreateRootNode(ref XmlTreeNode rootNode)
        {
            rootNode.Text       = "Sass Files";
            rootNode.Icon       = ".sprTreeFolder";
            rootNode.OpenIcon   = ".sprTreeFolder_o";
            rootNode.NodeID     = "init";
            rootNode.NodeType   = rootNode.NodeID + TreeAlias;
            rootNode.Menu       = new List<IAction> { ActionNew.Instance, ActionRefresh.Instance };
        }

        public override void RenderJS(ref StringBuilder Javascript)
        {
            Javascript.Append(
            @"
                function openSassyFileEditor(id) { 
                    parent.right.document.location.href = 'developer/sassy/SassyFileEditor.aspx?file=' + id; 
                }");
        }

        protected override void OnRenderFolderNode(ref XmlTreeNode xNode)
        {
            xNode.Menu      = new List<IAction> {ActionNew.Instance, ActionRefresh.Instance };
            xNode.NodeType  = "configFolder";
        }

        protected override void OnRenderFileNode(ref XmlTreeNode xNode)
        {
            xNode.Action    = xNode.Action.Replace("openFile", "openConfigEditor");
            xNode.Menu      = new List<IAction> { ActionDelete.Instance };
            xNode.Icon      = "../../images/umbraco/settingCss.gif";
            xNode.OpenIcon  = xNode.Icon;
            xNode.Action    = string.Format("javascript:openSassyFileEditor('{0}');", xNode.NodeID);
        }

    }
}