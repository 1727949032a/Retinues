using System;
using Retinues.Features.Agents;
using Retinues.Game.Events;
using Retinues.Mods;
using Retinues.Utils;
using SandBox.Tournaments.MissionLogics;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Retinues.Game.Helpers
{
	// Token: 0x020000A5 RID: 165
	internal static class MissionHelper
	{
		// Token: 0x06000719 RID: 1817 RVA: 0x00023EE4 File Offset: 0x000220E4
		public static bool IsCombatMission(Mission mission = null)
		{
			if (mission == null)
			{
				mission = Mission.Current;
			}
			if (mission == null)
			{
				MissionHelper._cachedMission = null;
				MissionHelper._cachedIsCombatMission = false;
				return false;
			}
			if (mission != MissionHelper._cachedMission)
			{
				MissionHelper._cachedMission = mission;
				MissionHelper._cachedIsCombatMission = MissionHelper.ComputeIsCombatMission(mission);
				Log.Info(string.Format("Computed IsCombatMission: {0}", MissionHelper._cachedIsCombatMission));
			}
			return MissionHelper._cachedIsCombatMission;
		}

		// Token: 0x0600071A RID: 1818 RVA: 0x00023F44 File Offset: 0x00022144
		public static PolicyToggleType GetBattleType(Mission mission = null)
		{
			if (mission == null)
			{
				mission = Mission.Current;
			}
			if (mission == null)
			{
				MissionHelper._cachedBattleMission = null;
				MissionHelper._cachedBattle = null;
				MissionHelper._cachedBattleType = PolicyToggleType.FieldBattle;
			}
			else
			{
				if (!MissionHelper.IsCombatMission(mission))
				{
					MissionHelper._cachedBattleMission = mission;
					MissionHelper._cachedBattle = null;
					MissionHelper._cachedBattleType = PolicyToggleType.FieldBattle;
					return MissionHelper._cachedBattleType;
				}
				if (mission != MissionHelper._cachedBattleMission || MissionHelper._cachedBattle == null)
				{
					MissionHelper._cachedBattleMission = mission;
					MissionHelper._cachedBattle = new Battle(null);
					MissionHelper._cachedBattleType = MissionHelper.ComputeBattleType(MissionHelper._cachedBattle);
					Log.Info(string.Format("Computed BattleType: {0}", MissionHelper._cachedBattleType));
				}
			}
			return MissionHelper._cachedBattleType;
		}

		// Token: 0x0600071B RID: 1819 RVA: 0x00023FE0 File Offset: 0x000221E0
		private static bool ComputeIsCombatMission(Mission mission)
		{
			MissionMode mode = mission.Mode;
			Log.Info(string.Format("Computing mission cache. Mode: {0}", mode));
			if (mode != MissionMode.Battle && mode != MissionMode.Duel && mode != MissionMode.Deployment && mode != MissionMode.Stealth && mode != MissionMode.StartUp)
			{
				return false;
			}
			if (mode == MissionMode.StartUp)
			{
				Log.Info("StartUp mode detected.");
				MobileParty mainParty = MobileParty.MainParty;
				if (((mainParty != null) ? mainParty.MapEvent : null) == null)
				{
					Log.Info("No map event - not a combat mission.");
					return false;
				}
				if (!ModCompatibility.HasNavalDLC)
				{
					Log.Info("No Naval DLC, can't be naval combat mission, skipping.");
					return false;
				}
				Log.Info("Naval DLC present, assuming naval combat mission.");
			}
			foreach (MissionBehavior missionBehavior in mission.MissionBehaviors)
			{
				if (missionBehavior is TournamentBehavior)
				{
					return false;
				}
				string fullName = missionBehavior.GetType().FullName;
				string text = ((fullName != null) ? fullName.ToLowerInvariant() : null) ?? string.Empty;
				if (text.Contains("tournament") || text.Contains("arena"))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600071C RID: 1820 RVA: 0x000240F8 File Offset: 0x000222F8
		private static PolicyToggleType ComputeBattleType(Battle battle)
		{
			Log.Info("Computing battle type.");
			PolicyToggleType result = PolicyToggleType.FieldBattle;
			if (battle != null && battle.IsSiege)
			{
				result = (battle.PlayerIsDefender ? PolicyToggleType.SiegeDefense : PolicyToggleType.SiegeAssault);
			}
			else if (battle != null && battle.IsNavalBattle)
			{
				result = PolicyToggleType.NavalBattle;
			}
			return result;
		}

		// Token: 0x040001C1 RID: 449
		private static Mission _cachedMission;

		// Token: 0x040001C2 RID: 450
		private static bool _cachedIsCombatMission;

		// Token: 0x040001C3 RID: 451
		private static Battle _cachedBattle;

		// Token: 0x040001C4 RID: 452
		private static Mission _cachedBattleMission;

		// Token: 0x040001C5 RID: 453
		private static PolicyToggleType _cachedBattleType;
	}
}
