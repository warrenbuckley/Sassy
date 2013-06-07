using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BundleTransformer.Core.Assets;
using BundleTransformer.SassAndScss.HttpHandlers;
using BundleTransformer.SassAndScss.Translators;
using ClientDependency.Core;
using ClientDependency.Core.Controls;
using umbraco;
using umbraco.BasePages;
using umbraco.uicontrols;


namespace CWS.Sassy.developer.Sassy
{
    public partial class SassyFileEditor : UmbracoEnsuredPage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (!UmbracoPanel.hasMenu)
            {
                return;
            }

            var imageButton             = UmbracoPanel.Menu.NewImageButton();
            imageButton.AlternateText   = "Save File";
            imageButton.ImageUrl        = GlobalSettings.Path + "/images/editor/save.gif";
            imageButton.Click           += MenuSaveClick;
        }

        protected override void OnLoad(EventArgs e)
        {
            //Register SASS file
            ClientDependencyLoader.Instance.RegisterDependency(2, "CodeMirror/js/mode/sass/sass.js", "UmbracoClient", ClientDependencyType.Javascript);

            var file        = Request.QueryString["file"];
            var path        = LoadSassyTree.SassPath + file;
            TxtName.Text    = file;
            var appPath     = Request.ApplicationPath;

            if (appPath == "/")
            {
                appPath = string.Empty;
            }

            LtrlPath.Text = appPath + path;
            if (IsPostBack)
            {
                return;
            }

            string fullPath = Server.MapPath(path);
            if (File.Exists(fullPath))
            {
                string content;
                using (var streamReader = File.OpenText(fullPath))
                {
                    content = streamReader.ReadToEnd();
                }

                if (string.IsNullOrEmpty(content))
                {
                    return;
                }

                EditorSource.Text = content;
            }
            else
            {
                Feedback.Text           = (string.Format("The file '{0}' does not exist.", file));
                Feedback.type           = Feedback.feedbacktype.error;
                Feedback.Visible        = true;
                UmbracoPanel.hasMenu    = NamePanel.Visible = PathPanel.Visible = EditorPanel.Visible = false;
            }
        }

        private bool SaveConfigFile(string filename, string contents)
        {
            try
            {
                var path = Server.MapPath(LoadSassyTree.SassPath + filename);
                using (var text = File.CreateText(path))
                {
                    //Save the SASS file
                    text.Write(contents);
                    text.Close();

                    //Try to auto generate the compiled CSS & minified compiled CSS
                    IAsset cssFile = new Asset(path);
                    SassAndScssTranslator sassTranslator = new SassAndScssTranslator();
                    var compiledSass = sassTranslator.Translate(cssFile);

                    //normal Path
                    var normalCSS = path.Replace(".scss", ".css");

                    using (var compiledCSS = File.CreateText(normalCSS))
                    {
                        compiledCSS.Write(compiledSass.Content);
                        compiledCSS.Close();
                    }

                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void MenuSaveClick(object sender, ImageClickEventArgs e)
        {
            if (SaveConfigFile(TxtName.Text, EditorSource.Text))
            {
                ClientTools.ShowSpeechBubble(speechBubbleIcon.save, ui.Text("speechBubbles", "fileSavedHeader"), ui.Text("speechBubbles", "fileSavedText"));
            }
            else
            {
                ClientTools.ShowSpeechBubble(speechBubbleIcon.error, ui.Text("speechBubbles", "fileErrorHeader"), ui.Text("speechBubbles", "fileErrorText"));
            }
        }
    }
}