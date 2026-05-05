using System;
using System.Collections.Generic;
using HarmonyLib;
using SandBox.Missions.AgentBehaviors;
using SandBox.Missions.MissionLogics;
using TaleWorlds.MountAndBlade;

namespace Retinues.Safety.Patches
{
	// Token: 0x02000045 RID: 69
	[HarmonyPatch(typeof(DisguiseMissionLogic), "CanAgentSeeAgent")]
	internal static class DisguiseMissionLogicCanAgentSeeAgentPatch
	{
		// Token: 0x06000162 RID: 354 RVA: 0x0000A23C File Offset: 0x0000843C
		[HarmonyPrefix]
		private static bool Prefix(Agent agent1, Agent agent2, ref bool hasVisualOnCorpse, ref bool __result, Dictionary<Agent, AlarmedBehaviorGroup> ____agentAlarmedBehaviorCache)
		{
			if (agent1 == null)
			{
				hasVisualOnCorpse = false;
				__result = false;
				return false;
			}
			AlarmedBehaviorGroup alarmedBehaviorGroup;
			if (!____agentAlarmedBehaviorCache.TryGetValue(agent1, out alarmedBehaviorGroup) || alarmedBehaviorGroup == null)
			{
				hasVisualOnCorpse = false;
				__result = false;
				return false;
			}
			return true;
		}
	}
}
