using System;
using HarmonyLib;
using Retinues.Utils;
using SandBox.GauntletUI;

namespace Retinues.Game.Helpers
{
	// Token: 0x020000A3 RID: 163
	[HarmonyPatch(typeof(GauntletBarberScreen))]
	internal static class GauntletBarberScreenPatches
	{
		// Token: 0x0600070F RID: 1807 RVA: 0x00023A07 File Offset: 0x00021C07
		[HarmonyPostfix]
		[HarmonyPatch("OnFinalize")]
		private static void OnFinalizePostfix()
		{
			Log.Debug("[HeroAppearanceHelper] GauntletBarberScreen.OnFinalize -> closing barber session (if any).");
			HeroAppearanceHelper.OnBarberClosed();
		}

		// Token: 0x06000710 RID: 1808 RVA: 0x00023A18 File Offset: 0x00021C18
		[HarmonyPostfix]
		[HarmonyPatch("OnFrameTick")]
		private static void OnFrameTickPostfix(GauntletBarberScreen __instance, float dt)
		{
			try
			{
				if (HeroAppearanceHelper.HasActiveSession)
				{
					HeroAppearanceHelper.PrimeFaceGen(__instance.Handler);
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}
	}
}
