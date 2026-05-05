using System;
using System.Collections.Generic;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.ObjectSystem;

namespace Retinues.Safety.Sanitizer
{
	// Token: 0x02000043 RID: 67
	[SafeClass]
	public class SanitizerBehavior : CampaignBehaviorBase
	{
		// Token: 0x06000159 RID: 345 RVA: 0x00009EC4 File Offset: 0x000080C4
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x0600015A RID: 346 RVA: 0x00009EC6 File Offset: 0x000080C6
		public override void RegisterEvents()
		{
			CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.OnGameLoadFinished));
		}

		// Token: 0x0600015B RID: 347 RVA: 0x00009EDF File Offset: 0x000080DF
		private void OnGameLoadFinished()
		{
			Log.Info("Performing safety checks...");
			SanitizerBehavior.Sanitize(false);
		}

		// Token: 0x0600015C RID: 348 RVA: 0x00009EF4 File Offset: 0x000080F4
		public static void Sanitize(bool replaceAllCustom = false)
		{
			if (replaceAllCustom)
			{
				SanitizerBehavior.PurgeCustomTroopDefinitions();
			}
			foreach (MobileParty mp in MobileParty.All)
			{
				PartySanitizer.SanitizeParty(mp, replaceAllCustom);
			}
			foreach (Settlement settlement in Campaign.Current.Settlements)
			{
				MobileParty mp2;
				if (settlement == null)
				{
					mp2 = null;
				}
				else
				{
					Town town = settlement.Town;
					mp2 = ((town != null) ? town.GarrisonParty : null);
				}
				PartySanitizer.SanitizeParty(mp2, replaceAllCustom);
				MobileParty mp3;
				if (settlement == null)
				{
					mp3 = null;
				}
				else
				{
					MilitiaPartyComponent militiaPartyComponent = settlement.MilitiaPartyComponent;
					mp3 = ((militiaPartyComponent != null) ? militiaPartyComponent.MobileParty : null);
				}
				PartySanitizer.SanitizeParty(mp3, replaceAllCustom);
				VolunteerSanitizer.SanitizeSettlement(settlement, replaceAllCustom);
			}
		}

		// Token: 0x0600015D RID: 349 RVA: 0x00009FD0 File Offset: 0x000081D0
		private static void PurgeCustomTroopDefinitions()
		{
			try
			{
				foreach (string text in new List<string>(WCharacter.ActiveStubIds))
				{
					if (WCharacter.ActiveStubIds.Contains(text))
					{
						WCharacter wcharacter = WCharacter.FromStringId(text);
						if (wcharacter != null && wcharacter.IsCustom && !(wcharacter.Parent != null))
						{
							wcharacter.Remove(null);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "PurgeCustomTroopDefinitions failed.", null);
			}
		}

		// Token: 0x0600015E RID: 350 RVA: 0x0000A078 File Offset: 0x00008278
		public static bool IsCharacterValid(CharacterObject c, bool replaceAllCustom = false)
		{
			if (c == null)
			{
				return false;
			}
			WCharacter wcharacter = new WCharacter(c);
			if (wcharacter == null || !wcharacter.IsValid)
			{
				return false;
			}
			if (replaceAllCustom && (wcharacter.IsCustom || wcharacter.IsLegacyCustom))
			{
				return false;
			}
			MBObjectManager instance = MBObjectManager.Instance;
			CharacterObject characterObject = (instance != null) ? instance.GetObject<CharacterObject>(c.StringId) : null;
			return characterObject == c || characterObject != null;
		}
	}
}
