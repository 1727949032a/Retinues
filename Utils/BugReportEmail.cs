using System;
using System.Text;
using Retinues.Configuration;
using TaleWorlds.Library;

namespace Retinues.Utils
{
	// Token: 0x02000023 RID: 35
	[SafeClass]
	public static class BugReportEmail
	{
		// Token: 0x0600008C RID: 140 RVA: 0x00004074 File Offset: 0x00002274
		public static void OpenDraft()
		{
			try
			{
				try
				{
					Config.LogDump();
				}
				catch (Exception arg)
				{
					Log.Warn(string.Format("BugReportEmail: Config.LogDump failed: {0}", arg));
				}
				string text = BannerlordVersion.Version.ToString() ?? "unknown";
				ModuleChecker.ModuleEntry module = ModuleChecker.GetModule("Retinues");
				string text2 = ((module != null) ? module.Version : null) ?? "unknown";
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("Please describe what happened **above** this line if your mail client allows it.");
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("-------------------------------------------------------------");
				stringBuilder.AppendLine("Environment");
				stringBuilder.AppendLine("- Game version: " + text);
				stringBuilder.AppendLine("- Retinues version: " + text2);
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("Active modules (load order):");
				foreach (ModuleChecker.ModuleEntry moduleEntry in ModuleChecker.GetActiveModules())
				{
					string text3 = moduleEntry.IsOfficial ? " (official)" : "";
					stringBuilder.AppendLine(string.Concat(new string[]
					{
						"- ",
						moduleEntry.Id,
						" [",
						moduleEntry.Version,
						"] - ",
						moduleEntry.Name,
						text3
					}));
				}
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("Debug log");
				stringBuilder.AppendLine("- Please attach your Retinues debug.log file to this email.");
				stringBuilder.AppendLine("- If you are unsure where it is, check the Retinues readme / troubleshooting section.");
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("Additional info");
				stringBuilder.AppendLine("- What were you doing when the bug happened?");
				stringBuilder.AppendLine("- Can you reproduce it? If yes, list the steps.");
				stringBuilder.AppendLine("- Any other mods that might be related?");
				string str = Uri.EscapeDataString(string.Concat(new string[]
				{
					"Retinues bug report (BL ",
					text,
					", Retinues ",
					text2,
					")"
				}));
				string str2 = Uri.EscapeDataString(stringBuilder.ToString());
				URL.OpenInBrowser("mailto:retinues.support@proton.me?subject=" + str + "&body=" + str2);
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
				InformationManager.DisplayMessage(new InformationMessage("Failed to open bug report email. See log for details."));
			}
		}

		// Token: 0x0400001F RID: 31
		private const string SupportEmail = "retinues.support@proton.me";
	}
}
