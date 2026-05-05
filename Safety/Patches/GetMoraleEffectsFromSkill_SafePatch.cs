using System;
using System.Reflection;
using HarmonyLib;
using Helpers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace Retinues.Safety.Patches
{
	// Token: 0x02000047 RID: 71
	[HarmonyPatch]
	internal static class GetMoraleEffectsFromSkill_SafePatch
	{
		// Token: 0x06000164 RID: 356 RVA: 0x0000A300 File Offset: 0x00008500
		[HarmonyTargetMethod]
		private static MethodBase TargetMethod()
		{
			return AccessTools.Method(typeof(DefaultPartyMoraleModel), "GetMoraleEffectsFromSkill", new Type[]
			{
				typeof(MobileParty),
				typeof(ExplainedNumber).MakeByRefType()
			}, null);
		}

		// Token: 0x06000165 RID: 357 RVA: 0x0000A33C File Offset: 0x0000853C
		[HarmonyPrefix]
		private static bool Prefix(MobileParty party, ref ExplainedNumber bonus)
		{
			bool result;
			try
			{
				if (party == null || party.Party == null)
				{
					Log.Error("GetMoraleEffectsFromSkill: party/party.Party is null.");
					result = false;
				}
				else
				{
					CharacterObject characterObject = null;
					try
					{
						characterObject = SkillHelper.GetEffectivePartyLeaderForSkill(party.Party);
					}
					catch (Exception ex)
					{
						Log.Exception(ex, "", null);
						return false;
					}
					if (characterObject == null)
					{
						result = false;
					}
					else
					{
						int num = 0;
						try
						{
							num = characterObject.GetSkillValue(DefaultSkills.Leadership);
						}
						catch (Exception ex2)
						{
							Log.Exception(ex2, "", null);
							return false;
						}
						if (num > 0)
						{
							try
							{
								SkillHelper.AddSkillBonusForCharacter(DefaultSkillEffects.LeadershipMoraleBonus, characterObject, ref bonus);
							}
							catch (Exception ex3)
							{
								Log.Exception(ex3, "", null);
							}
						}
						result = false;
					}
				}
			}
			catch (Exception ex4)
			{
				Log.Exception(ex4, "", null);
				result = false;
			}
			return result;
		}
	}
}
