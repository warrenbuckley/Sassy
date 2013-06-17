using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using PackageActionsContrib.Helpers;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.interfaces;

namespace PackageActionsContrib
{
	/// <summary>
	/// Moves a file
	/// </summary>
	/// <remarks>
	/// Can be used for renaming as well
	/// </remarks>
	public class MoveFile : IPackageAction
	{
		public bool Execute(string packageName, XmlNode xmlData)
		{
			File.Move(
				HttpContext.Current.Server.MapPath(GetSourceFileName(xmlData)), 
				HttpContext.Current.Server.MapPath(GetTargetFileName(xmlData)));

			return true;
		}

		private string GetSourceFileName(XmlNode xmlData)
		{
			return XmlHelper.GetAttributeValueFromNode(xmlData, "sourceFile");
		}

		private string GetTargetFileName(XmlNode xmlData)
		{
			return XmlHelper.GetAttributeValueFromNode(xmlData, "targetFile");
		}

		public string Alias()
		{
			return "MoveFile";
		}

		public bool Undo(string packageName, XmlNode xmlData)
		{
			File.Delete(HttpContext.Current.Server.MapPath(GetTargetFileName(xmlData)));
			
			return true;
		}

		public XmlNode SampleXml()
		{
            string sample = string.Format("<Action runat=\"install\" undo=\"false\" alias=\"{0}\" sourceFile=\"~/bin/UCommerce.Uninstaller.dll.tmp\" targetFile=\"~/bin/UCommerce.Uninstaller.dll\"/>", Alias());
			return helper.parseStringToXmlNode(sample);
		}
	}
}
