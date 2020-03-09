using UnityEditor;
using UnityEngine;

namespace JCMG.Nodey.Editor
{
	/// <summary>
	/// Menu items for this library.
	/// </summary>
	internal static class NodeyMenuItems
	{
		[MenuItem("Tools/JCMG/Nodey/Submit bug or feature request")]
		internal static void OpenURLToGitHubIssuesSection()
		{
			const string GITHUB_ISSUES_URL = "https://github.com/jeffcampbellmakesgames/nodey/issues";

			Application.OpenURL(GITHUB_ISSUES_URL);
		}

		[MenuItem("Tools/JCMG/Nodey/Donate to support development")]
		internal static void OpenURLToKoFi()
		{
			const string KOFI_URL = "https://ko-fi.com/stampyturtle";

			Application.OpenURL(KOFI_URL);
		}

		[MenuItem("Tools/JCMG/Nodey/About")]
		internal static void OpenAboutModalDialog()
		{
			AboutWindow.View();
		}
	}
}
