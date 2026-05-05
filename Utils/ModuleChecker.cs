using System;
using System.Collections.Generic;
using System.IO;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;

namespace Retinues.Utils
{
	// Token: 0x02000024 RID: 36
	[SafeClass]
	public class ModuleChecker
	{
		// Token: 0x0600008D RID: 141 RVA: 0x000042F8 File Offset: 0x000024F8
		public static ModuleChecker.ModuleEntry GetModule(string id)
		{
			foreach (ModuleChecker.ModuleEntry moduleEntry in ModuleChecker.GetActiveModules())
			{
				if (moduleEntry.Id.Equals(id, StringComparison.OrdinalIgnoreCase))
				{
					return moduleEntry;
				}
			}
			return null;
		}

		// Token: 0x0600008E RID: 142 RVA: 0x0000435C File Offset: 0x0000255C
		public static bool IsLoaded(string id)
		{
			return ModuleChecker.GetModule(id) != null;
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00004368 File Offset: 0x00002568
		public static bool IsLoaded(params string[] ids)
		{
			if (ids == null || ids.Length == 0)
			{
				return false;
			}
			foreach (string text in ids)
			{
				if (!string.IsNullOrWhiteSpace(text) && ModuleChecker.GetModule(text) != null)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000090 RID: 144 RVA: 0x000043A8 File Offset: 0x000025A8
		public static List<ModuleChecker.ModuleEntry> GetActiveModules()
		{
			if (ModuleChecker._cachedActiveModules != null)
			{
				return ModuleChecker._cachedActiveModules;
			}
			List<ModuleChecker.ModuleEntry> list = new List<ModuleChecker.ModuleEntry>();
			string[] array;
			try
			{
				array = Utilities.GetModulesNames();
			}
			catch
			{
				array = Array.Empty<string>();
			}
			foreach (string text in array)
			{
				if (!string.IsNullOrWhiteSpace(text))
				{
					string text2 = text;
					ApplicationVersion appVersion = ApplicationVersion.Empty;
					bool isOfficial = ModuleChecker.IsOfficialModuleId(text);
					string text3 = null;
					try
					{
						ModuleInfo moduleInfo = ModuleHelper.GetModuleInfo(text);
						if (moduleInfo != null)
						{
							text2 = (moduleInfo.Name ?? text2);
							isOfficial = moduleInfo.IsOfficial;
							appVersion = moduleInfo.Version;
							text3 = moduleInfo.FolderPath;
						}
					}
					catch
					{
					}
					if (string.IsNullOrEmpty(text3))
					{
						text3 = System.IO.Path.Combine(System.IO.Path.Combine(BasePath.Name, "Modules"), text);
					}
					list.Add(new ModuleChecker.ModuleEntry
					{
						Id = text,
						Name = text2,
						AppVersion = appVersion,
						Path = text3,
						IsOfficial = isOfficial
					});
				}
			}
			ModuleChecker._cachedActiveModules = list;
			return list;
		}

		// Token: 0x06000091 RID: 145 RVA: 0x000044CC File Offset: 0x000026CC
		private static bool IsOfficialModuleId(string id)
		{
			return id == "Native" || id == "SandboxCore" || id == "Sandbox" || id == "StoryMode" || id == "CustomBattle";
		}

		// Token: 0x04000020 RID: 32
		public const string UnknownVersionString = "unknown";

		// Token: 0x04000021 RID: 33
		private static List<ModuleChecker.ModuleEntry> _cachedActiveModules;

		// Token: 0x02000100 RID: 256
		public sealed class ModuleEntry
		{
			// Token: 0x1700044C RID: 1100
			// (get) Token: 0x06000A2D RID: 2605 RVA: 0x000306EE File Offset: 0x0002E8EE
			// (set) Token: 0x06000A2E RID: 2606 RVA: 0x000306F6 File Offset: 0x0002E8F6
			public string Id { get; set; }

			// Token: 0x1700044D RID: 1101
			// (get) Token: 0x06000A2F RID: 2607 RVA: 0x000306FF File Offset: 0x0002E8FF
			// (set) Token: 0x06000A30 RID: 2608 RVA: 0x00030707 File Offset: 0x0002E907
			public string Name { get; set; }

			// Token: 0x1700044E RID: 1102
			// (get) Token: 0x06000A31 RID: 2609 RVA: 0x00030710 File Offset: 0x0002E910
			// (set) Token: 0x06000A32 RID: 2610 RVA: 0x00030718 File Offset: 0x0002E918
			public ApplicationVersion AppVersion { get; set; }

			// Token: 0x1700044F RID: 1103
			// (get) Token: 0x06000A33 RID: 2611 RVA: 0x00030724 File Offset: 0x0002E924
			public string Version
			{
				get
				{
					if (!(this.AppVersion == ApplicationVersion.Empty))
					{
						return this.AppVersion.ToString();
					}
					return "unknown";
				}
			}

			// Token: 0x17000450 RID: 1104
			// (get) Token: 0x06000A34 RID: 2612 RVA: 0x0003075D File Offset: 0x0002E95D
			// (set) Token: 0x06000A35 RID: 2613 RVA: 0x00030765 File Offset: 0x0002E965
			public string Path { get; set; }

			// Token: 0x17000451 RID: 1105
			// (get) Token: 0x06000A36 RID: 2614 RVA: 0x0003076E File Offset: 0x0002E96E
			// (set) Token: 0x06000A37 RID: 2615 RVA: 0x00030776 File Offset: 0x0002E976
			public bool IsOfficial { get; set; }

			// Token: 0x06000A38 RID: 2616 RVA: 0x00030780 File Offset: 0x0002E980
			public override string ToString()
			{
				return string.Concat(new string[]
				{
					this.Id,
					" [",
					this.Version,
					"] - ",
					this.Name,
					this.IsOfficial ? " (official)" : ""
				});
			}
		}
	}
}
