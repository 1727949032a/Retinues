using System;
using System.Diagnostics;
using TaleWorlds.Library;

namespace Retinues.Utils
{
	// Token: 0x02000035 RID: 53
	[SafeClass]
	public static class URL
	{
		// Token: 0x06000100 RID: 256 RVA: 0x00006788 File Offset: 0x00004988
		public static void OpenInBrowser(string url)
		{
			try
			{
				Process.Start(new ProcessStartInfo(url)
				{
					UseShellExecute = true
				});
			}
			catch (Exception arg)
			{
				InformationManager.DisplayMessage(new InformationMessage(L.T("open_browser_fail", "Failed to open browser. URL: {URL}").SetTextVariable("URL", url).ToString()));
				Log.Error(string.Format("Failed to open URL '{0}': {1}", url, arg));
			}
		}
	}
}
