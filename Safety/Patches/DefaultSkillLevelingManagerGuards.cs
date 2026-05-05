using System;
using System.Collections.Generic;
using HarmonyLib;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Localization;

namespace Retinues.Safety.Patches
{
	// Token: 0x02000049 RID: 73
	[HarmonyPatch(typeof(DefaultSkillLevelingManager), "OnTroopRecruited")]
	internal static class DefaultSkillLevelingManagerGuards
	{
		// Token: 0x06000167 RID: 359 RVA: 0x0000A490 File Offset: 0x00008690
		[HarmonyPrefix]
		private static bool Prefix(Hero hero, int amount, int tier)
		{
			bool result;
			try
			{
				if (hero == null)
				{
					result = false;
				}
				else if (hero.HeroDeveloper == null)
				{
					string text;
					if ((text = hero.StringId) == null)
					{
						TextObject name = hero.Name;
						text = (((name != null) ? name.ToString() : null) ?? "<unknown-hero>");
					}
					string text2 = text;
					if (DefaultSkillLevelingManagerGuards._reported.Add(text2))
					{
						Log.Error("DefaultSkillLevelingManagerGuards: HeroDeveloper is null for hero '" + text2 + "'. " + string.Format("Skipping OnTroopRecruited XP (amount={0}, tier={1}).", amount, tier));
					}
					result = false;
				}
				else
				{
					result = true;
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "DefaultSkillLevelingManagerGuards: Exception in Prefix.", null);
				result = true;
			}
			return result;
		}

		// Token: 0x04000089 RID: 137
		private static readonly HashSet<string> _reported = new HashSet<string>(StringComparer.Ordinal);
	}
}
