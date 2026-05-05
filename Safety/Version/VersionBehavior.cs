using System;
using Retinues.GUI.Helpers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace Retinues.Safety.Version
{
	// Token: 0x02000041 RID: 65
	[SafeClass]
	public class VersionBehavior : CampaignBehaviorBase
	{
		// Token: 0x0600014B RID: 331 RVA: 0x00009564 File Offset: 0x00007764
		public override void SyncData(IDataStore dataStore)
		{
			if (dataStore.IsSaving)
			{
				ModuleChecker.ModuleEntry module = ModuleChecker.GetModule("Retinues");
				this._retinuesVersion = (((module != null) ? module.Version : null) ?? "unknown");
			}
			dataStore.SyncData<string>("Retinues_Version", ref this._retinuesVersion);
		}

		// Token: 0x0600014C RID: 332 RVA: 0x000095B1 File Offset: 0x000077B1
		public override void RegisterEvents()
		{
			CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.OnGameLoadFinished));
		}

		// Token: 0x0600014D RID: 333 RVA: 0x000095CC File Offset: 0x000077CC
		private void OnGameLoadFinished()
		{
			try
			{
				ModuleChecker.ModuleEntry module = ModuleChecker.GetModule("Retinues");
				if (module == null)
				{
					Log.Warn("VersionBehavior: Retinues module not found in ModuleChecker.");
				}
				else
				{
					string version = module.Version;
					ApplicationVersion appVersion = module.AppVersion;
					Log.Info("Current Retinues version: " + version);
					Log.Info("Retinues version in save: " + this._retinuesVersion);
					if (string.IsNullOrWhiteSpace(this._retinuesVersion))
					{
						this._retinuesVersion = version;
						Log.Info("No Retinues version stored in save; assuming current version " + this._retinuesVersion + ".");
					}
					else
					{
						string retinuesVersion = this._retinuesVersion;
						ApplicationVersion applicationVersion;
						if (retinuesVersion == "unknown")
						{
							this.VersionMismatchPopup(version, retinuesVersion);
						}
						else if (!VersionBehavior.TryParseAppVersion(retinuesVersion, out applicationVersion) || appVersion == ApplicationVersion.Empty)
						{
							this.VersionMismatchPopup(version, retinuesVersion);
						}
						else if (appVersion.IsSame(applicationVersion, true))
						{
							Log.Info("Retinues version in save matches current version " + version + ".");
						}
						else
						{
							Log.Info("Save File: Retinues " + retinuesVersion);
							try
							{
								if (VersionBehavior.ShouldShowUpdatePopup(applicationVersion, appVersion))
								{
									this.VersionUpdatePopup(version, retinuesVersion);
								}
								else
								{
									this.VersionMismatchPopup(version, retinuesVersion);
								}
							}
							catch (Exception ex)
							{
								this.VersionMismatchPopup(version, retinuesVersion);
								Log.Exception(ex, "", null);
							}
						}
					}
				}
			}
			catch (Exception ex2)
			{
				Log.Exception(ex2, "", null);
			}
		}

		// Token: 0x0600014E RID: 334 RVA: 0x00009748 File Offset: 0x00007948
		private void VersionUpdatePopup(string currentVersionString, string saveVersionString)
		{
			if (saveVersionString == "unknown")
			{
				Notifications.Popup(L.T("retinues_update_title", "Retinues Update"), L.T("retinues_version_update_text_no_current", "The Retinues mod version in this save file has been updated to the next version ({CURRENT_VERSION}). Your save data has been automatically migrated to the new version.\n\nAs a safety precaution, check that everything is as it should before overwriting your save.\n\nIf you notice any issues and wish to go back to the previous version, do not save and download the version you were previously using from Nexus Mods.").SetTextVariable("CURRENT_VERSION", currentVersionString), null, true);
				return;
			}
			Notifications.Popup(L.T("retinues_update_title", "Retinues Update"), L.T("retinues_version_update_text", "The Retinues mod version in this save file has been updated to the next version ({CURRENT_VERSION}). Your save data has been automatically migrated to the new version.\n\nAs a safety precaution, check that everything is as it should before overwriting your save.\n\nIf you notice any issues and wish to go back to the previous version, do not save and download the {SAVE_VERSION} file from Nexus Mods.").SetTextVariable("SAVE_VERSION", saveVersionString).SetTextVariable("CURRENT_VERSION", currentVersionString), null, true);
		}

		// Token: 0x0600014F RID: 335 RVA: 0x000097D0 File Offset: 0x000079D0
		private void VersionMismatchPopup(string currentVersionString, string saveVersionString)
		{
			if (saveVersionString == "unknown")
			{
				Notifications.Popup(L.T("retinues_version_change_title", "Retinues Version Change"), L.T("retinues_version_change_text_no_current", "The Retinues mod version in this save file does not match the current mod version ({CURRENT_VERSION}).\n\nAs a safety precaution, check that everything is as it should before overwriting your save.\n\nIf you notice any issues and wish to go back to the previous version, do not save and download the version you were previously using from Nexus Mods.").SetTextVariable("CURRENT_VERSION", currentVersionString), null, true);
				return;
			}
			Notifications.Popup(L.T("retinues_version_change_title", "Retinues Version Change"), L.T("retinues_version_change_text", "The Retinues mod version in this save file does not match the current mod version ({CURRENT_VERSION}).\n\nAs a safety precaution, check that everything is as it should before overwriting your save.\n\nIf you notice any issues and wish to go back to the previous version, do not save and download the {SAVE_VERSION} file from Nexus Mods.").SetTextVariable("SAVE_VERSION", saveVersionString).SetTextVariable("CURRENT_VERSION", currentVersionString), null, true);
		}

		// Token: 0x06000150 RID: 336 RVA: 0x00009858 File Offset: 0x00007A58
		private static bool TryParseAppVersion(string versionString, out ApplicationVersion version)
		{
			version = ApplicationVersion.Empty;
			if (string.IsNullOrWhiteSpace(versionString))
			{
				return false;
			}
			if (versionString == "unknown")
			{
				return false;
			}
			string text = versionString.Trim();
			if (!char.IsLetter(text[0]))
			{
				text = "v" + text;
			}
			bool result;
			try
			{
				version = ApplicationVersion.FromString(text, 0);
				result = true;
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000151 RID: 337 RVA: 0x000098D4 File Offset: 0x00007AD4
		private static bool ShouldShowUpdatePopup(ApplicationVersion saveVersion, ApplicationVersion currentVersion)
		{
			if (!currentVersion.IsNewerThan(saveVersion))
			{
				return false;
			}
			if (saveVersion.Major != currentVersion.Major || saveVersion.Minor != currentVersion.Minor)
			{
				return false;
			}
			int revision = saveVersion.Revision;
			int changeSet = saveVersion.ChangeSet;
			int revision2 = currentVersion.Revision;
			int changeSet2 = currentVersion.ChangeSet;
			if (revision2 == 13 && changeSet2 == 0 && VersionBehavior.IsLegacyToNewBridge(saveVersion, currentVersion))
			{
				return true;
			}
			bool flag = revision2 == revision && changeSet2 == changeSet + 1;
			bool flag2 = revision2 == revision + 1 && changeSet2 == changeSet;
			return flag || flag2;
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00009968 File Offset: 0x00007B68
		private static bool IsLegacyToNewBridge(ApplicationVersion saveVersion, ApplicationVersion currentVersion)
		{
			if (currentVersion.Major == 1 && currentVersion.Minor == 2 && saveVersion.Revision == 12 && (saveVersion.ChangeSet == 9 || saveVersion.ChangeSet == 10))
			{
				return true;
			}
			if (currentVersion.Major == 1 && currentVersion.Minor == 3)
			{
				bool flag = saveVersion.Revision == 3 || saveVersion.Revision == 1;
				bool flag2 = saveVersion.ChangeSet == 9 || saveVersion.ChangeSet == 10;
				if (flag && flag2)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04000086 RID: 134
		private string _retinuesVersion;
	}
}
