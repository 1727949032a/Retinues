using System;
using System.Collections.Generic;
using HarmonyLib;
using Retinues.Mods.BanditMilitias;
using Retinues.Mods.NavalDLC;
using Retinues.Mods.Shokuho;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;

namespace Retinues.Mods
{
	// Token: 0x02000057 RID: 87
	[SafeClass]
	public class ModCompatibility : ModuleChecker
	{
		// Token: 0x17000055 RID: 85
		// (get) Token: 0x0600019B RID: 411 RVA: 0x0000B98C File Offset: 0x00009B8C
		public static bool HasNavalDLC
		{
			get
			{
				return ModuleChecker.IsLoaded("NavalDLC");
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x0600019C RID: 412 RVA: 0x0000B998 File Offset: 0x00009B98
		public static bool HasBanditMilitias
		{
			get
			{
				return ModuleChecker.IsLoaded("BanditMilitias");
			}
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x0600019D RID: 413 RVA: 0x0000B9A4 File Offset: 0x00009BA4
		public static bool HasShokuho
		{
			get
			{
				return ModuleChecker.IsLoaded("Shokuho");
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x0600019E RID: 414 RVA: 0x0000B9B0 File Offset: 0x00009BB0
		public static bool HasImprovedGarrisons
		{
			get
			{
				return ModuleChecker.IsLoaded("ImprovedGarrisons");
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x0600019F RID: 415 RVA: 0x0000B9BC File Offset: 0x00009BBC
		public static bool HasTier7Unlocker
		{
			get
			{
				return ModuleChecker.IsLoaded("T7TroopUnlocker");
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060001A0 RID: 416 RVA: 0x0000B9C8 File Offset: 0x00009BC8
		public static bool ForceClanTabsReset
		{
			get
			{
				return ModuleChecker.IsLoaded("BannerKings");
			}
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060001A1 RID: 417 RVA: 0x0000B9D4 File Offset: 0x00009BD4
		public static bool SkipItemCultureChecks
		{
			get
			{
				return ModuleChecker.IsLoaded(new string[]
				{
					"Shokuho",
					"AD1259"
				});
			}
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x0000B9F1 File Offset: 0x00009BF1
		public static void AddBehaviors(CampaignGameStarter cs)
		{
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x0000B9F3 File Offset: 0x00009BF3
		public static void AddPatches(Harmony harmony)
		{
			if (ModCompatibility.HasShokuho)
			{
				ShokuhoEquipmentPatcher.TryPatch(harmony);
			}
			if (ModCompatibility.HasBanditMilitias)
			{
				BanditMilitiasTroopsPatcher.TryPatch(harmony);
			}
			if (ModCompatibility.HasNavalDLC)
			{
				NavalDlcShipTradePatcher.TryPatch(harmony);
			}
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x0000BA1C File Offset: 0x00009C1C
		public static void IncompatibilityCheck()
		{
			foreach (string text in ModCompatibility.IncompatibleMods)
			{
				ModuleChecker.ModuleEntry module = ModuleChecker.GetModule(text);
				if (module != null)
				{
					if (text.Contains("Retinues."))
					{
						Log.Critical(string.Format("[Retinues] WARNING: detected legacy mod '{0}'. Please uninstall it to avoid conflicts.", module));
					}
					else
					{
						Log.Critical(string.Format("[Retinues] WARNING: incompatible mod detected: '{0}'.", module));
					}
				}
			}
		}

		// Token: 0x040000A5 RID: 165
		private static readonly List<string> IncompatibleMods = new List<string>(2)
		{
			"Retinues.Core",
			"Retinues.MCM"
		};
	}
}
