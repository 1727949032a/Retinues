using System;
using System.Linq;
using Retinues.Doctrines.Model;
using Retinues.Game;
using Retinues.Game.Events;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.Localization;

namespace Retinues.Doctrines.Catalog
{
	// Token: 0x020000EF RID: 239
	public sealed class BoundByHonor : Doctrine
	{
		// Token: 0x170003F7 RID: 1015
		// (get) Token: 0x06000963 RID: 2403 RVA: 0x0002D2DA File Offset: 0x0002B4DA
		public override TextObject Name
		{
			get
			{
				return L.T("bound_by_honor", "Bound by Honor");
			}
		}

		// Token: 0x170003F8 RID: 1016
		// (get) Token: 0x06000964 RID: 2404 RVA: 0x0002D2EB File Offset: 0x0002B4EB
		public override TextObject Description
		{
			get
			{
				return L.T("bound_by_honor_description", "+20% retinue morale.");
			}
		}

		// Token: 0x170003F9 RID: 1017
		// (get) Token: 0x06000965 RID: 2405 RVA: 0x0002D2FC File Offset: 0x0002B4FC
		public override int Column
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x170003FA RID: 1018
		// (get) Token: 0x06000966 RID: 2406 RVA: 0x0002D2FF File Offset: 0x0002B4FF
		public override int Row
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x020001D6 RID: 470
		public sealed class BBH_ProtectVillagersOrCaravans : Feat
		{
			// Token: 0x170004E2 RID: 1250
			// (get) Token: 0x06000D73 RID: 3443 RVA: 0x00037A09 File Offset: 0x00035C09
			public override TextObject Description
			{
				get
				{
					return L.T("bound_by_honor_protect_villagers_or_caravans", "Save 3 caravans or villager parties from enemy attacks.");
				}
			}

			// Token: 0x170004E3 RID: 1251
			// (get) Token: 0x06000D74 RID: 3444 RVA: 0x00037A1A File Offset: 0x00035C1A
			public override int Target
			{
				get
				{
					return 3;
				}
			}

			// Token: 0x06000D75 RID: 3445 RVA: 0x00037A20 File Offset: 0x00035C20
			public override void OnBattleEnd(Battle battle)
			{
				if (battle.IsLost)
				{
					return;
				}
				int amount = battle.AllyParties.Count((WParty p) => p.IsCaravan || p.IsVillager);
				base.AdvanceProgress(amount);
			}
		}

		// Token: 0x020001D7 RID: 471
		public sealed class BBH_RetinueOnlyMorale90For15Days : Feat
		{
			// Token: 0x170004E4 RID: 1252
			// (get) Token: 0x06000D77 RID: 3447 RVA: 0x00037A71 File Offset: 0x00035C71
			public override TextObject Description
			{
				get
				{
					return L.T("bound_by_honor_retinue_only_morale_90_for_15_days", "Maintain a retinue-only party's morale above 90 for 15 days.");
				}
			}

			// Token: 0x170004E5 RID: 1253
			// (get) Token: 0x06000D78 RID: 3448 RVA: 0x00037A82 File Offset: 0x00035C82
			public override int Target
			{
				get
				{
					return 15;
				}
			}

			// Token: 0x06000D79 RID: 3449 RVA: 0x00037A86 File Offset: 0x00035C86
			public override void OnDailyTick()
			{
				if (Player.Party.Morale > 90f && Player.Party.MemberRoster.RetinueRatio > 0.99f)
				{
					base.AdvanceProgress(1);
					return;
				}
				base.SetProgress(0);
			}
		}

		// Token: 0x020001D8 RID: 472
		public sealed class BBH_Defeat10Bandits : Feat
		{
			// Token: 0x170004E6 RID: 1254
			// (get) Token: 0x06000D7B RID: 3451 RVA: 0x00037AC7 File Offset: 0x00035CC7
			public override TextObject Description
			{
				get
				{
					return L.T("bound_by_honor_defeat_10_bandits", "Get rid of 10 bandit parties.");
				}
			}

			// Token: 0x170004E7 RID: 1255
			// (get) Token: 0x06000D7C RID: 3452 RVA: 0x00037AD8 File Offset: 0x00035CD8
			public override int Target
			{
				get
				{
					return 10;
				}
			}

			// Token: 0x06000D7D RID: 3453 RVA: 0x00037ADC File Offset: 0x00035CDC
			public override void OnBattleEnd(Battle battle)
			{
				if (battle.IsLost)
				{
					return;
				}
				int amount = battle.EnemyParties.Count((WParty p) => p.IsBandit);
				base.AdvanceProgress(amount);
			}
		}
	}
}
