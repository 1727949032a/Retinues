using System;
using Retinues.Game.Events;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.Localization;

namespace Retinues.Doctrines.Model
{
	// Token: 0x020000D9 RID: 217
	public abstract class Feat
	{
		// Token: 0x17000394 RID: 916
		// (get) Token: 0x060008D5 RID: 2261
		public abstract TextObject Description { get; }

		// Token: 0x17000395 RID: 917
		// (get) Token: 0x060008D6 RID: 2262 RVA: 0x0002C8DB File Offset: 0x0002AADB
		public virtual int Target
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000396 RID: 918
		// (get) Token: 0x060008D7 RID: 2263 RVA: 0x0002C8DE File Offset: 0x0002AADE
		public string Key
		{
			get
			{
				return base.GetType().FullName;
			}
		}

		// Token: 0x17000397 RID: 919
		// (get) Token: 0x060008D8 RID: 2264 RVA: 0x0002C8EB File Offset: 0x0002AAEB
		public Type DoctrineType
		{
			get
			{
				return base.GetType().DeclaringType;
			}
		}

		// Token: 0x17000398 RID: 920
		// (get) Token: 0x060008D9 RID: 2265 RVA: 0x0002C8F8 File Offset: 0x0002AAF8
		public int Progress
		{
			get
			{
				return DoctrineAPI.GetFeatProgress(base.GetType());
			}
		}

		// Token: 0x060008DA RID: 2266 RVA: 0x0002C905 File Offset: 0x0002AB05
		protected void SetProgress(int progress)
		{
			Log.Info(string.Format("Setting progress of feat {0} to {1}", base.GetType().FullName, progress));
			DoctrineAPI.SetFeatProgress(base.GetType(), progress);
		}

		// Token: 0x060008DB RID: 2267 RVA: 0x0002C933 File Offset: 0x0002AB33
		protected int AdvanceProgress(int amount = 1)
		{
			Log.Info(string.Format("Advancing progress of feat {0} by {1}", base.GetType().FullName, amount));
			return DoctrineAPI.AdvanceFeat(base.GetType(), amount);
		}

		// Token: 0x060008DC RID: 2268 RVA: 0x0002C961 File Offset: 0x0002AB61
		public virtual void OnRegister()
		{
		}

		// Token: 0x060008DD RID: 2269 RVA: 0x0002C963 File Offset: 0x0002AB63
		public virtual void OnUnregister()
		{
		}

		// Token: 0x060008DE RID: 2270 RVA: 0x0002C965 File Offset: 0x0002AB65
		public virtual void OnDailyTick()
		{
		}

		// Token: 0x060008DF RID: 2271 RVA: 0x0002C967 File Offset: 0x0002AB67
		public virtual void OnBattleEnd(Battle battle)
		{
		}

		// Token: 0x060008E0 RID: 2272 RVA: 0x0002C969 File Offset: 0x0002AB69
		public virtual void OnBattleStart(Battle battle)
		{
		}

		// Token: 0x060008E1 RID: 2273 RVA: 0x0002C96B File Offset: 0x0002AB6B
		public virtual void OnTournamentStart(Tournament tournament)
		{
		}

		// Token: 0x060008E2 RID: 2274 RVA: 0x0002C96D File Offset: 0x0002AB6D
		public virtual void OnTournamentFinished(Tournament tournament)
		{
		}

		// Token: 0x060008E3 RID: 2275 RVA: 0x0002C96F File Offset: 0x0002AB6F
		public virtual void OnSettlementOwnerChanged(SettlementOwnerChange change)
		{
		}

		// Token: 0x060008E4 RID: 2276 RVA: 0x0002C971 File Offset: 0x0002AB71
		public virtual void OnQuestCompleted(Quest quest)
		{
		}

		// Token: 0x060008E5 RID: 2277 RVA: 0x0002C973 File Offset: 0x0002AB73
		public virtual void OnTroopRecruited(WCharacter troop, int amount)
		{
		}

		// Token: 0x060008E6 RID: 2278 RVA: 0x0002C975 File Offset: 0x0002AB75
		public virtual void PlayerUpgradedTroops(WCharacter upgradeFromTroop, WCharacter upgradeToTroop, int number)
		{
		}

		// Token: 0x060008E7 RID: 2279 RVA: 0x0002C977 File Offset: 0x0002AB77
		public virtual void OnArenaStart(Combat combat)
		{
		}

		// Token: 0x060008E8 RID: 2280 RVA: 0x0002C979 File Offset: 0x0002AB79
		public virtual void OnArenaEnd(Combat combat)
		{
		}
	}
}
