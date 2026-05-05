using System;
using System.Reflection;
using System.Text.RegularExpressions;
using HarmonyLib;
using Retinues.Utils;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu;

namespace Retinues.Game.Menu.Patches
{
	// Token: 0x0200009F RID: 159
	[HarmonyPatch(typeof(GameMenuItemProgressVM), "Refresh")]
	internal static class GameMenuItemProgressVM_Refresh_Patch
	{
		// Token: 0x060006FA RID: 1786 RVA: 0x00022BF8 File Offset: 0x00020DF8
		[SafeMethod(null, true, null)]
		private static void Postfix(GameMenuItemProgressVM __instance)
		{
			PropertyInfo property = __instance.GetType().GetProperty("ProgressText");
			string text = ((property != null) ? property.GetValue(__instance) : null) as string;
			if (text != null && !string.IsNullOrEmpty(text))
			{
				string text2 = GameMenuItemProgressVM_Refresh_Patch.TrailingPointZero.Replace(text, "$1$3");
				if (text2 != text)
				{
					property.SetValue(__instance, text2);
				}
			}
		}

		// Token: 0x040001AA RID: 426
		private static readonly Regex TrailingPointZero = new Regex("(\\d+)([.,])0(\\D|$)", RegexOptions.Compiled);
	}
}
