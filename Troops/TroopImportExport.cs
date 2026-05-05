using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Retinues.Game;
using Retinues.Game.Wrappers;
using Retinues.GUI.Helpers;
using Retinues.Safety.Legacy;
using Retinues.Troops.Save;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ModuleManager;
using TaleWorlds.ObjectSystem;

namespace Retinues.Troops
{
	// Token: 0x02000039 RID: 57
	[SafeClass]
	public static class TroopImportExport
	{
		// Token: 0x0600011A RID: 282 RVA: 0x0000760C File Offset: 0x0000580C
		private static void EnsureDir()
		{
			Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(TroopImportExport.DefaultDir, "x")) ?? ".");
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00007631 File Offset: 0x00005831
		public static string SuggestTimestampName(string prefix)
		{
			return string.Format("{0}_{1:yyyy_MM_dd_HH_mm}.xml", prefix, DateTime.Now);
		}

		// Token: 0x0600011C RID: 284 RVA: 0x00007648 File Offset: 0x00005848
		private static string NormalizePath(string fileName)
		{
			if (string.IsNullOrWhiteSpace(fileName))
			{
				return Path.Combine(TroopImportExport.DefaultDir, "troops.xml");
			}
			string path = fileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase) ? fileName : (fileName + ".xml");
			return Path.Combine(TroopImportExport.DefaultDir, path);
		}

		// Token: 0x0600011D RID: 285 RVA: 0x00007698 File Offset: 0x00005898
		private static bool IsUnifiedExport(string absPath)
		{
			try
			{
				using (FileStream fileStream = File.OpenRead(absPath))
				{
					using (XmlReader xmlReader = XmlReader.Create(fileStream, new XmlReaderSettings
					{
						IgnoreComments = true
					}))
					{
						while (xmlReader.Read())
						{
							if (xmlReader.NodeType == XmlNodeType.Element)
							{
								return xmlReader.Name == "RetinuesTroops";
							}
						}
					}
				}
			}
			catch
			{
			}
			return false;
		}

		// Token: 0x0600011E RID: 286 RVA: 0x00007730 File Offset: 0x00005930
		public static List<string> ListValidUnifiedFilesNewestFirst()
		{
			TroopImportExport.EnsureDir();
			IEnumerable<string> source = from p in Directory.EnumerateFiles(TroopImportExport.DefaultDir, "*.xml", SearchOption.TopDirectoryOnly)
			where TroopImportExport.IsUnifiedExport(p) || LegacyTroopImporter.IsLegacyExport(p)
			select p;
			Func<string, DateTime> keySelector;
			if ((keySelector = TroopImportExport.<>O.<0>__GetLastWriteTimeUtc) == null)
			{
				keySelector = (TroopImportExport.<>O.<0>__GetLastWriteTimeUtc = new Func<string, DateTime>(File.GetLastWriteTimeUtc));
			}
			IEnumerable<string> source2 = source.OrderByDescending(keySelector);
			Func<string, string> selector;
			if ((selector = TroopImportExport.<>O.<1>__GetFileName) == null)
			{
				selector = (TroopImportExport.<>O.<1>__GetFileName = new Func<string, string>(Path.GetFileName));
			}
			return source2.Select(selector).ToList<string>();
		}

		// Token: 0x0600011F RID: 287 RVA: 0x000077BC File Offset: 0x000059BC
		private static bool TryResolveExistingPath(string fileName, out string absPath)
		{
			absPath = null;
			if (string.IsNullOrWhiteSpace(fileName))
			{
				return false;
			}
			if (File.Exists(fileName))
			{
				absPath = Path.GetFullPath(fileName);
				return true;
			}
			string text = Path.Combine(TroopImportExport.DefaultDir, fileName);
			if (File.Exists(text))
			{
				absPath = text;
				return true;
			}
			if (!fileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
			{
				text = Path.Combine(TroopImportExport.DefaultDir, fileName + ".xml");
				if (File.Exists(text))
				{
					absPath = text;
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000120 RID: 288 RVA: 0x00007834 File Offset: 0x00005A34
		private static void SerializeUnifiedToFile(TroopImportExport.RetinuesTroopsPackage payload, string absPath)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(TroopImportExport.RetinuesTroopsPackage));
			XmlWriterSettings settings = new XmlWriterSettings
			{
				Indent = true,
				Encoding = new UTF8Encoding(false)
			};
			TroopImportExport.EnsureDir();
			using (FileStream fileStream = File.Create(absPath))
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(fileStream, settings))
				{
					xmlSerializer.Serialize(xmlWriter, payload);
				}
			}
		}

		// Token: 0x06000121 RID: 289 RVA: 0x000078B8 File Offset: 0x00005AB8
		private static TroopImportExport.RetinuesTroopsPackage DeserializeUnifiedFromFile(string absPath)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(TroopImportExport.RetinuesTroopsPackage));
			TroopImportExport.RetinuesTroopsPackage result;
			using (FileStream fileStream = File.OpenRead(absPath))
			{
				result = (TroopImportExport.RetinuesTroopsPackage)xmlSerializer.Deserialize(fileStream);
			}
			return result;
		}

		// Token: 0x06000122 RID: 290 RVA: 0x00007908 File Offset: 0x00005B08
		public static string ExportUnified(string fileName, bool includeCustom, bool includeCultures)
		{
			if (!includeCustom && !includeCultures)
			{
				throw new InvalidOperationException("Nothing selected to export.");
			}
			string text = TroopImportExport.NormalizePath(fileName);
			TroopImportExport.RetinuesTroopsPackage retinuesTroopsPackage = new TroopImportExport.RetinuesTroopsPackage();
			if (includeCustom)
			{
				retinuesTroopsPackage.Factions = new TroopImportExport.FactionExportData
				{
					clanData = new FactionSaveData(Player.Clan),
					kingdomData = new FactionSaveData(Player.Kingdom)
				};
			}
			if (includeCultures)
			{
				MBReadOnlyList<CultureObject> objectTypeList = MBObjectManager.Instance.GetObjectTypeList<CultureObject>();
				List<CultureObject> list;
				if (objectTypeList == null)
				{
					list = null;
				}
				else
				{
					list = objectTypeList.OrderBy(delegate(CultureObject c)
					{
						if (c == null)
						{
							return null;
						}
						TextObject name = c.Name;
						if (name == null)
						{
							return null;
						}
						return name.ToString();
					}).ToList<CultureObject>();
				}
				foreach (CultureObject culture in (list ?? new List<CultureObject>()))
				{
					retinuesTroopsPackage.Cultures.Add(new FactionSaveData(new WCulture(culture)));
				}
				foreach (Clan clan in Clan.All)
				{
					if (clan.IsMinorFaction)
					{
						retinuesTroopsPackage.Cultures.Add(new FactionSaveData(new WClan(clan)));
					}
				}
			}
			TroopImportExport.SerializeUnifiedToFile(retinuesTroopsPackage, text);
			return Path.GetFullPath(text);
		}

		// Token: 0x06000123 RID: 291 RVA: 0x00007A6C File Offset: 0x00005C6C
		public static void MakeBackup()
		{
			try
			{
				TroopImportExport.EnsureDir();
				string text = Path.Combine(TroopImportExport.DefaultDir, "auto_backup.xml");
				TroopImportExport.SerializeUnifiedToFile(new TroopImportExport.RetinuesTroopsPackage
				{
					Factions = new TroopImportExport.FactionExportData
					{
						clanData = new FactionSaveData(Player.Clan),
						kingdomData = new FactionSaveData(Player.Kingdom)
					}
				}, text);
				Log.Debug("[TroopImportExport] Backup created at: " + text);
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "[TroopImportExport] Failed to create troop backup before saving.", null);
			}
		}

		// Token: 0x06000124 RID: 292 RVA: 0x00007AFC File Offset: 0x00005CFC
		public static bool ImportUnified(string fileName, TroopImportExport.ImportScope scope)
		{
			string text;
			if (!TroopImportExport.TryResolveExistingPath(fileName, out text))
			{
				return false;
			}
			TroopImportExport.RetinuesTroopsPackage retinuesTroopsPackage;
			if (LegacyTroopImporter.IsLegacyExport(text))
			{
				retinuesTroopsPackage = LegacyTroopImporter.LoadLegacyPackage(text);
				if (retinuesTroopsPackage == null)
				{
					throw new InvalidOperationException("Failed to load legacy export '" + fileName + "'.");
				}
			}
			else
			{
				retinuesTroopsPackage = TroopImportExport.DeserializeUnifiedFromFile(text);
			}
			if (retinuesTroopsPackage == null)
			{
				throw new InvalidOperationException("File not found or invalid format: '" + fileName + "'.");
			}
			bool flag = scope == TroopImportExport.ImportScope.CustomOnly || scope == TroopImportExport.ImportScope.Both;
			if (flag && retinuesTroopsPackage.HasFactions)
			{
				FactionSaveData clanData = retinuesTroopsPackage.Factions.clanData;
				if (clanData != null)
				{
					clanData.Apply(Player.Clan);
				}
				FactionSaveData kingdomData = retinuesTroopsPackage.Factions.kingdomData;
				if (kingdomData != null)
				{
					kingdomData.Apply(Player.Kingdom);
				}
			}
			flag = (scope - TroopImportExport.ImportScope.CulturesOnly <= 1);
			if (flag && retinuesTroopsPackage.HasCultures)
			{
				foreach (FactionSaveData factionSaveData in retinuesTroopsPackage.Cultures)
				{
					factionSaveData.Apply(null);
				}
			}
			return true;
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00007C08 File Offset: 0x00005E08
		public static void ShowUnifiedPicker(string title, string body, string confirmText, Action<string> onChoice)
		{
			TroopImportExport.EnsureDir();
			List<string> list = TroopImportExport.ListValidUnifiedFilesNewestFirst();
			if (list.Count == 0)
			{
				Notifications.Popup(L.T("no_exports_title", "No Exports Found"), L.T("no_exports_body", "No valid export files were found in the Exports folder."), null, true);
				return;
			}
			List<InquiryElement> inquiryElements = (from f in list
			select new InquiryElement(f, f, null)).ToList<InquiryElement>();
			MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(title, body, inquiryElements, true, 1, 1, confirmText, L.S("cancel", "Cancel"), delegate(List<InquiryElement> sel)
			{
				object obj;
				if (sel == null)
				{
					obj = null;
				}
				else
				{
					InquiryElement inquiryElement = sel.FirstOrDefault<InquiryElement>();
					obj = ((inquiryElement != null) ? inquiryElement.Identifier : null);
				}
				string text = obj as string;
				if (string.IsNullOrWhiteSpace(text))
				{
					Notifications.Popup(L.T("no_selection_title", "No Selection"), L.T("no_selection_body", "No file was selected."), null, true);
					return;
				}
				onChoice(text);
			}, delegate(List<InquiryElement> _)
			{
			}, "", false), false, false);
		}

		// Token: 0x06000126 RID: 294 RVA: 0x00007CDC File Offset: 0x00005EDC
		public static void PromptAndExport(string suggestedName = null)
		{
			List<InquiryElement> inquiryElements = new List<InquiryElement>
			{
				new InquiryElement("custom", L.S("exp_player_troops", "Player Troops"), null, true, null),
				new InquiryElement("cultures", L.S("exp_culture_troops", "Culture Troops"), null, true, null)
			};
			MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(L.S("export_which_title", "Export Selection"), L.S("export_which_body", "Select one or both."), inquiryElements, true, 1, 2, L.S("continue", "Continue"), L.S("cancel", "Cancel"), delegate(List<InquiryElement> selected)
			{
				HashSet<string> hashSet;
				if (selected == null)
				{
					hashSet = null;
				}
				else
				{
					hashSet = (from e in selected
					select e.Identifier as string).ToHashSet<string>();
				}
				HashSet<string> hashSet2 = hashSet ?? new HashSet<string>();
				bool includeCustom = hashSet2.Contains("custom");
				bool includeCultures = hashSet2.Contains("cultures");
				InformationManager.ShowTextInquiry(new TextInquiryData(L.S("enter_file_name", "Enter a file name:"), string.Empty, true, true, L.S("confirm", "Confirm"), L.S("cancel", "Cancel"), delegate(string name)
				{
					try
					{
						TroopImportExport.EnsureDir();
						string variable = TroopImportExport.ExportUnified(string.IsNullOrWhiteSpace(name) ? TroopImportExport.SuggestTimestampName("troops") : name.Trim(), includeCustom, includeCultures);
						Notifications.Popup(L.T("export_done_title", "Export Completed"), L.T("export_done_body", "Exported to: {PATH}.").SetTextVariable("PATH", variable), null, true);
					}
					catch (Exception ex)
					{
						Notifications.Popup(L.T("export_fail_title", "Export Failed"), L.T("export_fail_body", ex.Message), null, true);
					}
				}, delegate()
				{
				}, false, null, "", suggestedName ?? TroopImportExport.SuggestTimestampName("troops")), false, false);
			}, delegate(List<InquiryElement> _)
			{
			}, "", false), false, false);
		}

		// Token: 0x06000127 RID: 295 RVA: 0x00007DC0 File Offset: 0x00005FC0
		public static void PickAndImportUnified(Action afterImport = null)
		{
			TroopImportExport.<>c__DisplayClass18_0 CS$<>8__locals1 = new TroopImportExport.<>c__DisplayClass18_0();
			CS$<>8__locals1.afterImport = afterImport;
			TroopImportExport.ShowUnifiedPicker(L.S("import_pick_title", "Import Troops"), L.S("import_pick_body", "Select an exported file to import."), L.S("import", "Import"), delegate(string choice)
			{
				TroopImportExport.<>c__DisplayClass18_1 CS$<>8__locals2 = new TroopImportExport.<>c__DisplayClass18_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				CS$<>8__locals2.choice = choice;
				try
				{
					string text = Path.Combine(TroopImportExport.DefaultDir, CS$<>8__locals2.choice);
					if (LegacyTroopImporter.IsLegacyExport(text))
					{
						CS$<>8__locals2.pkg = LegacyTroopImporter.LoadLegacyPackage(text);
					}
					else
					{
						CS$<>8__locals2.pkg = TroopImportExport.DeserializeUnifiedFromFile(text);
					}
				}
				catch (Exception ex)
				{
					Notifications.Popup(L.T("import_fail_title", "Import Failed"), L.T("import_fail_body", ex.Message), null, true);
					return;
				}
				if (!CS$<>8__locals2.pkg.HasFactions && !CS$<>8__locals2.pkg.HasCultures)
				{
					Notifications.Popup(L.T("import_empty_title", "Nothing To Import"), L.T("import_empty_body", "The file contains no troops."), null, true);
					return;
				}
				if (CS$<>8__locals2.pkg.HasFactions && CS$<>8__locals2.pkg.HasCultures)
				{
					List<InquiryElement> inquiryElements = new List<InquiryElement>
					{
						new InquiryElement("custom", L.S("imp_player_troops", "Player Troops"), null, true, null),
						new InquiryElement("cultures", L.S("imp_culture_troops", "Culture Troops"), null, true, null)
					};
					MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(L.S("import_which_title", "Import Selection"), L.S("import_which_body", "Select one or both."), inquiryElements, true, 1, 2, L.S("continue", "Continue"), L.S("cancel", "Cancel"), delegate(List<InquiryElement> picked)
					{
						HashSet<string> hashSet;
						if (picked == null)
						{
							hashSet = null;
						}
						else
						{
							hashSet = (from e in picked
							select e.Identifier as string).ToHashSet<string>();
						}
						HashSet<string> hashSet2 = hashSet ?? new HashSet<string>();
						TroopImportExport.ImportScope scopeToUse2 = (hashSet2.Contains("custom") && hashSet2.Contains("cultures")) ? TroopImportExport.ImportScope.Both : (hashSet2.Contains("custom") ? TroopImportExport.ImportScope.CustomOnly : TroopImportExport.ImportScope.CulturesOnly);
						base.<PickAndImportUnified>g__TryConfirmAndImport|2(scopeToUse2);
					}, delegate(List<InquiryElement> _)
					{
					}, "", false), false, false);
					return;
				}
				TroopImportExport.ImportScope scopeToUse = CS$<>8__locals2.pkg.HasFactions ? TroopImportExport.ImportScope.CustomOnly : TroopImportExport.ImportScope.CulturesOnly;
				CS$<>8__locals2.<PickAndImportUnified>g__TryConfirmAndImport|2(scopeToUse);
			});
		}

		// Token: 0x06000128 RID: 296 RVA: 0x00007E18 File Offset: 0x00006018
		private static bool HasCulturePersistenceMismatch(TroopImportExport.RetinuesTroopsPackage pkg, out List<string> missingRootIds)
		{
			TroopImportExport.<>c__DisplayClass19_0 CS$<>8__locals1 = new TroopImportExport.<>c__DisplayClass19_0();
			missingRootIds = null;
			if (pkg == null || pkg.Cultures == null || pkg.Cultures.Count == 0)
			{
				return false;
			}
			HashSet<string> editedVanillaRootIds = WCharacter.EditedVanillaRootIds;
			if (editedVanillaRootIds == null || editedVanillaRootIds.Count == 0)
			{
				return false;
			}
			CS$<>8__locals1.fileRoots = new HashSet<string>(StringComparer.Ordinal);
			foreach (FactionSaveData factionSaveData in pkg.Cultures)
			{
				CS$<>8__locals1.<HasCulturePersistenceMismatch>g__AddRoot|0(factionSaveData.RetinueElite);
				CS$<>8__locals1.<HasCulturePersistenceMismatch>g__AddRoot|0(factionSaveData.RetinueBasic);
				CS$<>8__locals1.<HasCulturePersistenceMismatch>g__AddRoot|0(factionSaveData.RootElite);
				CS$<>8__locals1.<HasCulturePersistenceMismatch>g__AddRoot|0(factionSaveData.RootBasic);
				CS$<>8__locals1.<HasCulturePersistenceMismatch>g__AddRoot|0(factionSaveData.MilitiaMelee);
				CS$<>8__locals1.<HasCulturePersistenceMismatch>g__AddRoot|0(factionSaveData.MilitiaMeleeElite);
				CS$<>8__locals1.<HasCulturePersistenceMismatch>g__AddRoot|0(factionSaveData.MilitiaRanged);
				CS$<>8__locals1.<HasCulturePersistenceMismatch>g__AddRoot|0(factionSaveData.MilitiaRangedElite);
				CS$<>8__locals1.<HasCulturePersistenceMismatch>g__AddRoot|0(factionSaveData.CaravanGuard);
				CS$<>8__locals1.<HasCulturePersistenceMismatch>g__AddRoot|0(factionSaveData.CaravanMaster);
				CS$<>8__locals1.<HasCulturePersistenceMismatch>g__AddRoot|0(factionSaveData.Villager);
			}
			List<string> list = (from id in editedVanillaRootIds
			where !CS$<>8__locals1.fileRoots.Contains(id)
			select id).ToList<string>();
			if (list.Count == 0)
			{
				return false;
			}
			missingRootIds = list;
			return true;
		}

		// Token: 0x04000054 RID: 84
		public static readonly string DefaultDir = Path.Combine(ModuleHelper.GetModuleFullPath("Retinues"), "Exports");

		// Token: 0x04000055 RID: 85
		private const string RootUnified = "RetinuesTroops";

		// Token: 0x02000112 RID: 274
		public struct FactionExportData
		{
			// Token: 0x17000459 RID: 1113
			// (get) Token: 0x06000A6D RID: 2669 RVA: 0x00030F23 File Offset: 0x0002F123
			public readonly bool HasAny
			{
				get
				{
					return this.clanData != null || this.kingdomData != null;
				}
			}

			// Token: 0x0400031C RID: 796
			public FactionSaveData clanData;

			// Token: 0x0400031D RID: 797
			public FactionSaveData kingdomData;
		}

		// Token: 0x02000113 RID: 275
		[XmlRoot("RetinuesTroops")]
		public class RetinuesTroopsPackage
		{
			// Token: 0x1700045A RID: 1114
			// (get) Token: 0x06000A6E RID: 2670 RVA: 0x00030F38 File Offset: 0x0002F138
			// (set) Token: 0x06000A6F RID: 2671 RVA: 0x00030F40 File Offset: 0x0002F140
			public TroopImportExport.FactionExportData Factions { get; set; }

			// Token: 0x1700045B RID: 1115
			// (get) Token: 0x06000A70 RID: 2672 RVA: 0x00030F49 File Offset: 0x0002F149
			// (set) Token: 0x06000A71 RID: 2673 RVA: 0x00030F51 File Offset: 0x0002F151
			public List<FactionSaveData> Cultures { get; set; } = new List<FactionSaveData>();

			// Token: 0x1700045C RID: 1116
			// (get) Token: 0x06000A72 RID: 2674 RVA: 0x00030F5C File Offset: 0x0002F15C
			public bool HasFactions
			{
				get
				{
					return this.Factions.HasAny;
				}
			}

			// Token: 0x1700045D RID: 1117
			// (get) Token: 0x06000A73 RID: 2675 RVA: 0x00030F77 File Offset: 0x0002F177
			public bool HasCultures
			{
				get
				{
					return this.Cultures != null && this.Cultures.Count > 0;
				}
			}
		}

		// Token: 0x02000114 RID: 276
		public enum ImportScope
		{
			// Token: 0x04000321 RID: 801
			CustomOnly,
			// Token: 0x04000322 RID: 802
			CulturesOnly,
			// Token: 0x04000323 RID: 803
			Both
		}

		// Token: 0x02000115 RID: 277
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04000324 RID: 804
			public static Func<string, DateTime> <0>__GetLastWriteTimeUtc;

			// Token: 0x04000325 RID: 805
			public static Func<string, string> <1>__GetFileName;
		}
	}
}
