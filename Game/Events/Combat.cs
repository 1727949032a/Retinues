using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Retinues.Configuration;
using Retinues.Features.Statistics;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Retinues.Game.Events
{
	// Token: 0x020000A9 RID: 169
	[SafeClass]
	public class Combat : MissionBehavior
	{
		// Token: 0x1700034D RID: 845
		// (get) Token: 0x06000737 RID: 1847 RVA: 0x00024C6A File Offset: 0x00022E6A
		public override MissionBehaviorType BehaviorType
		{
			get
			{
				return MissionBehaviorType.Other;
			}
		}

		// Token: 0x1700034E RID: 846
		// (get) Token: 0x06000738 RID: 1848 RVA: 0x00024C70 File Offset: 0x00022E70
		public bool IsVictory
		{
			get
			{
				Mission mission = Mission.Current;
				if (mission == null)
				{
					return false;
				}
				MissionResult missionResult = mission.MissionResult;
				return ((missionResult != null) ? new bool?(missionResult.PlayerVictory) : null).GetValueOrDefault();
			}
		}

		// Token: 0x1700034F RID: 847
		// (get) Token: 0x06000739 RID: 1849 RVA: 0x00024CAE File Offset: 0x00022EAE
		public bool IsDefeat
		{
			get
			{
				return !this.IsVictory;
			}
		}

		// Token: 0x0600073A RID: 1850 RVA: 0x00024CB9 File Offset: 0x00022EB9
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this.EnsureTracker();
		}

		// Token: 0x17000350 RID: 848
		// (get) Token: 0x0600073B RID: 1851 RVA: 0x00024CC8 File Offset: 0x00022EC8
		public List<Combat.Kill> Kills
		{
			get
			{
				Combat.KillTracker tracker = this._tracker;
				return (((tracker != null) ? tracker.Kills : null) ?? new List<Combat.Kill>()).FindAll((Combat.Kill kill) => Combat.<get_Kills>g__IsValidId|10_0(kill.KillerCharacterId) && Combat.<get_Kills>g__IsValidId|10_0(kill.VictimCharacterId));
			}
		}

		// Token: 0x0600073C RID: 1852 RVA: 0x00024D14 File Offset: 0x00022F14
		private void EnsureTracker()
		{
			Mission mission = base.Mission;
			if (mission == null)
			{
				return;
			}
			this._tracker = mission.GetMissionBehavior<Combat.KillTracker>();
			if (this._tracker == null)
			{
				this._tracker = new Combat.KillTracker();
				mission.AddMissionBehavior(this._tracker);
			}
		}

		// Token: 0x0600073E RID: 1854 RVA: 0x00024D60 File Offset: 0x00022F60
		[CompilerGenerated]
		internal static bool <get_Kills>g__IsValidId|10_0(string stringId)
		{
			try
			{
				new WCharacter(stringId);
			}
			catch (Exception ex)
			{
				Log.Error("Kill.IsValid: invalid ID: " + stringId);
				Log.Exception(ex, "", null);
				return false;
			}
			return true;
		}

		// Token: 0x040001D7 RID: 471
		private Combat.KillTracker _tracker;

		// Token: 0x02000181 RID: 385
		public readonly struct Kill
		{
			// Token: 0x17000472 RID: 1138
			// (get) Token: 0x06000C19 RID: 3097 RVA: 0x0003490B File Offset: 0x00032B0B
			public WCharacter Killer
			{
				get
				{
					return new WCharacter(this.KillerCharacterId);
				}
			}

			// Token: 0x17000473 RID: 1139
			// (get) Token: 0x06000C1A RID: 3098 RVA: 0x00034918 File Offset: 0x00032B18
			public WCharacter Victim
			{
				get
				{
					return new WCharacter(this.VictimCharacterId);
				}
			}

			// Token: 0x06000C1B RID: 3099 RVA: 0x00034928 File Offset: 0x00032B28
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Kill(Agent victim, Agent killer, AgentState state, KillingBlow blow)
			{
				Mission mission = Mission.Current;
				Team team;
				if ((team = ((mission != null) ? mission.PlayerTeam : null)) == null)
				{
					if (mission == null)
					{
						team = null;
					}
					else
					{
						Agent mainAgent = mission.MainAgent;
						team = ((mainAgent != null) ? mainAgent.Team : null);
					}
				}
				Team team2 = team;
				this.KillerIsPlayer = (killer != null && killer.IsPlayerControlled);
				bool killerIsPlayerTroop;
				if (killer != null && killer.IsAIControlled)
				{
					if (killer == null)
					{
						killerIsPlayerTroop = false;
					}
					else
					{
						Team team3 = killer.Team;
						killerIsPlayerTroop = ((team3 != null) ? new bool?(team3.IsPlayerTeam) : null).GetValueOrDefault();
					}
				}
				else
				{
					killerIsPlayerTroop = false;
				}
				this.KillerIsPlayerTroop = killerIsPlayerTroop;
				bool killerIsAllyTroop;
				if (killer != null)
				{
					Team team4 = killer.Team;
					if (((team4 != null) ? new bool?(team4.IsPlayerAlly) : null).GetValueOrDefault() && !this.KillerIsPlayer)
					{
						killerIsAllyTroop = !this.KillerIsPlayerTroop;
						goto IL_C3;
					}
				}
				killerIsAllyTroop = false;
				IL_C3:
				this.KillerIsAllyTroop = killerIsAllyTroop;
				bool killerIsEnemyTroop;
				if (team2 != null)
				{
					if (killer == null)
					{
						killerIsEnemyTroop = false;
					}
					else
					{
						Team team5 = killer.Team;
						killerIsEnemyTroop = ((team5 != null) ? new bool?(team5.IsEnemyOf(team2)) : null).GetValueOrDefault();
					}
				}
				else
				{
					killerIsEnemyTroop = false;
				}
				this.KillerIsEnemyTroop = killerIsEnemyTroop;
				this.VictimIsPlayer = (victim != null && victim.IsPlayerControlled);
				bool victimIsPlayerTroop;
				if (victim != null && victim.IsAIControlled)
				{
					if (victim == null)
					{
						victimIsPlayerTroop = false;
					}
					else
					{
						Team team6 = victim.Team;
						victimIsPlayerTroop = ((team6 != null) ? new bool?(team6.IsPlayerTeam) : null).GetValueOrDefault();
					}
				}
				else
				{
					victimIsPlayerTroop = false;
				}
				this.VictimIsPlayerTroop = victimIsPlayerTroop;
				bool victimIsAllyTroop;
				if (victim != null)
				{
					Team team7 = victim.Team;
					if (((team7 != null) ? new bool?(team7.IsPlayerAlly) : null).GetValueOrDefault() && !this.VictimIsPlayer)
					{
						victimIsAllyTroop = !this.VictimIsPlayerTroop;
						goto IL_196;
					}
				}
				victimIsAllyTroop = false;
				IL_196:
				this.VictimIsAllyTroop = victimIsAllyTroop;
				bool victimIsEnemyTroop;
				if (team2 != null)
				{
					if (victim == null)
					{
						victimIsEnemyTroop = false;
					}
					else
					{
						Team team8 = victim.Team;
						victimIsEnemyTroop = ((team8 != null) ? new bool?(team8.IsEnemyOf(team2)) : null).GetValueOrDefault();
					}
				}
				else
				{
					victimIsEnemyTroop = false;
				}
				this.VictimIsEnemyTroop = victimIsEnemyTroop;
				string killerCharacterId;
				if (killer == null)
				{
					killerCharacterId = null;
				}
				else
				{
					BasicCharacterObject character = killer.Character;
					killerCharacterId = ((character != null) ? character.StringId : null);
				}
				this.KillerCharacterId = killerCharacterId;
				string victimCharacterId;
				if (victim == null)
				{
					victimCharacterId = null;
				}
				else
				{
					BasicCharacterObject character2 = victim.Character;
					victimCharacterId = ((character2 != null) ? character2.StringId : null);
				}
				this.VictimCharacterId = victimCharacterId;
				string lootCode;
				if (victim == null)
				{
					lootCode = null;
				}
				else
				{
					Equipment spawnEquipment = victim.SpawnEquipment;
					lootCode = ((spawnEquipment != null) ? spawnEquipment.CalculateEquipmentCode() : null);
				}
				this.LootCode = lootCode;
				this.State = state;
				this.IsMissile = blow.IsMissile;
				this.BlowWeaponClass = blow.WeaponClass;
			}

			// Token: 0x06000C1C RID: 3100 RVA: 0x00034B85 File Offset: 0x00032D85
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool IsValid(Agent victim, Agent killer, AgentState state)
			{
				return killer != null && victim != null && killer.IsHuman && victim.IsHuman && (state == AgentState.Killed || state == AgentState.Unconscious) && killer.Character is CharacterObject && victim.Character is CharacterObject;
			}

			// Token: 0x04000494 RID: 1172
			public readonly bool KillerIsPlayer;

			// Token: 0x04000495 RID: 1173
			public readonly bool KillerIsPlayerTroop;

			// Token: 0x04000496 RID: 1174
			public readonly bool KillerIsAllyTroop;

			// Token: 0x04000497 RID: 1175
			public readonly bool KillerIsEnemyTroop;

			// Token: 0x04000498 RID: 1176
			public readonly bool VictimIsPlayer;

			// Token: 0x04000499 RID: 1177
			public readonly bool VictimIsPlayerTroop;

			// Token: 0x0400049A RID: 1178
			public readonly bool VictimIsAllyTroop;

			// Token: 0x0400049B RID: 1179
			public readonly bool VictimIsEnemyTroop;

			// Token: 0x0400049C RID: 1180
			public readonly string KillerCharacterId;

			// Token: 0x0400049D RID: 1181
			public readonly string VictimCharacterId;

			// Token: 0x0400049E RID: 1182
			public readonly string LootCode;

			// Token: 0x0400049F RID: 1183
			public readonly AgentState State;

			// Token: 0x040004A0 RID: 1184
			public readonly bool IsMissile;

			// Token: 0x040004A1 RID: 1185
			public readonly int BlowWeaponClass;
		}

		// Token: 0x02000182 RID: 386
		[SafeClass]
		internal sealed class KillTracker : MissionBehavior
		{
			// Token: 0x17000474 RID: 1140
			// (get) Token: 0x06000C1D RID: 3101 RVA: 0x00034BC2 File Offset: 0x00032DC2
			public override MissionBehaviorType BehaviorType
			{
				get
				{
					return MissionBehaviorType.Other;
				}
			}

			// Token: 0x06000C1E RID: 3102 RVA: 0x00034BC8 File Offset: 0x00032DC8
			internal void Capture(Agent victim, Agent killer, AgentState state, KillingBlow blow)
			{
				if (!Combat.Kill.IsValid(victim, killer, state))
				{
					return;
				}
				Combat.Kill item = new Combat.Kill(victim, killer, state, blow);
				this.Kills.Add(item);
			}

			// Token: 0x06000C1F RID: 3103 RVA: 0x00034BF8 File Offset: 0x00032DF8
			public override void OnAgentRemoved(Agent victim, Agent killer, AgentState state, KillingBlow blow)
			{
				try
				{
					if (Combat.Kill.IsValid(victim, killer, state))
					{
						this.Kills.Add(new Combat.Kill(victim, killer, state, blow));
					}
				}
				catch (Exception ex)
				{
					Log.Exception(ex, "", null);
				}
			}

			// Token: 0x06000C20 RID: 3104 RVA: 0x00034C44 File Offset: 0x00032E44
			public sealed override void OnEndMissionInternal()
			{
				base.OnEndMissionInternal();
				try
				{
					MobileParty mainParty = MobileParty.MainParty;
					MapEvent mapEvent = (mainParty != null) ? mainParty.MapEvent : null;
					if (mapEvent != null && this.Kills.Count > 0)
					{
						TroopStatisticsBehavior.RecordFromMission(new Battle(mapEvent), this.Kills);
					}
					if (Config.DebugMode)
					{
						this.LogReport();
					}
				}
				catch (Exception ex)
				{
					Log.Exception(ex, "", null);
				}
				finally
				{
					if (this.Kills.Count > 0)
					{
						this.Kills.Clear();
					}
				}
			}

			// Token: 0x06000C21 RID: 3105 RVA: 0x00034CE4 File Offset: 0x00032EE4
			public void LogReport()
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				int num6 = 0;
				int num7 = 0;
				int num8 = 0;
				foreach (Combat.Kill kill in this.Kills)
				{
					if (kill.KillerIsPlayer)
					{
						num++;
					}
					if (kill.KillerIsPlayerTroop)
					{
						num2++;
					}
					if (kill.KillerIsAllyTroop)
					{
						num3++;
					}
					if (kill.KillerIsEnemyTroop)
					{
						num4++;
					}
					if (kill.VictimIsPlayer)
					{
						num5++;
					}
					if (kill.VictimIsPlayerTroop)
					{
						num6++;
					}
					if (kill.VictimIsAllyTroop)
					{
						num7++;
					}
					if (kill.VictimIsEnemyTroop)
					{
						num8++;
					}
				}
				Log.Debug("--- Combat Report ---");
				Log.Debug(string.Format("Kills: {0} total", this.Kills.Count));
				Log.Debug(string.Format("PlayerKills = {0}", num));
				Log.Debug(string.Format("PlayerTroopKills = {0}", num2));
				Log.Debug(string.Format("AllyKills = {0}", num3));
				Log.Debug(string.Format("EnemyKills = {0}", num4));
				Log.Debug("---------------------");
				Log.Debug(string.Format("Casualties: {0} total", this.Kills.Count));
				Log.Debug(string.Format("PlayerCasualties = {0}", num5));
				Log.Debug(string.Format("PlayerTroopCasualties = {0}", num6));
				Log.Debug(string.Format("AllyCasualties = {0}", num7));
				Log.Debug(string.Format("EnemyCasualties = {0}", num8));
				Log.Debug("---------------------");
			}

			// Token: 0x040004A2 RID: 1186
			internal readonly List<Combat.Kill> Kills = new List<Combat.Kill>();
		}
	}
}
