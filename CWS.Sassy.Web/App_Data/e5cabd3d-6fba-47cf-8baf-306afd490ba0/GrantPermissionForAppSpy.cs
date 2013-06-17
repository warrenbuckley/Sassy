using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PackageActionsContrib.Test.Helpers
{
	public class GrantPermissionForAppSpy : GrantPermissionForApp
	{
		public string GetAppNameFromXmlExposed(System.Xml.XmlNode xmlData)
		{
			return base.GetAppNameFromXml(xmlData);
		}

		public string GetUserLgionFromXmlExpoed(System.Xml.XmlNode xmlData)
		{
			return base.GetUserLoginFromXml(xmlData);
		}
	}
}
