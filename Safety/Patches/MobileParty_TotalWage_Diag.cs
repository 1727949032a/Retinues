using System;
using HarmonyLib;
using Retinues.Safety.Sanitizer;
using Retinues.Utils;
using TaleWorlds.CampaignSystem.Party;

namespace Retinues.Safety.Patches
{
	// Token: 0x0200004D RID: 77
	[HarmonyPatch(typeof(MobileParty), "get_TotalWage")]
	public static class MobileParty_TotalWage_Diag
	{
		// Token: 0x0600016D RID: 365 RVA: 0x0000A938 File Offset: 0x00008B38
		private static Exception Finalizer(MobileParty __instance, Exception __exception)
		{
			if (__exception == null)
			{
				return null;
			}
			try
			{
				PartySanitizer.SanitizeParty(__instance, false);
			}
			catch (Exception arg)
			{
				Log.Error(string.Format("Dump failed: {0}", arg));
			}
			return null;
		}
	}
}
