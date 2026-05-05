using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Retinues.GUI.Helpers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;

namespace Retinues.Safety.Version
{
	// Token: 0x02000040 RID: 64
	[SafeClass]
	public class DependenciesBehavior : CampaignBehaviorBase
	{
		// Token: 0x06000146 RID: 326 RVA: 0x0000941E File Offset: 0x0000761E
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06000147 RID: 327 RVA: 0x00009420 File Offset: 0x00007620
		public override void RegisterEvents()
		{
			CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.OnGameLoadFinished));
		}

		// Token: 0x06000148 RID: 328 RVA: 0x0000943C File Offset: 0x0000763C
		private void OnGameLoadFinished()
		{
			try
			{
				DependenciesBehavior.DependencyStatus dependencyStatus = DependenciesBehavior.<OnGameLoadFinished>g__CheckDependency|3_0(SubModule.UIExtenderExEnabled, "Bannerlord.UIExtenderEx");
				DependenciesBehavior.DependencyStatus dependencyStatus2 = DependenciesBehavior.<OnGameLoadFinished>g__CheckDependency|3_0(SubModule.HarmonyPatchesApplied, "Bannerlord.Harmony");
				if (dependencyStatus == DependenciesBehavior.DependencyStatus.OK && dependencyStatus2 == DependenciesBehavior.DependencyStatus.OK)
				{
					Log.Debug("All dependencies initialized correctly.");
				}
				else
				{
					List<string> list = new List<string>();
					if (dependencyStatus != DependenciesBehavior.DependencyStatus.OK)
					{
						list.Add(string.Format("UIExtenderEx: {0}", dependencyStatus));
					}
					if (dependencyStatus2 != DependenciesBehavior.DependencyStatus.OK)
					{
						list.Add(string.Format("Harmony: {0}", dependencyStatus2));
					}
					string text = (list.Count > 0) ? string.Join("; ", list) : "none";
					Log.Error(string.Format("Module started without full initialization. UIExtenderExEnabled={0}, HarmonyPatchesApplied={1}. Details: {2}.", SubModule.UIExtenderExEnabled, SubModule.HarmonyPatchesApplied, text));
					Notifications.Popup(L.T("retinues_init_error_title", "Retinues Dependency Error"), L.T("retinues_init_error_body_detailed", "Retinues mod dependencies failed to initialize properly:\n\n{DETAILS}.\n\nThe mod will not function correctly until the dependencies are correct.").SetTextVariable("DETAILS", text), null, true);
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "Error during dependency check.", null);
			}
		}

		// Token: 0x0600014A RID: 330 RVA: 0x00009550 File Offset: 0x00007750
		[CompilerGenerated]
		internal static DependenciesBehavior.DependencyStatus <OnGameLoadFinished>g__CheckDependency|3_0(bool enabled, string moduleId)
		{
			if (enabled)
			{
				return DependenciesBehavior.DependencyStatus.OK;
			}
			if (ModuleChecker.IsLoaded(moduleId))
			{
				return DependenciesBehavior.DependencyStatus.Error;
			}
			return DependenciesBehavior.DependencyStatus.Missing;
		}

		// Token: 0x02000125 RID: 293
		private enum DependencyStatus
		{
			// Token: 0x04000351 RID: 849
			OK,
			// Token: 0x04000352 RID: 850
			Missing,
			// Token: 0x04000353 RID: 851
			Error
		}
	}
}
