using System;
using System.Reflection;
using HarmonyLib;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Retinues.Mods.Shokuho
{
	// Token: 0x02000058 RID: 88
	internal static class ShokuhoEquipmentPatcher
	{
		// Token: 0x060001A7 RID: 423 RVA: 0x0000BACC File Offset: 0x00009CCC
		public static void TryPatch(Harmony harmony)
		{
			try
			{
				Type type = AccessTools.TypeByName("Shokuho.ShokuhoMissions.MissionLogics.ShokuhoAgentEquipmentMissionLogic");
				if (type == null)
				{
					Log.Debug("Shokuho not found; skipping ShokuhoAgentEquipmentMissionLogic patch.");
				}
				else
				{
					MethodInfo methodInfo = AccessTools.Method(type, "OnAgentBuild", new Type[]
					{
						typeof(Agent),
						typeof(Banner)
					}, null);
					if (methodInfo == null)
					{
						Log.Debug("ShokuhoAgentEquipmentMissionLogic.OnAgentBuild not found; skipping patch.");
					}
					else
					{
						HarmonyMethod prefix = new HarmonyMethod(typeof(ShokuhoEquipmentPatcher).GetMethod("Prefix", BindingFlags.Static | BindingFlags.NonPublic));
						harmony.Patch(methodInfo, prefix, null, null, null);
						Log.Info("Patched ShokuhoAgentEquipmentMissionLogic.OnAgentBuild.");
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x0000BB90 File Offset: 0x00009D90
		private static bool Prefix(Agent agent, Banner banner)
		{
			bool result;
			try
			{
				if (agent == null || agent.Character == null)
				{
					result = true;
				}
				else
				{
					CharacterObject characterObject = agent.Character as CharacterObject;
					if (characterObject == null)
					{
						result = true;
					}
					else
					{
						WCharacter wcharacter = new WCharacter(characterObject);
						if (wcharacter.IsCustom || wcharacter.NeedsPersistence)
						{
							result = false;
						}
						else
						{
							result = true;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
				result = true;
			}
			return result;
		}

		// Token: 0x040000A6 RID: 166
		private const string ShokuhoTypeName = "Shokuho.ShokuhoMissions.MissionLogics.ShokuhoAgentEquipmentMissionLogic";
	}
}
