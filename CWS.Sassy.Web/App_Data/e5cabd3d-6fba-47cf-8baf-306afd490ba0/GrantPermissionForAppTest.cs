using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;
using NUnit.Framework;
using PackageActionsContrib.Helpers;
using PackageActionsContrib.Test.Helpers;
using umbraco.cms.businesslogic.packager.standardPackageActions;

namespace PackageActionsContrib.Test
{
	[TestFixture]
	public class GrantPermissionForAppTest
	{
		[Test]
		public void GetUserLoginFromXml_WellformedXml_ReturnsUserName()
		{
			// Arrange
			var sut = new GrantPermissionForAppSpy();
			XmlNode node = sut.SampleXml();

			// Act
			string userLogin = sut.GetUserLgionFromXmlExpoed(node.FirstChild);

			// Assert
			Assert.That(userLogin, Is.EqualTo("$CurrentUser"));
		}

		[Test]
		public void GetAppNameFromXml_WellformedXml_ReturnsAppName()
		{
			// Arrange
			var sut = new GrantPermissionForAppSpy();
			XmlNode node = sut.SampleXml();

			// Act
			string userLogin = sut.GetAppNameFromXmlExposed(node.FirstChild);

			// Assert
			Assert.That(userLogin, Is.EqualTo("uCommerce"));
		}

		[Test]
		public void GetAppNameFromXml_MalformedXml_ReturnsEmptyString()
		{
			//Arrange
			var sut = new GrantPermissionForAppSpy();
			XmlNode node =
				helper.parseStringToXmlNode("<Action runat=\"install\" undo=\"false\" alias=\"GrantPermissionForApp\"></Action>");

			//Act
			string appName = sut.GetAppNameFromXmlExposed(node.FirstChild);

			//Assert
			Assert.That(appName, Is.EqualTo(string.Empty));
		}
	}
}
