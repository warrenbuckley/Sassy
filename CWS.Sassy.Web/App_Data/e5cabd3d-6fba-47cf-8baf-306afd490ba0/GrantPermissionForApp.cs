using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using PackageActionsContrib.Helpers;
using umbraco.BasePages;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.DataLayer;
using umbraco.interfaces;

namespace PackageActionsContrib
{	
	/// <summary>
	/// Grants a user access to an Umbraco app
	/// </summary>
	public class GrantPermissionForApp : IPackageAction
	{
		#region Constants

		private const string REVOKE_SQL = "DELETE umbracoUser2app FROM umbracoUser2app JOIN umbracoUser ON umbracoUser2App.[user] = umbracoUser.id WHERE umbracoUser.userLogin = @UserLogin AND umbracoUser2App.app = @AppName";
		private const string GRANT_SQL = "INSERT INTO umbracoUser2app ([user], app) SELECT id, @AppName FROM umbracoUser WHERE userLogin = @UserLogin";

		#endregion

		#region Implementation of IPackageAction

		public bool Execute(string packageName, XmlNode xmlData)
		{
			//Execute revoke first to clear existing permissions app/user relationships
			Revoke(packageName, xmlData);
			return Grant(packageName, xmlData);
		}

		public string Alias()
		{
			return "GrantPermissionForApp";
		}

		public bool Undo(string packageName, XmlNode xmlData)
		{
			return Revoke(packageName, xmlData);
		}

		public XmlNode SampleXml()
		{
			string sample = "<Action runat=\"install\" undo=\"false\" alias=\"GrantPermissionForApp\" userLogin=\"$CurrentUser\" appName=\"uCommerce\"/>";
			return helper.parseStringToXmlNode(sample);
		}

		#endregion

		#region Supporting methods

		protected virtual string GetAppNameFromXml(XmlNode xmlData)
		{
			return GetAttributeValue(xmlData, "appName");
		}

		protected virtual string GetUserLoginFromXml(XmlNode xmlData)
		{
			return GetAttributeValue(xmlData, "userLogin");
		}

		internal string GetAttributeValue(XmlNode xmlData, string attributeName)
		{
			return XmlHelper.GetAttributeValueFromNode(xmlData, attributeName);
		}

		/// <summary>
		/// Grants persmission to app for user login
		/// </summary>
		/// <param name="userLogin">Login of the user</param>
		/// <param name="appName">Application name</param>
		/// <returns></returns>
		protected virtual bool Grant(string packageName, XmlNode xmlData)
		{
			return ExecutePermissionSql(packageName, xmlData, GRANT_SQL);
		}

		/// <summary>
		/// Revokes persmission to app for user login
		/// </summary>
		/// <param name="userLogin">Login of the user</param>
		/// <param name="appName">Application name</param>
		/// <returns></returns>
		protected virtual bool Revoke(string packageName, XmlNode xmlData)
		{
			return ExecutePermissionSql(packageName, xmlData, REVOKE_SQL);
		}

		private bool ExecutePermissionSql(string packageName, XmlNode xmlData, string sql)
		{
			string appName = GetAppNameFromXml(xmlData);
			string userLogin = GetUserLoginFromXml(xmlData);

			if (UserLoginIsCurrentUserPlaceHolder(userLogin))
				userLogin = GetUserLoginOfCurrentUser();

			IParameter userNameParam = Application.SqlHelper.CreateParameter("@UserLogin", userLogin);
			IParameter appNameParam = Application.SqlHelper.CreateParameter("@AppName", appName);

			try
			{
				Application.SqlHelper.ExecuteNonQuery(sql, userNameParam, appNameParam);
				return true;
			}
			catch (SqlHelperException sqlException)
			{
				Log.Add(LogTypes.Error, -1, string.Format("Error in Grant User Permission for App action for package {0} error:{1} ", packageName, sqlException.ToString()));
			}

			return false;
		}

		/// <summary>
		/// Returns the user name of the user currently logged in
		/// </summary>
		/// <remarks>
		/// If no user is logged in "admin" is returned as as default
		/// </remarks>
		/// <returns></returns>
		public string GetUserLoginOfCurrentUser()
		{
			return UmbracoEnsuredPage.CurrentUser.LoginName;
		}

		/// <summary>
		/// Determines whether to lookup the user login of the current user
		/// </summary>
		/// <param name="login"></param>
		/// <returns></returns>
		private bool UserLoginIsCurrentUserPlaceHolder(string login)
		{
			return login == "$CurrentUser";
		}

		#endregion

	}
}
