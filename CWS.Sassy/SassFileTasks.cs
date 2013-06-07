using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ClientDependency.Core;
using Umbraco.Core.IO;
using umbraco.interfaces;

namespace CWS.Sassy
{
    public class SassFileTasks : ITaskReturnUrl
    {
        public int UserId { private get; set; }

        public int TypeID { get; set; }

        public string Alias { get; set; }

        public int ParentID { get; set; }

        public bool Save()
        {
            var createFolder = ParentID;
            var basePath = IOHelper.MapPath(LoadSassyTree.SassPath + "/" + Alias);

            if (createFolder == 1)
            {
                System.IO.Directory.CreateDirectory(basePath);
            }
            else
            {
                var directory = Path.GetDirectoryName(basePath);
                if (directory != null && !System.IO.Directory.Exists(directory))
                {
                    System.IO.Directory.CreateDirectory(directory);
                }

                System.IO.File.Create(basePath + ".scss").Close();
                m_returnUrl = string.Format("developer/sassy/SassyFileEditor.aspx?file={0}.scss", Alias);
            }
            return true;
        }

        public bool Delete()
        {
            var path = IOHelper.MapPath(LoadSassyTree.SassPath + Alias.TrimStart('/'));

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
            else if (System.IO.Directory.Exists(path))
                System.IO.Directory.Delete(path, true);

            umbraco.BusinessLogic.Log.Add(umbraco.BusinessLogic.LogTypes.Delete, umbraco.BasePages.UmbracoEnsuredPage.CurrentUser, -1, Alias + " Deleted");
            return true;
        }

        #region ITaskReturnUrl Members
        private string m_returnUrl = "";
        public string ReturnUrl
        {
            get { return m_returnUrl; }
        }

        #endregion
    }
}