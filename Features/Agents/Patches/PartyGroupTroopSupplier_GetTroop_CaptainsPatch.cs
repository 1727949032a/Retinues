using System;
using System.Collections.Generic;
using HarmonyLib;
using Retinues.Game.Helpers;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.TroopSuppliers;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace Retinues.Features.Agents.Patches
{
	// Token: 0x020000CE RID: 206
	[HarmonyPatch(typeof(PartyGroupTroopSupplier), "GetTroop")]
	internal static class PartyGroupTroopSupplier_GetTroop_CaptainsPatch
	{
		// Token: 0x06000862 RID: 2146 RVA: 0x0002A664 File Offset: 0x00028864
		private static void Postfix(UniqueTroopDescriptor troopDescriptor, ref CharacterObject __result)
		{
			try
			{
				Mission mission = Mission.Current;
				if (MissionHelper.IsCombatMission(mission))
				{
					if (__result != null)
					{
						string text = __result.StringId;
						if (WCharacter.IsCustomId(text) || WCharacter.IsCaptainId(text))
						{
							string text2;
							if (WCharacter.TryGetBaseIdFromCaptainId(text, out text2))
							{
								text = text2;
							}
							if (WCharacter.IsCaptainEnabledId(text))
							{
								if (mission != PartyGroupTroopSupplier_GetTroop_CaptainsPatch._lastMission)
								{
									PartyGroupTroopSupplier_GetTroop_CaptainsPatch._lastMission = mission;
									PartyGroupTroopSupplier_GetTroop_CaptainsPatch._spawnCounts.Clear();
								}
								int num;
								PartyGroupTroopSupplier_GetTroop_CaptainsPatch._spawnCounts.TryGetValue(text, out num);
								num++;
								PartyGroupTroopSupplier_GetTroop_CaptainsPatch._spawnCounts[text] = num;
								if (num % 15 == 0)
								{
									CharacterObject characterObject;
									if (WCharacter.TryGetCaptainObject(text, out characterObject))
									{
										__result = characterObject;
									}
									else
									{
										CharacterObject @object = MBObjectManager.Instance.GetObject<CharacterObject>(text);
										if (@object != null)
										{
											WCharacter wcharacter = new WCharacter(@object);
											if (wcharacter.CanHaveCaptain)
											{
												WCharacter captain = wcharacter.Captain;
												if (((captain != null) ? captain.Base : null) != null)
												{
													__result = captain.Base;
												}
											}
										}
									}
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

		// Token: 0x04000237 RID: 567
		private const int CaptainFrequency = 15;

		// Token: 0x04000238 RID: 568
		private static Mission _lastMission;

		// Token: 0x04000239 RID: 569
		private static readonly Dictionary<string, int> _spawnCounts = new Dictionary<string, int>();
	}
}
