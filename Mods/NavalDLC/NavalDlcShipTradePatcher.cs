using System;
using System.Reflection;
using HarmonyLib;
using Retinues.Utils;
using TaleWorlds.CampaignSystem.Party;

namespace Retinues.Mods.NavalDLC
{
	// Token: 0x02000059 RID: 89
	public static class NavalDlcShipTradePatcher
	{
		// Token: 0x060001A9 RID: 425 RVA: 0x0000BC00 File Offset: 0x00009E00
		public static void TryPatch(Harmony harmony)
		{
			try
			{
				Type type = AccessTools.TypeByName("NavalDLC.CampaignBehaviors.ShipTradeCampaignBehavior");
				MethodInfo methodInfo = AccessTools.Method(type, "OnShipOwnerChanged", new Type[]
				{
					AccessTools.TypeByName("TaleWorlds.CampaignSystem.Naval.Ship"),
					typeof(PartyBase),
					AccessTools.TypeByName("TaleWorlds.CampaignSystem.Actions.ChangeShipOwnerAction+ShipOwnerChangeDetail")
				}, null);
				if (type == null || methodInfo == null)
				{
					Log.Warn("[NavalDLCCompat] ShipTradeCampaignBehavior.OnShipOwnerChanged not found; no patch applied.");
				}
				else
				{
					MethodInfo method = typeof(NavalDlcShipTradePatcher).GetMethod("OnShipOwnerChanged_Finalizer", BindingFlags.Static | BindingFlags.NonPublic);
					harmony.Patch(methodInfo, null, null, null, new HarmonyMethod(method));
					Log.Info("[NavalDLCCompat] Patched ShipTradeCampaignBehavior.OnShipOwnerChanged (finalizer).");
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "[NavalDLCCompat] Failed to patch ShipTradeCampaignBehavior.OnShipOwnerChanged.", null);
			}
		}

		// Token: 0x060001AA RID: 426 RVA: 0x0000BCC4 File Offset: 0x00009EC4
		private static Exception OnShipOwnerChanged_Finalizer(Exception __exception)
		{
			if (__exception == null)
			{
				return null;
			}
			Log.Error("[NavalDLCCompat] Suppressed crash in OnShipOwnerChanged: " + __exception.GetType().Name + ": " + __exception.Message);
			return null;
		}
	}
}
