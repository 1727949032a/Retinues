using System;
using System.Collections.Generic;
using HarmonyLib;
using Retinues.Configuration;
using Retinues.Utils;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Retinues.Features.Formations.Patches
{
	// Token: 0x020000C4 RID: 196
	[HarmonyPatch(typeof(FormationQuerySystem), "get_MainClass")]
	internal static class FormationQuerySystemPatch
	{
		// Token: 0x06000812 RID: 2066 RVA: 0x00028A64 File Offset: 0x00026C64
		private static bool Prefix(FormationQuerySystem __instance, ref FormationClass __result)
		{
			bool result;
			try
			{
				if (!Config.AllowFormationOverrides)
				{
					result = true;
				}
				else
				{
					Mission mission = Mission.Current;
					if (mission != FormationQuerySystemPatch._cachedMission)
					{
						FormationQuerySystemPatch._cachedMission = mission;
						FormationQuerySystemPatch._cache.Clear();
					}
					Formation formation = (__instance != null) ? __instance.Formation : null;
					if (formation == null)
					{
						result = true;
					}
					else
					{
						FormationQuerySystemPatch.CacheEntry orUpdateEntry = FormationQuerySystemPatch.GetOrUpdateEntry(formation);
						if (orUpdateEntry.HasForced)
						{
							__result = orUpdateEntry.Forced;
							result = false;
						}
						else
						{
							result = true;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
				result = true;
			}
			return result;
		}

		// Token: 0x06000813 RID: 2067 RVA: 0x00028AF4 File Offset: 0x00026CF4
		private static FormationQuerySystemPatch.CacheEntry GetOrUpdateEntry(Formation formation)
		{
			Mission mission = Mission.Current;
			float num = (mission != null) ? mission.CurrentTime : 0f;
			int countOfUnits = formation.CountOfUnits;
			FormationQuerySystemPatch.CacheEntry cacheEntry;
			if (!FormationQuerySystemPatch._cache.TryGetValue(formation, out cacheEntry))
			{
				cacheEntry = new FormationQuerySystemPatch.CacheEntry
				{
					LastUnitCount = countOfUnits,
					LastUpdateTime = num
				};
				FormationQuerySystemPatch.TryRefreshForcedClass(formation, countOfUnits, num, cacheEntry);
				FormationQuerySystemPatch._cache[formation] = cacheEntry;
				return cacheEntry;
			}
			if (cacheEntry.LastUnitCount != countOfUnits && (num - cacheEntry.LastUpdateTime >= 0.2f || cacheEntry.LastUnitCount == 0))
			{
				cacheEntry.LastUnitCount = countOfUnits;
				cacheEntry.LastUpdateTime = num;
				FormationQuerySystemPatch.TryRefreshForcedClass(formation, countOfUnits, num, cacheEntry);
			}
			return cacheEntry;
		}

		// Token: 0x06000814 RID: 2068 RVA: 0x00028B90 File Offset: 0x00026D90
		private static void TryRefreshForcedClass(Formation formation, int total, float currentTime, FormationQuerySystemPatch.CacheEntry entry)
		{
			FormationClass forced;
			if (FormationQuerySystemPatch.TryComputeForcedClass(formation, total, out forced))
			{
				entry.HasForced = true;
				entry.Forced = forced;
				return;
			}
			entry.HasForced = false;
		}

		// Token: 0x06000815 RID: 2069 RVA: 0x00028BC0 File Offset: 0x00026DC0
		private static bool TryComputeForcedClass(Formation formation, int total, out FormationClass forced)
		{
			forced = FormationClass.Infantry;
			bool result;
			try
			{
				if (total <= 0)
				{
					result = false;
				}
				else
				{
					int num = 0;
					int num2 = 0;
					int num3 = 0;
					int num4 = 0;
					int num5 = 0;
					foreach (IFormationUnit formationUnit in formation.UnitsWithoutLooseDetachedOnes)
					{
						Agent agent = formationUnit as Agent;
						BasicCharacterObject basicCharacterObject = (agent != null) ? agent.Character : null;
						if (basicCharacterObject != null)
						{
							if (FormationQuerySystemPatch.IsRetinuesCustom(basicCharacterObject))
							{
								num++;
							}
							switch (basicCharacterObject.DefaultFormationClass)
							{
							case FormationClass.Ranged:
								num3++;
								break;
							case FormationClass.Cavalry:
								num4++;
								break;
							case FormationClass.HorseArcher:
								num5++;
								break;
							default:
								num2++;
								break;
							}
						}
					}
					if (num * 100 / total < 70)
					{
						result = false;
					}
					else
					{
						forced = FormationQuerySystemPatch.DominantClass(num2, num3, num4, num5);
						Mission mission = Mission.Current;
						if (mission != null)
						{
							if (!mission.IsSiegeBattle)
							{
								if (!mission.IsSallyOutBattle)
								{
									goto IL_FD;
								}
								Team team = formation.Team;
								if (team == null || team.Side != BattleSideEnum.Attacker)
								{
									goto IL_FD;
								}
							}
							forced = forced.DismountedClass();
						}
						IL_FD:
						result = true;
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
				forced = FormationClass.Infantry;
				result = false;
			}
			return result;
		}

		// Token: 0x06000816 RID: 2070 RVA: 0x00028D00 File Offset: 0x00026F00
		private static FormationClass DominantClass(int inf, int rng, int cav, int har)
		{
			if (inf >= rng && inf >= cav && inf >= har)
			{
				return FormationClass.Infantry;
			}
			if (rng >= cav && rng >= har)
			{
				return FormationClass.Ranged;
			}
			if (cav >= har)
			{
				return FormationClass.Cavalry;
			}
			return FormationClass.HorseArcher;
		}

		// Token: 0x06000817 RID: 2071 RVA: 0x00028D24 File Offset: 0x00026F24
		private static bool IsRetinuesCustom(BasicCharacterObject c)
		{
			bool result;
			try
			{
				string text = (c != null) ? c.StringId : null;
				result = (text != null && text.StartsWith("retinues_custom_", StringComparison.Ordinal));
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
				result = false;
			}
			return result;
		}

		// Token: 0x04000218 RID: 536
		private const float RecomputeDelaySeconds = 0.2f;

		// Token: 0x04000219 RID: 537
		private static readonly Dictionary<Formation, FormationQuerySystemPatch.CacheEntry> _cache = new Dictionary<Formation, FormationQuerySystemPatch.CacheEntry>();

		// Token: 0x0400021A RID: 538
		private static Mission _cachedMission;

		// Token: 0x02000194 RID: 404
		private sealed class CacheEntry
		{
			// Token: 0x040004D6 RID: 1238
			public int LastUnitCount;

			// Token: 0x040004D7 RID: 1239
			public float LastUpdateTime;

			// Token: 0x040004D8 RID: 1240
			public bool HasForced;

			// Token: 0x040004D9 RID: 1241
			public FormationClass Forced;
		}
	}
}
