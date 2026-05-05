using System;
using HarmonyLib;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Settlements;

namespace Retinues.Features.Volunteers.Patches
{
	// Token: 0x020000AF RID: 175
	[HarmonyPatch(typeof(RecruitmentCampaignBehavior), "UpdateVolunteersOfNotablesInSettlement")]
	public static class VolunteerSwapOnUpdate
	{
		// Token: 0x06000753 RID: 1875 RVA: 0x000255E8 File Offset: 0x000237E8
		[SafeMethod(null, true, null)]
		private static void Postfix(Settlement settlement)
		{
			if (settlement == null)
			{
				return;
			}
			WSettlement wsettlement = new WSettlement(settlement);
			if (wsettlement.PlayerFaction == null)
			{
				return;
			}
			wsettlement.SwapVolunteers(null);
		}
	}
}
