using System;
using Retinues.Doctrines.Model;
using Retinues.Game;
using Retinues.Game.Wrappers;
using Retinues.Managers;
using Retinues.Utils;
using TaleWorlds.Localization;

namespace Retinues.Doctrines.Catalog
{
	// Token: 0x020000E9 RID: 233
	public sealed class Captains : Doctrine
	{
		// Token: 0x170003DB RID: 987
		// (get) Token: 0x06000941 RID: 2369 RVA: 0x0002D142 File Offset: 0x0002B342
		public override TextObject Name
		{
			get
			{
				return L.T("captains", "Captains");
			}
		}

		// Token: 0x170003DC RID: 988
		// (get) Token: 0x06000942 RID: 2370 RVA: 0x0002D153 File Offset: 0x0002B353
		public override TextObject Description
		{
			get
			{
				return L.T("captains_description", "Unlocks Captains.");
			}
		}

		// Token: 0x170003DD RID: 989
		// (get) Token: 0x06000943 RID: 2371 RVA: 0x0002D164 File Offset: 0x0002B364
		public override int Column
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x170003DE RID: 990
		// (get) Token: 0x06000944 RID: 2372 RVA: 0x0002D167 File Offset: 0x0002B367
		public override int Row
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x020001C4 RID: 452
		public sealed class CP_MaxOutEliteAndBasic : Feat
		{
			// Token: 0x170004BE RID: 1214
			// (get) Token: 0x06000D2A RID: 3370 RVA: 0x0003718E File Offset: 0x0003538E
			public override TextObject Description
			{
				get
				{
					return L.T("captains_max_out_elite_basic", "Max out the skills of a T6 elite troop and a T5 basic troop.");
				}
			}

			// Token: 0x170004BF RID: 1215
			// (get) Token: 0x06000D2B RID: 3371 RVA: 0x0003719F File Offset: 0x0003539F
			public override int Target
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x06000D2C RID: 3372 RVA: 0x000371A4 File Offset: 0x000353A4
			public override void OnDailyTick()
			{
				if (base.Progress >= this.Target)
				{
					return;
				}
				bool flag = false;
				bool flag2 = false;
				foreach (WCharacter wcharacter in Player.Troops)
				{
					if (!(wcharacter == null) && wcharacter.IsValid)
					{
						if (!flag && wcharacter.IsElite && wcharacter.Tier >= 6 && SkillManager.SkillPointsLeft(wcharacter) == 0)
						{
							flag = true;
						}
						if (!flag2 && !wcharacter.IsElite && wcharacter.Tier >= 5 && SkillManager.SkillPointsLeft(wcharacter) == 0)
						{
							flag2 = true;
						}
						if (flag && flag2)
						{
							break;
						}
					}
				}
				if (flag && flag2)
				{
					base.SetProgress(1);
				}
			}
		}

		// Token: 0x020001C5 RID: 453
		public sealed class CP_Earn200InfluenceInADay : Feat
		{
			// Token: 0x170004C0 RID: 1216
			// (get) Token: 0x06000D2E RID: 3374 RVA: 0x00037264 File Offset: 0x00035464
			public override TextObject Description
			{
				get
				{
					return L.T("captains_earn_200_influence_in_a_day", "Earn 200 influence in a single day.");
				}
			}

			// Token: 0x170004C1 RID: 1217
			// (get) Token: 0x06000D2F RID: 3375 RVA: 0x00037275 File Offset: 0x00035475
			public override int Target
			{
				get
				{
					return 200;
				}
			}

			// Token: 0x06000D30 RID: 3376 RVA: 0x0003727C File Offset: 0x0003547C
			public override void OnRegister()
			{
				this._previousInfluence = Player.Influence;
				this._initialized = true;
			}

			// Token: 0x06000D31 RID: 3377 RVA: 0x00037290 File Offset: 0x00035490
			public override void OnDailyTick()
			{
				if (!this._initialized)
				{
					this._previousInfluence = Player.Influence;
					this._initialized = true;
					return;
				}
				int influence = Player.Influence;
				int num = influence - this._previousInfluence;
				if (num > base.Progress)
				{
					base.SetProgress(num);
				}
				this._previousInfluence = influence;
			}

			// Token: 0x0400051D RID: 1309
			private int _previousInfluence;

			// Token: 0x0400051E RID: 1310
			private bool _initialized;
		}

		// Token: 0x020001C6 RID: 454
		public sealed class CP_Have4000Renown : Feat
		{
			// Token: 0x170004C2 RID: 1218
			// (get) Token: 0x06000D33 RID: 3379 RVA: 0x000372E6 File Offset: 0x000354E6
			public override TextObject Description
			{
				get
				{
					return L.T("captains_have_4000_renown", "Have 4 000 renown.");
				}
			}

			// Token: 0x170004C3 RID: 1219
			// (get) Token: 0x06000D34 RID: 3380 RVA: 0x000372F7 File Offset: 0x000354F7
			public override int Target
			{
				get
				{
					return 4000;
				}
			}

			// Token: 0x06000D35 RID: 3381 RVA: 0x00037300 File Offset: 0x00035500
			public override void OnDailyTick()
			{
				int num = (int)Player.Renown;
				if (num > base.Progress)
				{
					base.SetProgress(num);
				}
			}
		}
	}
}
