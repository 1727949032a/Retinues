using System;
using System.Collections.Generic;
using System.Linq;
using Retinues.Doctrines.Model;
using Retinues.Game;
using Retinues.Game.Events;
using Retinues.Game.Wrappers;
using Retinues.Managers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace Retinues.Doctrines.Catalog
{
	// Token: 0x020000EB RID: 235
	public sealed class SteadfastSoldiers : Doctrine
	{
		// Token: 0x170003E3 RID: 995
		// (get) Token: 0x0600094B RID: 2379 RVA: 0x0002D1A2 File Offset: 0x0002B3A2
		public override TextObject Name
		{
			get
			{
				return L.T("steadfast_soldiers", "Steadfast Soldiers");
			}
		}

		// Token: 0x170003E4 RID: 996
		// (get) Token: 0x0600094C RID: 2380 RVA: 0x0002D1B3 File Offset: 0x0002B3B3
		public override TextObject Description
		{
			get
			{
				return L.T("steadfast_soldiers_description", "+10 skill points.");
			}
		}

		// Token: 0x170003E5 RID: 997
		// (get) Token: 0x0600094D RID: 2381 RVA: 0x0002D1C4 File Offset: 0x0002B3C4
		public override int Column
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x170003E6 RID: 998
		// (get) Token: 0x0600094E RID: 2382 RVA: 0x0002D1C7 File Offset: 0x0002B3C7
		public override int Row
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x020001CA RID: 458
		public sealed class SS_TroopsMaxedSkills : Feat
		{
			// Token: 0x170004CA RID: 1226
			// (get) Token: 0x06000D43 RID: 3395 RVA: 0x00037419 File Offset: 0x00035619
			public override TextObject Description
			{
				get
				{
					return L.T("steadfast_soldiers_troops_maxed_skills", "Max out the skills of 15 custom troops.");
				}
			}

			// Token: 0x170004CB RID: 1227
			// (get) Token: 0x06000D44 RID: 3396 RVA: 0x0003742A File Offset: 0x0003562A
			public override int Target
			{
				get
				{
					return 15;
				}
			}

			// Token: 0x06000D45 RID: 3397 RVA: 0x00037430 File Offset: 0x00035630
			public override void OnDailyTick()
			{
				int num = 0;
				using (IEnumerator<WCharacter> enumerator = Player.Troops.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (SkillManager.SkillPointsLeft(enumerator.Current) == 0)
						{
							num++;
						}
					}
				}
				base.SetProgress(num);
			}
		}

		// Token: 0x020001CB RID: 459
		public sealed class SS_SiegeDefenseOnlyCustom : Feat
		{
			// Token: 0x170004CC RID: 1228
			// (get) Token: 0x06000D47 RID: 3399 RVA: 0x00037490 File Offset: 0x00035690
			public override TextObject Description
			{
				get
				{
					return L.T("steadfast_soldiers_siege_defense_only_custom", "Win a siege defense using only custom troops.");
				}
			}

			// Token: 0x170004CD RID: 1229
			// (get) Token: 0x06000D48 RID: 3400 RVA: 0x000374A1 File Offset: 0x000356A1
			public override int Target
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x06000D49 RID: 3401 RVA: 0x000374A4 File Offset: 0x000356A4
			public override void OnBattleEnd(Battle battle)
			{
				if (battle.IsLost)
				{
					return;
				}
				if (!battle.PlayerIsDefender)
				{
					return;
				}
				if (!battle.IsSiege)
				{
					return;
				}
				if (Player.Party.MemberRoster.CustomRatio < 0.99f)
				{
					return;
				}
				base.AdvanceProgress(1);
			}
		}

		// Token: 0x020001CC RID: 460
		public sealed class SS_RaiseSecurityTo60 : Feat
		{
			// Token: 0x170004CE RID: 1230
			// (get) Token: 0x06000D4B RID: 3403 RVA: 0x000374E8 File Offset: 0x000356E8
			public override TextObject Description
			{
				get
				{
					return L.T("steadfast_soldiers_raise_security_60", "Raise the security value of a fief to 60.");
				}
			}

			// Token: 0x170004CF RID: 1231
			// (get) Token: 0x06000D4C RID: 3404 RVA: 0x000374F9 File Offset: 0x000356F9
			public override int Target
			{
				get
				{
					return 60;
				}
			}

			// Token: 0x06000D4D RID: 3405 RVA: 0x00037500 File Offset: 0x00035700
			public override void OnDailyTick()
			{
				if (Campaign.Current == null || Clan.PlayerClan == null)
				{
					return;
				}
				List<Town> list = (from s in Clan.PlayerClan.Fiefs
				where s.IsTown || s.IsCastle
				select s).ToList<Town>();
				if (list.Count == 0)
				{
					return;
				}
				int progress = (int)(from s in list
				select s.Security).DefaultIfEmpty(0f).Max();
				base.SetProgress(progress);
			}
		}
	}
}
