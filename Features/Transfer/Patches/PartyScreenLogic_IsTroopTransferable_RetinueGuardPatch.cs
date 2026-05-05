using System;
using HarmonyLib;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;

namespace Retinues.Features.Transfer.Patches
{
	// Token: 0x020000B5 RID: 181
	[HarmonyPatch(typeof(PartyScreenLogic))]
	internal static class PartyScreenLogic_IsTroopTransferable_RetinueGuardPatch
	{
		// Token: 0x06000776 RID: 1910 RVA: 0x000265FC File Offset: 0x000247FC
		[HarmonyPostfix]
		[HarmonyPatch("IsTroopTransferable")]
		[HarmonyPriority(0)]
		private static void Postfix(PartyScreenLogic __instance, PartyScreenLogic.TroopType troopType, CharacterObject character, int side, ref bool __result)
		{
			try
			{
				if (__result)
				{
					if (character != null)
					{
						if (troopType == PartyScreenLogic.TroopType.Member)
						{
							if (new WCharacter(character).IsRetinue)
							{
								byte b = (byte)side;
								PartyBase partyBase = (b == 0) ? __instance.LeftOwnerParty : __instance.RightOwnerParty;
								object obj = (b == 0) ? __instance.RightOwnerParty : __instance.LeftOwnerParty;
								bool flag = partyBase != null;
								bool flag2 = obj != null;
								bool flag3 = partyBase != null && partyBase.MobileParty == MobileParty.MainParty;
								if (PartyScreenContext.IsCreateClanPartyScreenActive && __instance.LeftOwnerParty == null && __instance.RightOwnerParty != null && __instance.RightOwnerParty.MobileParty == MobileParty.MainParty)
								{
									__result = false;
								}
								else if (flag && flag2 && (flag3 || !flag3))
								{
									__result = false;
								}
								else if (flag && !flag3)
								{
									__result = false;
								}
							}
						}
					}
				}
			}
			catch (Exception arg)
			{
				Log.Warn(string.Format("[Retinues] PartyScreenLogic.IsTroopTransferable retinue guard failed: {0}", arg));
			}
		}
	}
}
