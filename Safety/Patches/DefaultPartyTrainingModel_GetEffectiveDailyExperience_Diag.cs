using System;
using HarmonyLib;
using Retinues.Safety.Sanitizer;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;

namespace Retinues.Safety.Patches
{
	// Token: 0x02000046 RID: 70
	[HarmonyPatch(typeof(DefaultPartyTrainingModel), "GetEffectiveDailyExperience")]
	public static class DefaultPartyTrainingModel_GetEffectiveDailyExperience_Diag
	{
		// Token: 0x06000163 RID: 355 RVA: 0x0000A26C File Offset: 0x0000846C
		private static Exception Finalizer(MobileParty mobileParty, TroopRosterElement troop, Exception __exception)
		{
			if (__exception == null)
			{
				return null;
			}
			try
			{
				string str = "GetEffectiveDailyExperience crashed for party ";
				string str2 = string.Format("{0} ({1}), ", (mobileParty != null) ? mobileParty.Name : null, (mobileParty != null) ? mobileParty.StringId : null);
				string format = "troop={0}: {1}";
				CharacterObject character = troop.Character;
				Log.Error(str + str2 + string.Format(format, (character != null) ? character.StringId : null, __exception));
				if (mobileParty != null)
				{
					PartySanitizer.SanitizeParty(mobileParty, false);
				}
			}
			catch (Exception arg)
			{
				Log.Error(string.Format("Training dump failed: {0}", arg));
			}
			return null;
		}
	}
}
