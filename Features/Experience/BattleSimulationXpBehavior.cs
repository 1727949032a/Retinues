using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Retinues.Game.Events;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace Retinues.Features.Experience
{
	// Token: 0x020000C6 RID: 198
	[SafeClass]
	public class BattleSimulationXpBehavior : CampaignBehaviorBase
	{
		// Token: 0x0600081B RID: 2075 RVA: 0x00028E24 File Offset: 0x00027024
		public override void RegisterEvents()
		{
			CampaignEvents.MapEventStarted.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, PartyBase>(this.OnMapEventStarted));
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
		}

		// Token: 0x0600081C RID: 2076 RVA: 0x00028E54 File Offset: 0x00027054
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x0600081D RID: 2077 RVA: 0x00028E58 File Offset: 0x00027058
		[SafeMethod(null, true, null)]
		private void OnMapEventStarted(MapEvent me, PartyBase attackerParty, PartyBase defenderParty)
		{
			if (me == null)
			{
				return;
			}
			Battle battle = new Battle(me);
			List<WParty> list = battle.PartiesOnSide(BattleSideEnum.Attacker, true).ToList<WParty>();
			List<WParty> list2 = battle.PartiesOnSide(BattleSideEnum.Defender, true).ToList<WParty>();
			bool flag = list.Any((WParty p) => p.IsMainParty || p.PlayerFaction != null);
			bool flag2 = list2.Any((WParty p) => p.IsMainParty || p.PlayerFaction != null);
			if (!flag && !flag2)
			{
				this._snapshots.Remove(me);
				return;
			}
			IEnumerable<PartyBase> involvedParties = me.InvolvedParties;
			bool flag3;
			if (involvedParties == null)
			{
				flag3 = false;
			}
			else
			{
				flag3 = involvedParties.Any(delegate(PartyBase p)
				{
					if (p == null)
					{
						return false;
					}
					MobileParty mobileParty = p.MobileParty;
					return ((mobileParty != null) ? new bool?(mobileParty.IsMainParty) : null).GetValueOrDefault();
				});
			}
			bool flag4 = flag3;
			List<WParty> list3 = flag ? list : list2;
			List<WParty> list4 = flag ? list2 : list;
			List<ValueTuple<BasicCharacterObject, int, bool>> list5 = new List<ValueTuple<BasicCharacterObject, int, bool>>();
			int num = 0;
			foreach (WParty wparty in list3)
			{
				foreach (WRosterElement wrosterElement in wparty.MemberRoster.Elements)
				{
					if (wrosterElement.Number > 0 && !(wrosterElement.Troop == null))
					{
						list5.Add(new ValueTuple<BasicCharacterObject, int, bool>(wrosterElement.Troop.Base, wrosterElement.Number, wrosterElement.Troop.IsCustom));
						num += wrosterElement.Number;
					}
				}
			}
			int num2 = 0;
			foreach (WParty wparty2 in list4)
			{
				foreach (WRosterElement wrosterElement2 in wparty2.MemberRoster.Elements)
				{
					if (wrosterElement2.Number > 0 && !(wrosterElement2.Troop == null))
					{
						int num3 = Math.Max(0, wrosterElement2.Troop.Tier);
						num2 += wrosterElement2.Number * (num3 + 1);
					}
				}
			}
			this._snapshots[me] = new BattleSimulationXpBehavior.Snapshot
			{
				MainPartyInvolved = flag4,
				PlayerOnAttack = flag,
				PlayerTotalCount = num,
				PlayerElements = list5,
				EnemyBudgetUnits = num2
			};
			Log.Debug(string.Format("AutoResolveXP[Start]: playerInvolved={0}, atk={1}, def={2}, playerCount={3}, enemyUnits={4}", new object[]
			{
				flag4,
				list.Count,
				list2.Count,
				num,
				num2
			}));
		}

		// Token: 0x0600081E RID: 2078 RVA: 0x00029148 File Offset: 0x00027348
		[SafeMethod(null, true, null)]
		private void OnMapEventEnded(MapEvent me)
		{
			if (me == null)
			{
				return;
			}
			BattleSimulationXpBehavior.Snapshot snapshot;
			if (!this._snapshots.TryGetValue(me, out snapshot))
			{
				return;
			}
			Log.Debug("AutoResolveXP[End]: processing snapshot.");
			this._snapshots.Remove(me);
			if (snapshot.MainPartyInvolved && !me.IsPlayerSimulation)
			{
				Log.Debug("AutoResolveXP[End]: real battle, skipping.");
				return;
			}
			float num = (float)snapshot.EnemyBudgetUnits * 2.5f;
			if (num <= 0f || snapshot.PlayerTotalCount <= 0)
			{
				Log.Debug(string.Format("AutoResolveXP[End]: budget={0}, playerTotal={1} → nothing to award.", num, snapshot.PlayerTotalCount));
				return;
			}
			foreach (ValueTuple<BasicCharacterObject, int, bool> valueTuple in snapshot.PlayerElements)
			{
				BasicCharacterObject item = valueTuple.Item1;
				int item2 = valueTuple.Item2;
				bool item3 = valueTuple.Item3;
				int num2 = (int)Math.Round((double)num * (double)item2 / (double)snapshot.PlayerTotalCount);
				if (num2 > 0 && item3)
				{
					TroopXpBehavior.Add(new WCharacter(item.StringId), num2, false);
				}
			}
			Log.Debug("AutoResolveXP[End]: XP awarding complete.");
		}

		// Token: 0x0400021C RID: 540
		private const float XpPerTier = 2.5f;

		// Token: 0x0400021D RID: 541
		private readonly Dictionary<MapEvent, BattleSimulationXpBehavior.Snapshot> _snapshots = new Dictionary<MapEvent, BattleSimulationXpBehavior.Snapshot>();

		// Token: 0x02000195 RID: 405
		private sealed class Snapshot
		{
			// Token: 0x040004DA RID: 1242
			public bool MainPartyInvolved;

			// Token: 0x040004DB RID: 1243
			public bool PlayerOnAttack;

			// Token: 0x040004DC RID: 1244
			public int PlayerTotalCount;

			// Token: 0x040004DD RID: 1245
			public int EnemyBudgetUnits;

			// Token: 0x040004DE RID: 1246
			[TupleElementNames(new string[]
			{
				"Troop",
				"Count",
				"IsCustom"
			})]
			public List<ValueTuple<BasicCharacterObject, int, bool>> PlayerElements;
		}
	}
}
