using System;
using HarmonyLib;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace Retinues.Safety.Patches
{
	// Token: 0x0200004C RID: 76
	[HarmonyPatch(typeof(RecruitmentCampaignBehavior), "UpdateVolunteersOfNotablesInSettlement")]
	public static class VolunteerUpdateSafetyPatch
	{
		// Token: 0x0600016B RID: 363 RVA: 0x0000A64C File Offset: 0x0000884C
		[HarmonyPriority(800)]
		private static bool Prefix(Settlement settlement)
		{
			bool result;
			try
			{
				if (settlement == null)
				{
					result = false;
				}
				else if (settlement.IsTown && settlement.Town == null)
				{
					Log.Error("VolunteerUpdateSafety: settlement '" + settlement.StringId + "' is Town but Town is null. Skipping update.");
					result = false;
				}
				else
				{
					if (settlement.IsVillage)
					{
						Village village = settlement.Village;
						if (village == null || village.Bound == null || village.Bound.Town == null)
						{
							Log.Error("VolunteerUpdateSafety: settlement '" + settlement.StringId + "' is Village but Bound/Town is null. Skipping update.");
							return false;
						}
					}
					MBReadOnlyList<Hero> notables = settlement.Notables;
					if (notables == null)
					{
						Log.Error("VolunteerUpdateSafety: settlement '" + settlement.StringId + "' Notables is null. Skipping update.");
						result = false;
					}
					else
					{
						bool flag = false;
						for (int i = 0; i < notables.Count; i++)
						{
							if (notables[i] == null)
							{
								flag = true;
								break;
							}
						}
						if (flag)
						{
							try
							{
								Reflector.InvokeMethod(settlement, "CollectNotablesToCache", null, Array.Empty<object>());
								notables = settlement.Notables;
							}
							catch (Exception ex)
							{
								Log.Exception(ex, "VolunteerUpdateSafety: failed to rebuild notables cache for '" + settlement.StringId + "'. Skipping update.", null);
								return false;
							}
							for (int j = 0; j < notables.Count; j++)
							{
								if (notables[j] == null)
								{
									Log.Error("VolunteerUpdateSafety: settlement '" + settlement.StringId + "' still has null notables after rebuild. Skipping update.");
									return false;
								}
							}
						}
						for (int k = 0; k < notables.Count; k++)
						{
							Hero hero = notables[k];
							if (hero != null && hero.IsAlive && hero.CanHaveRecruits)
							{
								CharacterObject[] volunteerTypes = hero.VolunteerTypes;
								if (volunteerTypes == null || volunteerTypes.Length != 6)
								{
									CharacterObject[] array = new CharacterObject[6];
									if (volunteerTypes != null)
									{
										int num = Math.Min(6, volunteerTypes.Length);
										for (int l = 0; l < num; l++)
										{
											array[l] = volunteerTypes[l];
										}
									}
									hero.VolunteerTypes = array;
									Log.Warn(string.Concat(new string[]
									{
										"VolunteerUpdateSafety: fixed VolunteerTypes array for notable '",
										hero.StringId,
										"' in settlement '",
										settlement.StringId,
										"'."
									}));
								}
							}
						}
						result = true;
					}
				}
			}
			catch (Exception ex2)
			{
				Log.Exception(ex2, "VolunteerUpdateSafety: suppressed exception in UpdateVolunteersOfNotablesInSettlement for settlement '" + (((settlement != null) ? settlement.StringId : null) ?? "null") + "'.", null);
				result = true;
			}
			return result;
		}

		// Token: 0x0600016C RID: 364 RVA: 0x0000A8E4 File Offset: 0x00008AE4
		[HarmonyPriority(0)]
		private static Exception Finalizer(Exception __exception, Settlement settlement)
		{
			if (__exception == null)
			{
				return null;
			}
			try
			{
				Log.Exception(__exception, "VolunteerUpdateSafety: suppressed exception in UpdateVolunteersOfNotablesInSettlement for settlement '" + (((settlement != null) ? settlement.StringId : null) ?? "null") + "'.", null);
			}
			catch
			{
			}
			return null;
		}
	}
}
