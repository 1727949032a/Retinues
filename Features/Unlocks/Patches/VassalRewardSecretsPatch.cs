using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Retinues.Game.Events;
using Retinues.Game.Wrappers;
using Retinues.GUI.Helpers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace Retinues.Features.Unlocks.Patches
{
	// Token: 0x020000B2 RID: 178
	[HarmonyPatch(typeof(LordConversationsCampaignBehavior))]
	internal static class VassalRewardSecretsPatch
	{
		// Token: 0x06000771 RID: 1905 RVA: 0x0002628C File Offset: 0x0002448C
		[HarmonyPatch("AddLordLiberateConversations")]
		[HarmonyPostfix]
		private static void AddSecretOption(CampaignGameStarter starter)
		{
			if (starter == null)
			{
				return;
			}
			starter.AddPlayerLine("lord_defeat_vassal_secrets", "defeated_lord_answer", "lord_defeat_vassal_secrets_ruler_answer", L.S("vassal_secrets_option", "Reveal the secrets of your most precious artifacts and I will let you go."), new ConversationSentence.OnConditionDelegate(VassalRewardSecretsPatch.Condition), new ConversationSentence.OnConsequenceDelegate(VassalRewardSecretsPatch.Consequence), 100, null, null);
			starter.AddDialogLine("lord_defeat_vassal_secrets_ruler_answer_line", "lord_defeat_vassal_secrets_ruler_answer", "close_window", L.S("vassal_secrets_ruler_answer", "Very well... I will share the knowledge of these relics, if it buys my freedom."), null, null, 100, null);
		}

		// Token: 0x06000772 RID: 1906 RVA: 0x00026308 File Offset: 0x00024508
		private static bool Condition()
		{
			Campaign campaign = Campaign.Current;
			if (campaign == null || campaign.CurrentConversationContext != ConversationContext.CapturedLord)
			{
				return false;
			}
			Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
			if (oneToOneConversationHero == null)
			{
				return false;
			}
			Kingdom kingdom = oneToOneConversationHero.MapFaction as Kingdom;
			if (kingdom == null)
			{
				return false;
			}
			if (kingdom.Leader != oneToOneConversationHero)
			{
				return false;
			}
			CultureObject culture = kingdom.Culture;
			if (culture == null || culture.VassalRewardItems == null || culture.VassalRewardItems.Count == 0)
			{
				return false;
			}
			Battle battle = new Battle(null);
			if (battle.AllyLeaders.Count<WCharacter>() > 0)
			{
				return false;
			}
			Log.Info(battle.TotalTroopCount.ToString());
			return (from i in culture.VassalRewardItems
			where i != null
			select new WItem(i)).Any((WItem wi) => !wi.IsUnlocked && wi.IsVassalRewardItem);
		}

		// Token: 0x06000773 RID: 1907 RVA: 0x00026414 File Offset: 0x00024614
		private static void Consequence()
		{
			Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
			if (oneToOneConversationHero == null)
			{
				return;
			}
			Kingdom kingdom = oneToOneConversationHero.MapFaction as Kingdom;
			if (kingdom == null)
			{
				return;
			}
			CultureObject culture = kingdom.Culture;
			if (culture == null || culture.VassalRewardItems == null || culture.VassalRewardItems.Count == 0)
			{
				return;
			}
			List<WItem> list = new List<WItem>();
			try
			{
				foreach (ItemObject itemObject in culture.VassalRewardItems)
				{
					if (itemObject != null)
					{
						WItem witem = new WItem(itemObject);
						if (witem.IsVassalRewardItem && !witem.IsUnlocked)
						{
							witem.Unlock();
							list.Add(witem);
							Log.Info(string.Concat(new string[]
							{
								"VassalRewardSecrets: Unlocked vassal reward item '",
								witem.StringId,
								"' for culture '",
								culture.StringId,
								"'."
							}));
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
			try
			{
				Campaign.Current.CurrentConversationContext = ConversationContext.Default;
				TakePrisonerAction.Apply(PartyBase.MainParty, oneToOneConversationHero);
				EndCaptivityAction.ApplyByReleasedAfterBattle(oneToOneConversationHero);
			}
			catch (Exception ex2)
			{
				Log.Exception(ex2, "", null);
			}
			if (list.Count == 0)
			{
				return;
			}
			string variable = string.Join(", ", from i in list
			select i.Name);
			Sound.Play2D("event:/ui/notification/education");
			Notifications.Popup(L.T("vassal_secrets_title", "Secrets Unlocked"), L.T("vassal_secrets_unlocked_body", "You have unlocked the secrets of: {ITEMS}.").SetTextVariable("CULTURE", (culture != null) ? culture.Name : null).SetTextVariable("ITEMS", variable), null, true);
		}

		// Token: 0x040001E9 RID: 489
		private const string PlayerLineId = "lord_defeat_vassal_secrets";

		// Token: 0x040001EA RID: 490
		private const string PlayerLineToken = "defeated_lord_answer";

		// Token: 0x040001EB RID: 491
		private const string PlayerLineOutput = "lord_defeat_vassal_secrets_ruler_answer";

		// Token: 0x040001EC RID: 492
		private const string RulerLineId = "lord_defeat_vassal_secrets_ruler_answer_line";

		// Token: 0x040001ED RID: 493
		private const string RulerLineInput = "lord_defeat_vassal_secrets_ruler_answer";

		// Token: 0x040001EE RID: 494
		private const string RulerLineOutput = "close_window";
	}
}
