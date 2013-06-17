using System;
using umbraco.uicontrols;
using umbraco.BasePages;

namespace tswe.pat
{
    public partial class manager : UmbracoEnsuredPage
    {
        
        # region members

        public TabPage tabTestPackageActions;

        # endregion members

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            // Tab setup
            tabTestPackageActions = TabView1.NewTabPage("Test Package Actions");
            tabTestPackageActions.Controls.Add(testAction);
            testAction.InitControlItems();

            base.OnInit(e);
        }

    }
}
