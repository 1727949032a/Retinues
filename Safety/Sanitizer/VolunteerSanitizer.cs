using System;
using Retinues.Game.Wrappers;
using Retinues.Troops;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;

namespace Retinues.Safety.Sanitizer
{
	// Token: 0x02000044 RID: 68
	[SafeClass]
	public static class VolunteerSanitizer
	{
		// Token: 0x06000160 RID: 352 RVA: 0x0000A0E4 File Offset: 0x000082E4
		public static void SanitizeSettlement(Settlement settlement, bool replaceAllCustom = false)
		{
			if (settlement == null)
			{
				return;
			}
			foreach (Hero hero in settlement.Notables)
			{
				if (hero != null)
				{
					VolunteerSanitizer.SanitizeNotable(hero, settlement, replaceAllCustom);
				}
			}
		}

		// Token: 0x06000161 RID: 353 RVA: 0x0000A140 File Offset: 0x00008340
		private static void SanitizeNotable(Hero notable, Settlement settlement, bool replaceAllCustom = false)
		{
			if (settlement == null)
			{
				return;
			}
			if (((notable != null) ? notable.VolunteerTypes : null) == null)
			{
				return;
			}
			for (int i = 0; i < notable.VolunteerTypes.Length; i++)
			{
				try
				{
					CharacterObject characterObject = notable.VolunteerTypes[i];
					if (characterObject != null)
					{
						if (!SanitizerBehavior.IsCharacterValid(characterObject, replaceAllCustom))
						{
							Log.Warn(string.Format("Invalid volunteer troop '{0}' found at notable '{1}' in settlement '{2}'.", characterObject.StringId, notable.Name, settlement.Name));
							WCharacter troop = new WCharacter(characterObject);
							CharacterObject @base = TroopMatcher.PickBestFromFaction(new WSettlement(settlement).Culture, troop, true, true, null).Base;
							if (@base != null)
							{
								Log.Warn(string.Format("Fallback found, replacing with '{0}'", @base));
								notable.VolunteerTypes[i] = @base;
							}
							else
							{
								Log.Warn("No fallback found, removing volunteer entry.");
								notable.VolunteerTypes[i] = null;
							}
						}
					}
				}
				catch (Exception ex)
				{
					Log.Exception(ex, string.Format("Exception while processing notable '{0}' in settlement '{1}'", notable.Name, settlement.Name), null);
				}
			}
		}
	}
}
