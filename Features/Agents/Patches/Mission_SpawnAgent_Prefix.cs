using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Retinues.Configuration;
using Retinues.Game.Helpers;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Retinues.Features.Agents.Patches
{
	// Token: 0x020000CD RID: 205
	[HarmonyPatch(typeof(Mission), "SpawnAgent")]
	internal static class Mission_SpawnAgent_Prefix
	{
		// Token: 0x06000861 RID: 2145 RVA: 0x0002A4B0 File Offset: 0x000286B0
		private static void Prefix(AgentBuildData agentBuildData, bool spawnFromAgentVisuals)
		{
			try
			{
				WCharacter wcharacter = AgentHelper.TroopFromAgentBuildData(agentBuildData, false);
				if (!(wcharacter == null))
				{
					if (!wcharacter.IsHero)
					{
						if (wcharacter.IsCustom)
						{
							if (MissionHelper.IsCombatMission(null))
							{
								WEquipment wequipment;
								if (agentBuildData.AgentCivilianEquipment)
								{
									List<WEquipment> list = wcharacter.Loadout.CivilianSets.ToList<WEquipment>();
									wequipment = ((list.Count == 0) ? wcharacter.Loadout.Civilian : list[MBRandom.RandomInt(list.Count)]);
								}
								else if (Config.ForceMainBattleSetInCombat)
								{
									wequipment = wcharacter.Loadout.Battle;
								}
								else
								{
									PolicyToggleType battleType = MissionHelper.GetBattleType(null);
									List<WEquipment> equipments = wcharacter.Loadout.Equipments;
									List<WEquipment> list2 = new List<WEquipment>();
									for (int i = 0; i < equipments.Count; i++)
									{
										WEquipment wequipment2 = equipments[i];
										if (!wequipment2.IsCivilian && CombatAgentBehavior.IsEnabled(wcharacter, i, battleType))
										{
											list2.Add(wequipment2);
										}
									}
									if (list2.Count == 0)
									{
										list2.Add(wcharacter.Loadout.Battle);
									}
									wequipment = list2[MBRandom.RandomInt(list2.Count)];
								}
								if (wequipment != null)
								{
									Equipment @base = wequipment.Base;
									if (CombatAgentBehavior.IsEnabled(wcharacter, wequipment.Index, PolicyToggleType.GenderOverride))
									{
										agentBuildData.IsFemale(!wcharacter.IsFemale);
									}
									agentBuildData.Equipment(@base).MissionEquipment(null).FixedEquipment(true).CivilianEquipment(false).NoWeapons(false).NoArmor(false);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}
	}
}
