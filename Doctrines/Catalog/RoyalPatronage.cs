using System;
using System.Linq;
using Retinues.Configuration;
using Retinues.Doctrines.Model;
using Retinues.Game;
using Retinues.Game.Events;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.Localization;

namespace Retinues.Doctrines.Catalog
{
	// Token: 0x020000E4 RID: 228
	public sealed class RoyalPatronage : Doctrine
	{
		// Token: 0x170003BF RID: 959
		// (get) Token: 0x06000920 RID: 2336 RVA: 0x0002CFC6 File Offset: 0x0002B1C6
		public override TextObject Name
		{
			get
			{
				return L.T("royal_patronage", "Royal Patronage");
			}
		}

		// Token: 0x170003C0 RID: 960
		// (get) Token: 0x06000921 RID: 2337 RVA: 0x0002CFD7 File Offset: 0x0002B1D7
		public override TextObject Description
		{
			get
			{
				return L.T("royal_patronage_description", "20% rebate on kingdom culture gear.");
			}
		}

		// Token: 0x170003C1 RID: 961
		// (get) Token: 0x06000922 RID: 2338 RVA: 0x0002CFE8 File Offset: 0x0002B1E8
		public override int Column
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x170003C2 RID: 962
		// (get) Token: 0x06000923 RID: 2339 RVA: 0x0002CFEB File Offset: 0x0002B1EB
		public override int Row
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x170003C3 RID: 963
		// (get) Token: 0x06000924 RID: 2340 RVA: 0x0002CFEE File Offset: 0x0002B1EE
		public override bool IsDisabled
		{
			get
			{
				return !Config.EquippingTroopsCostsGold || Config.EquipmentCostMultiplier <= 0f;
			}
		}

		// Token: 0x170003C4 RID: 964
		// (get) Token: 0x06000925 RID: 2341 RVA: 0x0002D012 File Offset: 0x0002B212
		public override TextObject DisabledMessage
		{
			get
			{
				return L.T("cultural_pride_disabled_message", "Disabled: equipment costs are disabled in config.");
			}
		}

		// Token: 0x020001B5 RID: 437
		public sealed class RP_Recruit100CustomKingdom : Feat
		{
			// Token: 0x170004A0 RID: 1184
			// (get) Token: 0x06000CEE RID: 3310 RVA: 0x00036942 File Offset: 0x00034B42
			public override TextObject Description
			{
				get
				{
					return L.T("royal_patronage_recruit_100_custom_kingdom", "Recruit 100 custom kingdom troops.");
				}
			}

			// Token: 0x170004A1 RID: 1185
			// (get) Token: 0x06000CEF RID: 3311 RVA: 0x00036953 File Offset: 0x00034B53
			public override int Target
			{
				get
				{
					return 100;
				}
			}

			// Token: 0x06000CF0 RID: 3312 RVA: 0x00036957 File Offset: 0x00034B57
			public override void OnTroopRecruited(WCharacter troop, int amount)
			{
				if (troop.IsCustom && troop.Faction == Player.Kingdom)
				{
					base.AdvanceProgress(amount);
				}
			}
		}

		// Token: 0x020001B6 RID: 438
		public sealed class RP_CompanionGovernor30Days : Feat
		{
			// Token: 0x170004A2 RID: 1186
			// (get) Token: 0x06000CF2 RID: 3314 RVA: 0x00036983 File Offset: 0x00034B83
			public override TextObject Description
			{
				get
				{
					return L.T("royal_patronage_companion_governor_30_days", "Have a companion of the same culture as your kingdom govern a kingdom fief for 30 days.");
				}
			}

			// Token: 0x170004A3 RID: 1187
			// (get) Token: 0x06000CF3 RID: 3315 RVA: 0x00036994 File Offset: 0x00034B94
			public override int Target
			{
				get
				{
					return 30;
				}
			}

			// Token: 0x06000CF4 RID: 3316 RVA: 0x00036998 File Offset: 0x00034B98
			public override void OnDailyTick()
			{
				if (Player.Kingdom == null)
				{
					return;
				}
				WFaction clan = Player.Clan;
				foreach (WSettlement wsettlement in ((clan != null) ? clan.Settlements : null))
				{
					WHero governor = wsettlement.Governor;
					StringIdentifier left = (governor != null) ? governor.Culture : null;
					WFaction kingdom = Player.Kingdom;
					if (left == ((kingdom != null) ? kingdom.Culture : null))
					{
						base.AdvanceProgress(1);
						break;
					}
				}
			}
		}

		// Token: 0x020001B7 RID: 439
		public sealed class RP_1000KillsCustomKingdom : Feat
		{
			// Token: 0x170004A4 RID: 1188
			// (get) Token: 0x06000CF6 RID: 3318 RVA: 0x00036A38 File Offset: 0x00034C38
			public override TextObject Description
			{
				get
				{
					return L.T("royal_patronage_1000_kills_custom_kingdom", "Get 1000 kills with custom kingdom troops.");
				}
			}

			// Token: 0x170004A5 RID: 1189
			// (get) Token: 0x06000CF7 RID: 3319 RVA: 0x00036A49 File Offset: 0x00034C49
			public override int Target
			{
				get
				{
					return 1000;
				}
			}

			// Token: 0x06000CF8 RID: 3320 RVA: 0x00036A50 File Offset: 0x00034C50
			public override void OnBattleEnd(Battle battle)
			{
				if (Player.Kingdom == null)
				{
					return;
				}
				int amount = battle.Kills.Count((Combat.Kill k) => k.KillerIsPlayerTroop && !k.KillerIsPlayer && k.Killer.Faction == Player.Kingdom);
				base.AdvanceProgress(amount);
			}
		}
	}
}
