using System;
using System.Collections.Generic;
using System.Text;
using Retinues.Game;
using Retinues.Game.Events;
using Retinues.Game.Wrappers;
using Retinues.GUI.Helpers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace Retinues.Features.Statistics
{
	// Token: 0x020000BD RID: 189
	[SafeClass]
	public sealed class TroopStatisticsBehavior : CampaignBehaviorBase
	{
		// Token: 0x0600078F RID: 1935 RVA: 0x00026C5B File Offset: 0x00024E5B
		public TroopStatisticsBehavior()
		{
			TroopStatisticsBehavior._instance = this;
		}

		// Token: 0x06000790 RID: 1936 RVA: 0x00026C74 File Offset: 0x00024E74
		public override void RegisterEvents()
		{
		}

		// Token: 0x06000791 RID: 1937 RVA: 0x00026C76 File Offset: 0x00024E76
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<string, TroopCombatStats>>("_retinuesTroopStatistics", ref this._stats);
		}

		// Token: 0x06000792 RID: 1938 RVA: 0x00026C8C File Offset: 0x00024E8C
		public static void Clear(WCharacter troop)
		{
			try
			{
				TroopStatisticsBehavior instance = TroopStatisticsBehavior.GetInstance();
				if (instance != null && !(troop == null))
				{
					instance._stats.Remove(troop.StringId);
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x06000793 RID: 1939 RVA: 0x00026CE0 File Offset: 0x00024EE0
		public static void RecordFromMission(Battle battle, IReadOnlyList<Combat.Kill> kills)
		{
			try
			{
				if (battle != null && kills != null && kills.Count != 0)
				{
					TroopStatisticsBehavior instance = TroopStatisticsBehavior.GetInstance();
					if (instance != null)
					{
						instance.RecordInternal(battle, kills);
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x06000794 RID: 1940 RVA: 0x00026D30 File Offset: 0x00024F30
		public static void ShowForTroop(WCharacter troop)
		{
			try
			{
				TroopStatisticsBehavior instance = TroopStatisticsBehavior.GetInstance();
				if (instance != null)
				{
					instance.ShowForTroopInternal(troop);
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x06000795 RID: 1941 RVA: 0x00026D70 File Offset: 0x00024F70
		private static TroopStatisticsBehavior GetInstance()
		{
			if (TroopStatisticsBehavior._instance != null)
			{
				return TroopStatisticsBehavior._instance;
			}
			Campaign campaign = Campaign.Current;
			if (campaign == null)
			{
				return null;
			}
			TroopStatisticsBehavior._instance = campaign.GetCampaignBehavior<TroopStatisticsBehavior>();
			return TroopStatisticsBehavior._instance;
		}

		// Token: 0x06000796 RID: 1942 RVA: 0x00026DA8 File Offset: 0x00024FA8
		private void RecordInternal(Battle battle, IReadOnlyList<Combat.Kill> kills)
		{
			HashSet<string> seenThisBattle = new HashSet<string>();
			foreach (Combat.Kill kill in kills)
			{
				bool flag = kill.KillerIsPlayer || kill.KillerIsPlayerTroop || kill.KillerIsAllyTroop;
				bool flag2 = kill.VictimIsPlayer || kill.VictimIsPlayerTroop || kill.VictimIsAllyTroop;
				if (flag && !string.IsNullOrEmpty(kill.KillerCharacterId))
				{
					this.UpdateForParticipant(battle, kill, kill.KillerCharacterId, true, seenThisBattle);
				}
				if (flag2 && !string.IsNullOrEmpty(kill.VictimCharacterId))
				{
					this.UpdateForParticipant(battle, kill, kill.VictimCharacterId, false, seenThisBattle);
				}
			}
		}

		// Token: 0x06000797 RID: 1943 RVA: 0x00026E64 File Offset: 0x00025064
		private void UpdateForParticipant(Battle battle, Combat.Kill kill, string troopId, bool isKiller, HashSet<string> seenThisBattle)
		{
			TroopCombatStats orCreateStats = this.GetOrCreateStats(troopId);
			if (seenThisBattle.Add(troopId))
			{
				TroopStatisticsBehavior.BumpBattleCounters(orCreateStats, battle);
			}
			string text = isKiller ? kill.VictimCharacterId : kill.KillerCharacterId;
			if (!string.IsNullOrEmpty(text))
			{
				string text2 = TroopStatisticsBehavior.ResolveFactionName(new WCharacter(text));
				if (!string.IsNullOrEmpty(text2))
				{
					TroopStatisticsBehavior.Increment(orCreateStats.FactionsFought, text2);
				}
				if (isKiller)
				{
					orCreateStats.TotalKills++;
					TroopStatisticsBehavior.Increment(orCreateStats.KillsByTroopId, text);
					return;
				}
				orCreateStats.TotalDeaths++;
				TroopStatisticsBehavior.Increment(orCreateStats.DeathsByTroopId, text);
			}
		}

		// Token: 0x06000798 RID: 1944 RVA: 0x00026F00 File Offset: 0x00025100
		private TroopCombatStats GetOrCreateStats(string troopId)
		{
			if (string.IsNullOrEmpty(troopId))
			{
				troopId = "?";
			}
			TroopCombatStats troopCombatStats;
			if (!this._stats.TryGetValue(troopId, out troopCombatStats))
			{
				troopCombatStats = new TroopCombatStats
				{
					TroopId = troopId
				};
				this._stats[troopId] = troopCombatStats;
			}
			return troopCombatStats;
		}

		// Token: 0x06000799 RID: 1945 RVA: 0x00026F48 File Offset: 0x00025148
		private static void BumpBattleCounters(TroopCombatStats stats, Battle battle)
		{
			stats.TotalBattles++;
			if (battle.IsWon)
			{
				stats.BattlesWon++;
			}
			else if (battle.IsLost)
			{
				stats.BattlesLost++;
			}
			if (battle.IsSiege)
			{
				stats.SiegeBattles++;
				return;
			}
			if (battle.IsVillageRaid)
			{
				stats.VillageRaidBattles++;
				return;
			}
			if (battle.IsHideout)
			{
				stats.HideoutBattles++;
				return;
			}
			if (battle.IsFieldBattle)
			{
				stats.FieldBattles++;
				return;
			}
			stats.OtherBattles++;
		}

		// Token: 0x0600079A RID: 1946 RVA: 0x00026FFC File Offset: 0x000251FC
		private static string ResolveFactionName(WCharacter other)
		{
			string result;
			try
			{
				BaseFaction baseFaction = (other != null) ? other.Faction : null;
				if (baseFaction == null)
				{
					result = null;
				}
				else
				{
					string name = baseFaction.Name;
					if (name != null && !string.IsNullOrEmpty(name.ToString()))
					{
						result = name.ToString();
					}
					else
					{
						result = baseFaction.StringId;
					}
				}
			}
			catch
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600079B RID: 1947 RVA: 0x00027064 File Offset: 0x00025264
		private static void Increment(Dictionary<string, int> dict, string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				key = "?";
			}
			int num;
			if (!dict.TryGetValue(key, out num))
			{
				num = 0;
			}
			dict[key] = num + 1;
		}

		// Token: 0x0600079C RID: 1948 RVA: 0x00027098 File Offset: 0x00025298
		private void ShowForTroopInternal(WCharacter troop)
		{
			if (troop == null)
			{
				return;
			}
			TextObject title = L.T("battle_record_title", "Battle Record");
			TroopCombatStats troopCombatStats;
			if (!this._stats.TryGetValue(troop.StringId, out troopCombatStats))
			{
				Notifications.Popup(title, L.T("battle_record_no_data", "No battle data available yet for this troop."), null, true);
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(L.T("battle_record_battles_summary", "Fought in {BATTLES} battles ({WON} victories, {LOST} defeats).").SetTextVariable("BATTLES", troopCombatStats.TotalBattles).SetTextVariable("WON", troopCombatStats.BattlesWon).SetTextVariable("LOST", troopCombatStats.BattlesLost).ToString());
			stringBuilder.AppendLine();
			if (troopCombatStats.TotalBattles > 0)
			{
				stringBuilder.AppendLine(L.S("battle_record_battle_types", "Battle types:"));
				if (troopCombatStats.FieldBattles > 0)
				{
					stringBuilder.AppendLine(L.T("battle_record_field_line", "  • Field battles: {BATTLES}").SetTextVariable("BATTLES", troopCombatStats.FieldBattles).ToString());
				}
				if (troopCombatStats.SiegeBattles > 0)
				{
					stringBuilder.AppendLine(L.T("battle_record_siege_line", "  • Sieges: {BATTLES}").SetTextVariable("BATTLES", troopCombatStats.SiegeBattles).ToString());
				}
				if (troopCombatStats.HideoutBattles > 0)
				{
					stringBuilder.AppendLine(L.T("battle_record_hideout_line", "  • Hideouts: {BATTLES}").SetTextVariable("BATTLES", troopCombatStats.HideoutBattles).ToString());
				}
				if (troopCombatStats.VillageRaidBattles > 0)
				{
					stringBuilder.AppendLine(L.T("battle_record_village_raid_line", "  • Village raids: {BATTLES}").SetTextVariable("BATTLES", troopCombatStats.VillageRaidBattles).ToString());
				}
				if (troopCombatStats.OtherBattles > 0)
				{
					stringBuilder.AppendLine(L.T("battle_record_other_line", "  • Other: {BATTLES}").SetTextVariable("BATTLES", troopCombatStats.OtherBattles).ToString());
				}
			}
			stringBuilder.AppendLine();
			stringBuilder.AppendLine(L.T("stats_kills_deaths_summary", "Killed {KILLS} enemies and suffered {DEATHS} casualties.").SetTextVariable("KILLS", troopCombatStats.TotalKills).SetTextVariable("DEATHS", troopCombatStats.TotalDeaths).ToString());
			stringBuilder.AppendLine();
			KeyValuePair<string, int> maxEntry = TroopStatisticsBehavior.GetMaxEntry(troopCombatStats.FactionsFought);
			if (maxEntry.Key != null)
			{
				stringBuilder.AppendLine(L.T("stats_most_battled_faction", "Most battled: {FACTION} ({COUNT} encounters)").SetTextVariable("FACTION", maxEntry.Key).SetTextVariable("COUNT", maxEntry.Value).ToString());
			}
			KeyValuePair<string, int> maxEntry2 = TroopStatisticsBehavior.GetMaxEntry(troopCombatStats.KillsByTroopId);
			if (maxEntry2.Key != null)
			{
				WCharacter wcharacter = new WCharacter(maxEntry2.Key);
				stringBuilder.AppendLine(L.T("stats_most_slain_enemy", "Most slain: {ENEMY} ({COUNT} slain)").SetTextVariable("ENEMY", wcharacter.Name).SetTextVariable("COUNT", maxEntry2.Value).ToString());
			}
			KeyValuePair<string, int> maxEntry3 = TroopStatisticsBehavior.GetMaxEntry(troopCombatStats.DeathsByTroopId);
			if (maxEntry3.Key != null)
			{
				WCharacter wcharacter2 = new WCharacter(maxEntry3.Key);
				stringBuilder.AppendLine(L.T("stats_most_feared_enemy", "Most feared: {ENEMY} ({COUNT} casualties)").SetTextVariable("ENEMY", wcharacter2.Name).SetTextVariable("COUNT", maxEntry3.Value).ToString());
			}
			TextObject description = new TextObject(stringBuilder.ToString(), null);
			Notifications.Popup(title, description, null, true);
		}

		// Token: 0x0600079D RID: 1949 RVA: 0x000273E0 File Offset: 0x000255E0
		private static KeyValuePair<string, int> GetMaxEntry(Dictionary<string, int> dict)
		{
			if (dict == null || dict.Count == 0)
			{
				return default(KeyValuePair<string, int>);
			}
			string text = null;
			int num = 0;
			foreach (KeyValuePair<string, int> keyValuePair in dict)
			{
				if (keyValuePair.Value > num)
				{
					text = keyValuePair.Key;
					num = keyValuePair.Value;
				}
			}
			if (text != null)
			{
				return new KeyValuePair<string, int>(text, num);
			}
			return default(KeyValuePair<string, int>);
		}

		// Token: 0x04000201 RID: 513
		private static TroopStatisticsBehavior _instance;

		// Token: 0x04000202 RID: 514
		private Dictionary<string, TroopCombatStats> _stats = new Dictionary<string, TroopCombatStats>();
	}
}
